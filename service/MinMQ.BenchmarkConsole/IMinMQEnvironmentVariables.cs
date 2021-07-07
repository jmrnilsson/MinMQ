namespace MinMQ.BenchmarkConsole
{
	public interface IMinMQEnvironmentVariables
	{
		int NTree { get; }
		int NumberOfObjects { get; }
		string RequestPath { get; }
	}
}