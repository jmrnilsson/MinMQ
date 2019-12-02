using System;
using System.Collections.Generic;

namespace MinMQ.BenchmarkConsole
{
	public static class ListExtensions
	{
		public static List<ConcurrencyPlan> ToConcurrencyPlan(this List<string> documents, int schedularLimit, int concurrentHttpRequests)
		{
			var concurrencyPlans = new List<ConcurrencyPlan>();
			int batch = documents.Count / (schedularLimit * 2);
			int modulus = documents.Count % schedularLimit;

			concurrencyPlans.Add(new ConcurrencyPlan(0, batch + modulus));

			for (int i = 1; i < documents.Count / batch; i++)
			{
				int index = (i * batch) + modulus;
				concurrencyPlans.Add(new ConcurrencyPlan(index, batch));
			}

			return concurrencyPlans;
		}
	}
}
