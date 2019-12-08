using Optional;

namespace MinMq.Service.Entities
{
	public class Cursor
	{
		// private int currentAddress;
		private long nextAddress;
		private int? id;

		public Cursor(int id, long nextAddress)
		{
			this.id = id;
			this.nextAddress = nextAddress;
		}
		public Cursor()
		{
			this.nextAddress = 0;
		}

		public int? Id => id;
		// public int CurrentAddress => id;
		public long NextAddress => nextAddress;

		public void Set(Option<long> nextAddress)
		{
			nextAddress.MatchSome(next => this.nextAddress = next);
		}

		public void Set(long nextAddress)
		{
			this.nextAddress = nextAddress;
		}
	}
}
