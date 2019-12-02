﻿using System.Collections.Generic;
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

namespace MinMQ.Service.Faster
{
	internal delegate void EndOfFileCallback(long address);

	/// <summary>
	/// A hosted service that moves stuff from the FASTER log to some EF-providers data context.
	/// </summary>
	public class FasterHostedServiceMoveData : IHostedService, 
	{
		private const int DelayMs = 1000;
		private readonly ILogger<FasterHostedServiceMoveData> logger;
		private readonly IServiceScopeFactory scopeFactory;
		private readonly IOptionsMonitor<MinMQConfiguration> optionsMonitor;
		private int delayCoefficient = 1;

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
				using (var messageRepository = scope.ServiceProvider.GetRequiredService<IMessageRepository>())
				{
					while (true)
					{
						await Task.Delay(DelayMs * delayCoefficient);
						delayCoefficient = 1;
						// Have a sneaking suspicions we should keep the nextAddress from the last iteration
						var scanner = FasterOps.Instance.Value.ListenAsync(optionsMonitor.CurrentValue.ScanFlushSize);
						var messages = await ToList(scanner, FasterOps.Instance.Value.TruncateUntil);
						if (!messages.Any())
						{
							delayCoefficient = 10;
							logger.LogInformation("Nothing to flush");
							continue;
						}
						var lastReferenceId = await messageRepository.AddRange(messages);
						lastReferenceId.MatchSome(referenceId => FasterOps.Instance.Value.TruncateUntil(referenceId));
						logger.LogInformation("Flushed records");
					}
				}
			}
		}

		// Maybe this also should be IAsyncEnumerable. Or, maybe not yet..
		private async Task<List<Message>> ToList(IAsyncEnumerable<(string, long, long)> scan, EndOfFileCallback endOfFileCallback)
		{
			var messages = new List<Message>();

			await foreach ((string content, long referenceId, long nextReferenceId) in scan)
			{
				// I'm guessing this is out of bounds for the current storage config
				if (nextReferenceId > 1000_000_000)
				{
					logger.LogError("Reached end of IDevice");
					// Debugger.Break();
					endOfFileCallback(nextReferenceId);
					continue;
				}
				messages.Add(new Message(content, referenceId, nextReferenceId));
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
