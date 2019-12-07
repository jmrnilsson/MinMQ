using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinMq.Service.Entities;
using MinMq.Service.Models;
using NodaTime;
using Optional;

namespace MinMq.Service.Repository
{
	public class CursorRepository : ICursorRepository
	{
		private readonly MessageQueueContext messageQueueContext;

		public CursorRepository(MessageQueueContext messageQueueContext)
		{
			this.messageQueueContext = messageQueueContext;
		}

		public async Task<int> Update(Cursor cursor)
		{
			var now = SystemClock.Instance.GetCurrentInstant().InUtc().ToDateTimeUtc();

			tCursor cursorDo = await messageQueueContext.tCursors.SingleOrDefaultAsync(q => q.CursorId == cursor.Id);
			cursorDo.Changed = now;
			await messageQueueContext.SaveChangesAsync();
			return cursorDo.CursorId;
		}

		public async Task<int> Add(Cursor cursor)
		{
			var now = SystemClock.Instance.GetCurrentInstant().InUtc().ToDateTimeUtc();

			tCursor cursor_ = new tCursor
			{
				Changed = now,
				Added = now,
				NextReferenceId = cursor.NextAddress
			};

			await messageQueueContext.AddAsync(cursor_);
			await messageQueueContext.SaveChangesAsync();
			return cursor_.CursorId;
		}

		public void Dispose()
		{
			messageQueueContext?.Dispose();
		}

		public async Task<Cursor> Find(int cursorId)
		{
			return await
			(
				from q in (IAsyncEnumerable<tCursor>)messageQueueContext.tCursors
				where q.CursorId == cursorId
				select new Cursor(q.CursorId, q.NextReferenceId)
			).SingleOrDefaultAsync();
		}

		public async Task<Cursor> FindOr(int cursorId, Func<Task<Cursor>> valueFactory)
		{
			var cursor = await Find(cursorId);
			return cursor != null ? cursor : await valueFactory();
		}
	}
}
