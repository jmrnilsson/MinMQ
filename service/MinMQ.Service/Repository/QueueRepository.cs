using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinMq.Service.Entities;
using MinMq.Service.Models;
using NodaTime;

namespace MinMq.Service.Repository
{
	public class QueueRepository : IQueueRepository
	{
		private readonly MessageQueueContext messageQueueContext;

		public QueueRepository(MessageQueueContext messageQueueContext)
		{
			this.messageQueueContext = messageQueueContext;
		}

		public async Task<short> Update(Queue queue)
		{
			var now = SystemClock.Instance.GetCurrentInstant().InUtc().ToDateTimeUtc();

			tQueue queueDo = await messageQueueContext.tQueues.SingleOrDefaultAsync(q => q.Name == queue.Name);
			queueDo.Changed = now;
			await messageQueueContext.SaveChangesAsync();
			return queueDo.QueueId;
		}

		public async Task<short> Add(Queue queue)
		{
			var now = SystemClock.Instance.GetCurrentInstant().InUtc().ToDateTimeUtc();

			tQueue queue_ = new tQueue
			{
				Name = queue.Name,
				Changed = now,
				Added = now
			};

			await messageQueueContext.AddAsync(queue_);
			await messageQueueContext.SaveChangesAsync();
			return queue_.QueueId;
		}

		public void Dispose()
		{
			messageQueueContext?.Dispose();
		}

		public async Task<short?> Find(string queueName)
		{
			return await
			(
				from q in (IAsyncEnumerable<tQueue>)messageQueueContext.tQueues
				where q.Name == queueName
				select (short?)q.QueueId
			).SingleOrDefaultAsync();
		}

		public async Task<short> FindOr(string queueName, Func<Task<short>> valueFactory)
		{
			var queueId = await Find(queueName);
			return queueId.HasValue ? queueId.Value : await valueFactory();
		}
	}
}
