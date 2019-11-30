using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using Optional;
using Polly;
using Polly.Registry;

namespace MinMQ.BenchmarkConsole
{
	public class Program
	{
		public static readonly int ShowProgressEvery = 200;
		public static readonly int NTree = 5;  // NTree = 2 == binary tree
		public static int NumberOfObjects { get; set; } = 1000;

		public static async Task Main(string[] args)
		{
			ParseArguments(args).MatchSome(value => NumberOfObjects = value);

			var builder = new HostBuilder()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddHttpClient();
					services.AddSingleton<IHostedService, HostedService>();
					services.AddHostedService<HostedService>();
				});

			await builder.RunConsoleAsync();
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

	public class HostedService : IHostedService
	{
		public HostedService(IHttpClientFactory httpClientFactory)
		{
			HttpClientFactory = httpClientFactory;
		}

		public IHttpClientFactory HttpClientFactory { get; }

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			var benchmarker = new Benchmarker(HttpClientFactory, Program.NTree, Program.ShowProgressEvery, Program.NumberOfObjects);
			await benchmarker.Start();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
