﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace MinMQ.BenchmarkConsole
{
	public class BenchmarkHostedService : IHostedService
	{
		private readonly IHttpClientFactory httpClientFactory;
		private readonly IHostApplicationLifetime hostApplicationLifetime;

		public BenchmarkHostedService(IHttpClientFactory httpClientFactory, IHostApplicationLifetime hostApplicationLifetime)
		{
			this.httpClientFactory = httpClientFactory;
			this.hostApplicationLifetime = hostApplicationLifetime;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			// hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
			hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
			// hostApplicationLifetime.ApplicationStopped.Register(OnStopped);

			var benchmarker = new Benchmarker(httpClientFactory, Program.NTree, Program.NumberOfObjects, cancellationToken);
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
