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

		public async Task<short> Add(Queue queue)
		{
			tQueue queueDo = await messageQueueContext.tQueues.SingleOrDefaultAsync(q => q.Name == queue.Name);

			var now = SystemClock.Instance.GetCurrentInstant().InUtc().ToDateTimeUtc();

			if (queueDo != null)
			{
				queueDo.Changed = now;
				await messageQueueContext.SaveChangesAsync();
				return queueDo.QueueId;
			}

			queueDo = new tQueue
			{
				Name = queue.Name,
				Changed = now,
				Added = now
			};

			await messageQueueContext.AddAsync(queueDo);
			await messageQueueContext.SaveChangesAsync();
			queueDo = await messageQueueContext.tQueues.SingleOrDefaultAsync(q => q.Name == queue.Name);
			return queueDo.QueueId;
		}

		public void Dispose()
		{
			messageQueueContext?.Dispose();
		}
	}
}
