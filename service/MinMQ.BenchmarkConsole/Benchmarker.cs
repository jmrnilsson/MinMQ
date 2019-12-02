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
		private const int SchedularLimit = 14;
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
			var tasks = new List<Task>();
			var limitedScheduler = new LimitedConcurrencyLevelTaskScheduler(SchedularLimit);
			TaskFactory factory = new TaskFactory(limitedScheduler);

			List<ConcurrencyPlan> plans = documents.ToConcurrencyPlan(SchedularLimit, ConcurrentHttpRequests);

			//int batch = documents.Count / SchedularLimit;
			//int modulus = documents.Count % SchedularLimit;
			//int concurrencyPerBatch = ConcurrentHttpRequests / batch;
			//int concurrencyPerBatchModulus = ConcurrentHttpRequests % batch;
			// Func<int, int> length = startIndex => Math.Min(startIndex + concurrencyPerBatch + modulus + concurrencyPerBatchModulus, documents.Count);

			//{
			//	var task = factory.StartNew(async () =>
			//	{
			//		await PostSendAsStringContentPart(documents, 0, concurrencyPerBatch + modulus + concurrencyPerBatchModulus);
			//	}, TaskCreationOptions.LongRunning);

			//	tasks.Add(task);
			//}



			foreach (ConcurrencyPlan plan in plans)
			{
				var task = factory.StartNew(async () =>
				{
					await PostSendAsStringContentPart(documents, plan.Index, plan.Length);
				}, TaskCreationOptions.LongRunning);

				tasks.Add(task);
			}

			await Task.WhenAll(tasks);
		}

		private async Task PostSendAsStringContentPart(List<string> documents, int index, int length)
		{
			List<Task> tasks = new List<Task>();
			int j = index;

			while (j < length)
			{
				if (cancellationToken.IsCancellationRequested) return;  // Task.FromCanceled(cancellationToken);

				HttpClient httpClient = httpClientFactory.CreateClient();
				StringContent content = new StringContent(documents[j]);
				tasks.Add(httpClient.PostAsync("http://localhost:9000/send", content)); // It seems a CancellationToken here will fail the service.

				j++;
			}

			await Task.WhenAll(tasks);
		}
	}
}
