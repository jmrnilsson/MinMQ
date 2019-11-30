using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NodaTime;

namespace MinMQ.BenchmarkConsole
{
	public class Program
	{
		private const int ShowProgressEvery = 200;
		private const int NTree = 5;  // NTree = 2 == binary tree

		public static async Task Main(string[] args)
		{
			int numberOfObjects = ParseArguments(args);

			var benchmarker = new Benchmarker(NTree, ShowProgressEvery, numberOfObjects);
			await benchmarker.Start();
		}

		private static int ParseArguments(string[] args)
		{
			int numberOfObjects = 1000;

			if (args.Length == 1 && int.TryParse(args[0], out int numberOfObjects_))
			{
				numberOfObjects = numberOfObjects_;
			}

			return numberOfObjects;
		}
	}
}
