using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public async Task<Option<long>> AddRange(List<Entities.Message> messages)
		{
			Option<long> nextReferenceId = Option.None<long>();
			var savedMessages = await Find(new FindMessagesQuery(messages));
			var newMessages = messages.Except(savedMessages, new MessageComparer());

			foreach (var message in newMessages)
			{
				var queue = await messageQueueContext.tQueues.SingleOrDefaultAsync(q => q.ByteKey == message.QueueByteKey);

				if (queue == null)
				{
					queue = new tQueue
					{
						ByteKey = message.QueueByteKey,
						Name = "default"
					};

					await messageQueueContext.AddAsync(queue);
					await messageQueueContext.SaveChangesAsync();
				}

				var messageDo = new tMessage
				{
					ReferenceId = message.ReferenceId,
					NextReferenceId = message.NextReferenceId,
					Content = message.Content,
					Queue = queue,
					HashCode = message.HashCode
				};

				messageQueueContext.tMessages.Add(messageDo);

#if TROUBLESHOOT
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
				nextReferenceId = nextReferenceId.Match
				(
					none: () => message.NextReferenceId.Some(),
					some: m => Math.Max(message.NextReferenceId, m).Some()
				);
			}
#if RELEASE || DEBUG
			await messageQueueContext.SaveChangesAsync();
#endif
			return nextReferenceId;
		}

		private async Task<IEnumerable<Entities.Message>> Find(FindMessagesQuery findMessageQuery)
		{
			IAsyncEnumerable<tMessage> messages = messageQueueContext.tMessages;

			var query =
				from m in messages
				where findMessageQuery.Messages.Any(pm => pm.ReferenceId == m.ReferenceId && pm.HashCode == m.HashCode)
				select new Entities.Message
				(
					m.Content,
					m.ReferenceId,
					m.NextReferenceId,
					m.HashCode,
					0x02,
					m.Queue.ByteKey
				);

			return await query.ToListAsync();
		}

		public void Dispose()
		{
			messageQueueContext?.Dispose();
		}

		public class MessageComparer : IEqualityComparer<Entities.Message>
		{
			bool IEqualityComparer<Entities.Message>.Equals(Entities.Message x, Entities.Message y)
			{
				return x.ReferenceId == y.ReferenceId
					&& x.HashCode == y.HashCode;
			}

			int IEqualityComparer<Entities.Message>.GetHashCode(Entities.Message obj)
			{
				return new StringBuilder().Append(obj.ReferenceId).Append(obj.HashCode).ToString().GetHashCode();
			}
		}
	}
}
