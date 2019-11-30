using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;

namespace MinMQ.BenchmarkConsole
{
	public sealed class Benchmarker
	{
		private const int ConcurrentHttpRequests = 400;
		private readonly IHttpClientFactory httpClientFactory;
		private readonly int ntree;
		private readonly int showProgressEvery;
		private readonly int numberOfObjects;
		private readonly CancellationToken cancellationToken;

		public Benchmarker(IHttpClientFactory httpClientFactory, int ntree, int showProgressEvery, int numberOfObjects, CancellationToken cancellationToken)
		{
			this.httpClientFactory = httpClientFactory;
			this.ntree = ntree;
			this.showProgressEvery = showProgressEvery;
			this.numberOfObjects = numberOfObjects;
			this.cancellationToken = cancellationToken;
		}

		public event OnCompleteDelegate OnComplete;

		internal async Task Start()
		{
			(List<string> jsons, List<string> xmls) = TimedFunction(() => GenerateObjects(numberOfObjects, out jsons, out xmls));

			Console.Write("Sending JSON and XML..");
			Instant start = SystemClock.Instance.GetCurrentInstant();

			// Old-school non-blocking
			await PostSendAsStringContent(jsons);
			await PostSendAsStringContent(xmls);
			Duration duration = SystemClock.Instance.GetCurrentInstant() - start;
			Console.WriteLine("Done! {0:N2} documents/s", (jsons.Count + xmls.Count) / (decimal)duration.TotalSeconds);

			OnComplete?.Invoke();
		}

		private (List<string>, List<string>) GenerateObjects(int numberOfObjects, out List<string> jsons, out List<string> xmls)
		{
			jsons = new List<string>();
			xmls = new List<string>();
			var jsonGenerator = new JsonGenerator(ntree);
			var xmlGenerator = new XmlGenerator(ntree);

			Console.WriteLine("Preparing payload");
			for (int i = 0; i < numberOfObjects; i++)
			{
				if (cancellationToken.IsCancellationRequested) return (new List<string>(), new List<string>());

				if (i > 0 && i % showProgressEvery == 0)
				{
					Console.WriteLine("{0}%", Math.Floor((decimal)i * 100 / numberOfObjects));
				}

				jsons.Add(jsonGenerator.GenerateObject());
				xmls.Add(xmlGenerator.GenerateObject());
			}

			return (jsons, xmls);
		}

		private (List<string>, List<string>) TimedFunction(Func<(List<string>, List<string>)> action)
		{
			Instant start = SystemClock.Instance.GetCurrentInstant();
			var objects = action();
			Duration duration = SystemClock.Instance.GetCurrentInstant() - start;
			var perf = (objects.Item1.Count + objects.Item2.Count) / (decimal)duration.TotalSeconds;
			Console.WriteLine("Done! {0:N2} documents/s", perf);
			return objects;
		}

		private async Task TimedFunction(Func<Task<int>> action, string name)
		{
			Console.Write("Sending XML..");
			Instant start = SystemClock.Instance.GetCurrentInstant();
			var count = await action();
			Duration duration = SystemClock.Instance.GetCurrentInstant() - start;
			Console.WriteLine("Done! {0:N2} documents/s", count / (decimal)duration.TotalSeconds);
		}

		private async Task PostSendAsStringContent(List<string> documents)
		{
			int j = 0;
			while (j < documents.Count)
			{
				List<Task> tasks = new List<Task>();

				for (int i = 0; i < ConcurrentHttpRequests && j < documents.Count; i++)
				{
					if (cancellationToken.IsCancellationRequested) return;

					HttpClient httpClient = httpClientFactory.CreateClient();
					StringContent content = new StringContent(documents[i]);
					tasks.Add(httpClient.PostAsync("http://localhost:9000/send", content)); // It seems a CancellationToken here will fail the service.
					if (i == 5)
					{
						await Task.WhenAll(tasks);
						tasks.Clear();
					}

					j++;
				}
			}
		}
	}
}
