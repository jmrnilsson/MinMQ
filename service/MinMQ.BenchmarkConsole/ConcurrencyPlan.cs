using System.Text;

namespace MinMQ.BenchmarkConsole
{
	public class ConcurrencyPlan
	{
		public ConcurrencyPlan(int index, int lenght)
		{
			Index = index;
			Length = lenght;
		}

		public int Index { get; }
		public int Length { get; }
	}
}
