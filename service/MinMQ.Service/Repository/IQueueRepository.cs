using System.Threading.Tasks;
using MinMq.Service.Entities;

namespace MinMq.Service.Repository
{
	public interface IQueueRepository
	{
		Task<short> Add(Queue queues);
	}
}
