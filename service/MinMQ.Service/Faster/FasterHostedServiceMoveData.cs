using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinMq.Service.Entities;
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
						var messages = ToListAsync(scanner);
						var lastReferenceId = await messageRepository.AddRange(messages);
						lastReferenceId.MatchSome(referenceId => FasterOps.Instance.Value.TruncateUntil(referenceId));
						logger.LogInformation("Flushed records");
					}
				}
			}
		}

		// Maybe this also should be IAsyncEnumerable
		private async Task<List<Message>> ToList(IAsyncEnumerable<(string, long, long)> scan)
		{
			var messages = new List<Message>();

			await foreach ((string content, long referenceId, long nextReferenceId) in scan)
			{
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
