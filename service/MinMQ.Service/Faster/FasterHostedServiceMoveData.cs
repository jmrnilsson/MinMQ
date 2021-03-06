﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinMQ.Service.Configuration;
using MinMq.Service.Entities;
using MinMq.Service.Repository;
using NodaTime;

namespace MinMQ.Service.Faster
{
	internal delegate void EndOfFileCallback(long address);
	internal delegate short MimeTypeDecider(string content);

	/// <summary>
	/// A hosted service that moves stuff from the FASTER log to some EF-providers data context.
	/// </summary>
	public class FasterHostedServiceMoveData : IHostedService, IDisposable
	{
		private const int DelayMs = 1000;
		private readonly ILogger<FasterHostedServiceMoveData> logger;
		private readonly IServiceScopeFactory scopeFactory;
		private readonly IOptionsMonitor<MinMQConfiguration> optionsMonitor;
		private int delayCoefficient = 1;
		private Cursor currentCursor = null;

		public FasterHostedServiceMoveData(ILogger<FasterHostedServiceMoveData> logger, IServiceScopeFactory scopeFactory, IOptionsMonitor<MinMQConfiguration> optionsMonitor)
		{
			this.logger = logger;
			this.scopeFactory = scopeFactory;
			this.optionsMonitor = optionsMonitor;
		}

		public async Task StartAsync(CancellationToken stoppingToken)
		{
			logger.LogInformation("{0} service running.", nameof(FasterHostedServiceMoveData));

			using (var scope = scopeFactory.CreateScope())
			{
				using (var queueRepository = scope.ServiceProvider.GetRequiredService<IQueueRepository>())
				using (var mimeTypeRepository = scope.ServiceProvider.GetRequiredService<IMimeTypeRepository>())
				using (var messageRepository = scope.ServiceProvider.GetRequiredService<IMessageRepository>())
				using (var cursorRepository = scope.ServiceProvider.GetRequiredService<ICursorRepository>())
				{
					short queueId = await queueRepository.FindOr("some", async () => await queueRepository.Add(new Queue("some")));

					if (currentCursor == null)
					{
						int cursorId = await cursorRepository.Add(new Cursor());
						currentCursor = await cursorRepository.Find(cursorId);
					}

					while (true)
					{
						await Task.Delay(DelayMs * delayCoefficient);
						delayCoefficient = 1;
						// Have a sneaking suspicions we should keep the nextAddress from the last iteration. If we remove truncate
						// then the cursor doesn't progress and flush next batch.
						Instant start = SystemClock.Instance.GetCurrentInstant();
						var scanner = FasterOps.Instance.Value.Listen(optionsMonitor.CurrentValue.ScanFlushSize, currentCursor);
						// var messages = await ToListAsync(scanner, FasterOps.Instance.Value.TruncateUntil);

						// Sloppy write (queue name and mime type should probably be known before)
						MimeType mimeTypeXml = (await mimeTypeRepository.Find("text/xml")).ValueOr(() => throw new ApplicationException("xml"));
						MimeType mimeTypeJson = (await mimeTypeRepository.Find("application/json")).ValueOr(() => throw new ApplicationException("json"));
						MimeTypeDecider decider = c => c.StartsWith("<") ? mimeTypeXml.MimeTypeId : mimeTypeJson.MimeTypeId;
						var messages = ToList(scanner, decider, queueId);

						if (!messages.Any())
						{
							delayCoefficient = 5;
							logger.LogInformation("Nothing to flush");
							continue;
						}

						var lastReferenceId = await messageRepository.AddRange(messages);
						currentCursor.Set(lastReferenceId);
						lastReferenceId.MatchSome(referenceId => FasterOps.Instance.Value.TruncateUntil(referenceId));
						await cursorRepository.Update(currentCursor);
						Duration elapsed = SystemClock.Instance.GetCurrentInstant() - start;
						logger.LogInformation("Flushed {0} records at {0:N2} documents/s", messages.Count, messages.Count / elapsed.TotalSeconds);
					}
				}
			}
		}

		// Maybe this also should be IAsyncEnumerable. Or, maybe not yet..
		// private async Task<List<Message>> ToListAsync(IAsyncEnumerable<(string, long, long)> scan, EndOfFileCallback endOfFileCallback)
		// {
		//	var messages = new List<Message>();

		//	await foreach ((string content, long referenceId, long nextReferenceId) in scan)
		//	{
		//		// I'm guessing this is out of bounds for the current storage config
		//		if (nextReferenceId > 1_000_000_000)
		//		{
		//			logger.LogError("Reached end of IDevice");
		//			// Debugger.Break();
		//			endOfFileCallback(nextReferenceId);
		//			continue;
		//		}
		//		messages.Add(new Message(content, referenceId, nextReferenceId));
		//	}

		//	return messages;
		// }

		// private List<Message> ToList(List<(string, long, long)> scan, EndOfFileCallback endOfFileCallback, MimeTypeDecider mimeTypeDecider, short queueId)
		private List<Message> ToList(List<(string, long, long)> scan, MimeTypeDecider mimeTypeDecider, short queueId)
		{
			var messages = new List<Message>();

			foreach ((string content, long referenceId, long nextReferenceId) in scan)
			{
				// I'm guessing this is out of bounds for the current storage config
				//if (nextReferenceId > 1_000_000_000)
				//{
				//	logger.LogError("Reached end of IDevice");
				//	// Debugger.Break();
				//	endOfFileCallback(nextReferenceId);
				//	continue;
				//}
				messages.Add(new Message(content, referenceId, nextReferenceId, mimeTypeDecider(content), queueId));
			}

			return messages;
		}

		public Task StopAsync(CancellationToken stoppingToken)
		{
			logger.LogInformation("{0} Service is stopping.", nameof(FasterHostedServiceMoveData));
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			// Nothing to do
		}
	}
}
