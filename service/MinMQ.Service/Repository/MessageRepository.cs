using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinMq.Service.Models;
using Optional;

namespace MinMq.Service.Repository
{
	public class MessageRepository : IMessageRepository
	{
		private readonly MessageQueueContext messageQueueContext;
		private readonly ILogger<MessageRepository> logger;

		public MessageRepository(MessageQueueContext messageQueueContext, ILogger<MessageRepository> logger)
		{
			this.messageQueueContext = messageQueueContext;
			this.logger = logger;
		}

		// public async Task<Option<long>> AddRange(IAsyncEnumerable<Entities.Message> messages)
		public async Task<Option<long>> AddRange(List<Entities.Message> messages)
		{
			Option<long> lastReferenceId = Option.None<long>();

			//await foreach (var message in messages)
			foreach (var message in messages)
			{
				var queue = await messageQueueContext.Queues.SingleOrDefaultAsync(q => q.ByteKey == message.QueueByteKey);

				if (queue == null)
				{
					queue = new Queue
					{
						ByteKey = message.QueueByteKey,
						Name = "default"
					};

					await messageQueueContext.AddAsync(queue);
					await messageQueueContext.SaveChangesAsync();
				}

				var messageDo = new Message
				{
					ReferenceId = message.ReferenceId,
					NextReferenceId = message.NextReferenceId,
					Content = message.Content,
					Queue = queue
				};

				messageQueueContext.Messages.Add(messageDo);

#if TROUBLESHOOT || DEBUG
				try
				{
					await messageQueueContext.SaveChangesAsync();
				}
				catch (Microsoft.EntityFrameworkCore.DbUpdateException error) when (error.InnerException.Message == "22021: invalid byte sequence for encoding \"UTF8\": 0x00")
				{
					logger.LogError("Invalid document. Skipping..");
					continue;
				}
#endif
				lastReferenceId = lastReferenceId.Match
				(
					none: () => message.ReferenceId.Some(),
					some: m => Math.Max(message.ReferenceId, m).Some()
				);
			}
#if RELEASE
			await messageQueueContext.SaveChangesAsync();
#endif
			return lastReferenceId;
		}

		public void Dispose()
		{
			messageQueueContext?.Dispose();
		}
	}
}
