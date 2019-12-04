using System.Linq;
using System.Threading.Tasks;
using MinMq.Service.Entities;
using MinMq.Service.Models;
using NodaTime;
using Optional;

namespace MinMq.Service.Repository
{
	public class MimeTypeRepository : IMimeTypeRepository
	{
		private readonly MessageQueueContext messageQueueContext;

		public MimeTypeRepository(MessageQueueContext messageQueueContext)
		{
			this.messageQueueContext = messageQueueContext;
		}

		public async Task<short> Add(MimeType mimeType)
		{
			var mimeTypeDo = await messageQueueContext.tMimeTypes.SingleOrDefaultAsync(q => q.Expression == mimeType.Expression);

			var now = SystemClock.Instance.GetCurrentInstant().InUtc().ToDateTimeUtc();

			if (mimeTypeDo != null)
			{
				mimeTypeDo.Changed = now;
				await messageQueueContext.SaveChangesAsync();
				return mimeTypeDo.MimeTypeId;
			}

			mimeTypeDo = new tMimeType
			{
				Expression = mimeType.Expression,
				Changed = now,
				Added = now
			};

			await messageQueueContext.AddAsync(mimeTypeDo);
			await messageQueueContext.SaveChangesAsync();
			mimeTypeDo = await messageQueueContext.tMimeTypes.SingleOrDefaultAsync(q => q.Expression == mimeType.Expression);
			return mimeTypeDo.MimeTypeId;
		}

		public async Task<Option<MimeType>> Find(string expression)
		{
			var mimeType = (await messageQueueContext.tMimeTypes.SingleOrDefaultAsync(q => q.Expression == expression)).SomeNotNull();

			return mimeType.Match
			(
				some: mt => new MimeType(mt.MimeTypeId, mt.Expression).Some(),
				none: () => Option.None<MimeType>()
			);
		}

		public void Dispose()
		{
			messageQueueContext?.Dispose();
		}
	}
}
