namespace MinMq.Service.Entities
{
	public class DebugCursor : Cursor
	{
		private int iteration = 0;

		public DebugCursor(int id, long nextAddress)
            : base(id, nextAddress)
		{
		}

		public int Iteration => iteration;

		public void Increment()
		{
			iteration++;
		}

	}
}
