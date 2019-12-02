using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MinMQ.BenchmarkConsole.Tests
{
    public class ConcurrencyPlanTests
    {
        [Fact]
        public void Sum_is_10_000()
        {
			var list = new List<string>();

			for (int i = 0; i < 10_000; i++)
			{
				list.Add("");
			}

			var plan = list.ToConcurrencyPlan(14, 400);

			plan[0].Index.ShouldBe(0);
			plan[0].Length.ShouldBe(361);
			plan[1].Index.ShouldBe(361);
			plan[1].Length.ShouldBe(10_000 / (14 * 2));
			plan.Sum(p => p.Length).ShouldBe(10_000);
		}
	}
}
