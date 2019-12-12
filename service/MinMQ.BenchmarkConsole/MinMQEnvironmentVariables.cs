using System;

namespace MinMQ.BenchmarkConsole
{
	public class MinMQEnvironmentVariables : IMinMQEnvironmentVariables
	{
		public MinMQEnvironmentVariables(int numberOfObjects = 1000)
		{
			RequestPath = Environment.GetEnvironmentVariable("REQUEST_PATH") ?? "http://localhost:9000/send";
			NumberOfObjects = numberOfObjects;
		}

		public string RequestPath { get; }
		public int NTree { get; } = 5;  // NTree = 2 == binary tree
		public int NumberOfObjects { get; }
	}
}
