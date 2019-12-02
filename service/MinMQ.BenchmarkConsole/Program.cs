using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Optional;

namespace MinMQ.BenchmarkConsole
{
	public delegate void OnCompleteDelegate();

	public class Program
	{
		public static readonly int ShowProgressEvery = 200;
		public static readonly int NTree = 5;  // NTree = 2 == binary tree
		private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
		public static int NumberOfObjects { get; set; } = 1000;

		public static async Task Main(string[] args)
		{
			ParseArguments(args).MatchSome(value => NumberOfObjects = value);

			var builder = new HostBuilder()
				.ConfigureServices((hostContext, services) =>
				{
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
