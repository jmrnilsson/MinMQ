using System;

namespace MinMQ.BenchmarkConsole
{
	public class MinMQEnvironmentVariables
	{
		public MinMQEnvironmentVariables()
		{
			RequestPath = Environment.GetEnvironmentVariable("REQUEST_PATH") ?? "http://localhost:9000/send";
		}

		public string RequestPath { get; }
    }
}
