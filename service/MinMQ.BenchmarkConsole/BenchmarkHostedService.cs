using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace MinMQ.BenchmarkConsole
{
	public class BenchmarkHostedService : IHostedService
	{
		private readonly IHostApplicationLifetime hostApplicationLifetime;
		private readonly BenchmarkerFactory benchmarkerFactory;

		public BenchmarkHostedService(IHttpClientFactory httpClientFactory, IHostApplicationLifetime hostApplicationLifetime, BenchmarkerFactory benchmarkerFactory)
		{
			HttpClientFactory = httpClientFactory;
			this.hostApplicationLifetime = hostApplicationLifetime;
			this.benchmarkerFactory = benchmarkerFactory;
		}

		public IHttpClientFactory HttpClientFactory { get; }

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			// hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
			hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
			// hostApplicationLifetime.ApplicationStopped.Register(OnStopped);

			var benchmarker = benchmarkerFactory.Create(Program.NTree, Program.NumberOfObjects, cancellationToken);
			benchmarker.OnComplete += Program.OnCompletedEvent;
			await benchmarker.Start();
			await StopAsync(cancellationToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await Task.CompletedTask;
		}

		// private void OnStarted()
		// {
		// }

		private void OnStopping()
		{
		}

		// private void OnStopped()
		// {
		// }
	}
}
