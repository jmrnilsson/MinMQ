using System.Threading.Tasks;
using MinMq.Service.Entities;

namespace MinMq.Service.Repository
{
	public interface IMimeTypeRepository
	{
		Task<short> Add(MimeType mimeType);
	}
}
