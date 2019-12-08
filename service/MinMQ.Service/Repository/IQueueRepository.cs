using System;
using System.Threading.Tasks;
using MinMq.Service.Entities;

namespace MinMq.Service.Repository
{
	public interface IQueueRepository : IDisposable
	{
		Task<short?> Find(string queueName);
		Task<short> FindOr(string queueName, Func<Task<short>> valueFactory);
		Task<short> Add(Queue queue);
		Task<short> Update(Queue queue);
	}
}
