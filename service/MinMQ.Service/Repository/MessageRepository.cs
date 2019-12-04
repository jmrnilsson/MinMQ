using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinMq.Service.Models;
using NodaTime;
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
			Option<long> nextReferenceId = Option.None<long>();
			var savedMessages = await Find(new FindMessagesQuery(messages));
			var newMessages = messages.Except(savedMessages, new MessageComparer());

			foreach (var message in newMessages)
			{
				var queue = await messageQueueContext.tQueues.SingleOrDefaultAsync(q => q.QueueId == message.QueueId);
				var now = SystemClock.Instance.GetCurrentInstant().InUtc().ToDateTimeUtc();

				var messageDo = new tMessage
				{
					ReferenceId = message.ReferenceId,
					NextReferenceId = message.NextReferenceId,
					Content = message.Content,
					Queue = queue,
					HashCode = message.HashCode,
					Added = now,
					Changed = now
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
					m.Queue.QueueId
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
