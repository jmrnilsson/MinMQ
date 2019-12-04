using System;
using System.Threading.Tasks;
using MinMq.Service.Entities;

namespace MinMq.Service.Repository
{
	public interface IQueueRepository : IDisposable
	{
		Task<short> Add(Queue queues);
	}
}
