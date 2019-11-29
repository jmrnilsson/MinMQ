using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinMq.Service.Entities;
using MinMq.Service.Repository;
using Optional;

namespace MinMQ.Service.Faster
{
	delegate void EndOfFileCallback(long address);

	/// <summary>
	/// A hosted service that moves stuff from the FASTER log to some EF-providers data context.
	/// </summary>
	public class FasterHostedServiceMoveData : IHostedService, IDisposable
	{
		private const int DelayMs = 1000;
		private readonly ILogger<FasterHostedServiceMoveData> logger;
		private readonly IServiceScopeFactory scopeFactory;
		private int delayCoefficient = 1;
		private SemaphoreSlim entityFrameworkFlushSemaphore = new SemaphoreSlim(1, 1);

		public FasterHostedServiceMoveData(ILogger<FasterHostedServiceMoveData> logger, IServiceScopeFactory scopeFactory)
		{
			this.logger = logger;
			this.scopeFactory = scopeFactory;
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
						var scanner = FasterOps.Instance.Value.ListenAsync();
						var messages = await ToList(scanner, FasterOps.Instance.Value.TruncateUntil);
						if (!messages.Any())
						{
							delayCoefficient = 10;
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
		private async Task<List<Message>> ToList(IAsyncEnumerable<(string, long, long)> scan, EndOfFileCallback eofCallback)
		{
			var messages = new List<Message>();

			await foreach ((string content, long referenceId, long nextReferenceId) in scan)
			{
				// I'm guessing this is out of bounds for the current storage config
				if (nextReferenceId > 100_000_000)
				{
					eofCallback(nextReferenceId);
					continue;
				}
				messages.Add(new Message(content, referenceId, nextReferenceId));
			}

			return messages;
		}

		private async IAsyncEnumerable<Message> ToListAsync(IAsyncEnumerable<(string, long, long)> scan)
		{
			await foreach ((string content, long referenceId, long nextReferenceId) in scan)
			{
				yield return new Message(content, referenceId, nextReferenceId);
			}
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
