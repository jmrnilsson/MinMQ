using System;
using System.Threading.Tasks;
using MinMq.Service.Entities;
using Optional;

namespace MinMq.Service.Repository
{
	public interface IMimeTypeRepository : IDisposable
	{
		Task<short> Add(MimeType mimeType);
		Task<Option<MimeType>> Find(string expression);
	}
}
