using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			try
			{
				return await GroupCommit(messages);
			}
			catch
			{
				Option<long> lastReferenceId = Option.None<long>();
				long? attemptedReferenceId = null;

				try
				{
					foreach (var message in messages)
					{
						attemptedReferenceId = message.ReferenceId;
						var referenceId = await Add(message);
						lastReferenceId = lastReferenceId.Match
						(
							none: () => message.ReferenceId.Some(),
							some: m => Math.Max(message.ReferenceId, m).Some()
						);
					}
				}
				catch (Exception e)
				{
					logger.LogError("Failed to insert item with ReferenceId={0}. Error={1}", attemptedReferenceId, e);
				}

				return lastReferenceId;
			}
		}

		private async Task<Option<long>> GroupCommit(List<Entities.Message> messages)
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

		private async Task<long> Add(Entities.Message message)
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
			await messageQueueContext.SaveChangesAsync();
			return message.ReferenceId;
		}

		public void Dispose()
		{
			messageQueueContext?.Dispose();
		}
	}
}
