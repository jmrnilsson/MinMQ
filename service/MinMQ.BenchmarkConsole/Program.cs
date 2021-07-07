using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Optional;
using Serilog;

namespace MinMQ.BenchmarkConsole
{
	public delegate void OnCompleteDelegate();

	public class Program
	{
		private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();


		public static async Task Main(string[] args)
		{
			var numberOfObjects = ParseArguments(args);

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.WriteTo.Console()
				.CreateLogger();

			var builder = new HostBuilder()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddSingleton<IMinMQEnvironmentVariables>(numberOfObjects.Match(noo => new MinMQEnvironmentVariables(noo), () => new MinMQEnvironmentVariables()));
					services.AddSingleton<Benchmarker>();
					services.AddHttpClient();
					services.AddHostedService<BenchmarkHostedService>();
				});

			await builder.RunConsoleAsync(CancellationTokenSource.Token);
		}

		public static void OnCompletedEvent()
		{
			CancellationTokenSource.Cancel();
		}

		private static Option<int> ParseArguments(string[] args)
		{
			if (args.Length == 1 && int.TryParse(args[0], out int numberOfObjects_))
			{
				return numberOfObjects_.Some();
			}

			return Option.None<int>();
		}
	}
}
