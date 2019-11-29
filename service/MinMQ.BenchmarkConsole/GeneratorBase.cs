using System;
using System.Collections.Generic;
using System.Text;

namespace MinMQ.BenchmarkConsole
{
	public abstract class GeneratorBase<T>
	{
		private readonly int n;

		public GeneratorBase(int n)
		{
			this.n = n;
		}

		protected Random Seed { get; set; } = new Random();
		protected abstract T GenerateChild(IEnumerable<T> innerChildren);
		public abstract string GenerateObject();

		protected IEnumerable<T> GenerateChildren(int depth)
		{
			if (depth < 10)
			{
				var count = Seed.Next(0, n);
				for (int i = 0; i < count; i++)
				{
					yield return GenerateChild(GenerateChildren(++depth));
				}
			}
		}

		protected int NumberOfProperties()
		{
			return Seed.Next(0, 6);
		}
	}
}
