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

		public async Task<Option<long>> AddRange(List<Entities.Message> messages)
		{
			Option<long> nextReferenceId = Option.None<long>();
			List<long> ids = messages.Select(mm => mm.ReferenceId).ToList();

			Dictionary<long, KeyValuePair<long, string>> messageDos = await IntersectMessages(ids);

			foreach (var message in messages)
			{
				if (messageDos.TryGetValue(message.ReferenceId, out KeyValuePair<long, string> kvp) && kvp.Value == message.HashCode)
				{
					continue;
				}

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
					Queue = queue,
					HashCode = message.HashCode
				};

				messageQueueContext.Messages.Add(messageDo);

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

		private async Task<Dictionary<long, KeyValuePair<long, string>>> IntersectMessages(List<long> ids)
		{
			return await (
				from m in (IAsyncEnumerable<Message>)messageQueueContext.Messages
				where ids.Any(id => id == m.ReferenceId)
				select new KeyValuePair<long, string>(m.ReferenceId, m.HashCode)
			).ToDictionaryAsync(m => m.Key);
		}

		public void Dispose()
		{
			messageQueueContext?.Dispose();
		}
	}
}
