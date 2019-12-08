using System;
using System.Threading.Tasks;
using MinMq.Service.Entities;

namespace MinMq.Service.Repository
{
	public interface ICursorRepository : IDisposable
	{
		Task<int> Add(Cursor cursor);
		Task<Cursor> Find(int cursorId);
		Task<Cursor> FindOr(int cursorId, Func<Task<Cursor>> valueFactory);
		Task<int> Update(Cursor cursor);
	}
}
