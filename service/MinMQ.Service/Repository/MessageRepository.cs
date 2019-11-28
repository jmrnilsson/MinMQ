using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinMq.Service.Models;
using Optional;

namespace MinMq.Service.Repository
{
	public class MessageRepository : IMessageRepository
	{
		private readonly MessageQueueContext messageQueueContext;

		public MessageRepository(MessageQueueContext messageQueueContext)
		{
			this.messageQueueContext = messageQueueContext;
		}

		public async Task<Option<long>> AddRange(List<Entities.Message> messages)
		{
			Option<long> lastReferenceId = Option.None<long>();

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

				lastReferenceId = lastReferenceId.Match
				(
					none: () => message.ReferenceId.Some(),
					some: m => Math.Max(message.ReferenceId, m).Some()
				);
			}

			await messageQueueContext.SaveChangesAsync();

			return lastReferenceId;
		}

		public void Dispose()
		{
			messageQueueContext?.Dispose();
		}
	}
}
