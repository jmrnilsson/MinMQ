using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MinMQ.BenchmarkConsole
{
	public sealed class BenchmarkerFactory
	{
		private readonly IHttpClientFactory httpClientFactory;
		private readonly ILogger<Benchmarker> logger;

		public BenchmarkerFactory(IHttpClientFactory httpClientFactory, ILogger<Benchmarker> logger)
		{
			this.httpClientFactory = httpClientFactory;
			this.logger = logger;
		}

		public Benchmarker Create(int ntree, int numberOfObjects, CancellationToken cancellationToken)
		{
			return new Benchmarker(httpClientFactory, logger, ntree, numberOfObjects, cancellationToken);
		}
	}
}
