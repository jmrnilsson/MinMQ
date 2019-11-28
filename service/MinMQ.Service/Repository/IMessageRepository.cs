using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MinMq.Service.Entities;
using Optional;

namespace MinMq.Service.Repository
{
	public interface IMessageRepository : IDisposable
	{
		public Task<Option<long>> AddRange(IAsyncEnumerable<Message> messages);
		// public Task<Option<long>> AddRange(List<Message> messages);
	}
}
