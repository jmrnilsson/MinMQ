using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinMq.Service.Repository;

namespace MinMQ.Service.Faster
{
	/// <summary>
	/// A hosted service that moves stuff from the FASTER log to some EF-providers data context.
	/// </summary>
	public class FasterHostedServiceMoveData : IHostedService, IDisposable
	{
		private const int DelayMs = 1000;
		private readonly ILogger<FasterHostedServiceMoveData> logger;
		private readonly IServiceScopeFactory scopeFactory;

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
						await Task.Delay(DelayMs);
						var scanner = FasterOps.Instance.Value.ListenAsync();
						var messages = await ToList(scanner);
						var lastReferenceId = await messageRepository.AddRange(messages);
						lastReferenceId.MatchSome(referenceId => FasterOps.Instance.Value.TruncateUntil(referenceId));
						logger.LogInformation("Flushed records to some EF-providers data context");
					}
				}
			}
		}

		private async Task<List<MinMq.Service.Entities.Message>> ToList(IAsyncEnumerable<(string, long, long)> scan)
		{
			var messages = new List<MinMq.Service.Entities.Message>();

			await foreach ((string content, long referenceId, long nextReferenceId) in scan)
			{
				messages.Add(new MinMq.Service.Entities.Message(content, referenceId, nextReferenceId));
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
