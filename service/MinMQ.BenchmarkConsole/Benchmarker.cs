using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;

namespace MinMQ.BenchmarkConsole
{
	public sealed class Benchmarker
	{
		private readonly SemaphoreSlim httpSemaphore = new SemaphoreSlim(6, 6);
		private readonly IHttpClientFactory httpClientFactory;
		private readonly int ntree;
		private readonly int showProgressEvery;
		private readonly int numberOfObjects;

		public Benchmarker(IHttpClientFactory httpClientFactory, int ntree, int showProgressEvery, int numberOfObjects)
		{
			this.httpClientFactory = httpClientFactory;
			this.ntree = ntree;
			this.showProgressEvery = showProgressEvery;
			this.numberOfObjects = numberOfObjects;
		}

		internal async Task Start()
		{
			(List<string> jsons, List<string> xmls) = TimedFunction(() => GenerateObjects(numberOfObjects, out jsons, out xmls));

			//await TimedFunction(() => PostSendAsStringContent(jsons), "Sending JSON..");
			//await TimedFunction(() => PostSendAsStringContent(xmls), "Sending XML..");

			Console.Write("Sending JSON and XML..");
			Instant start = SystemClock.Instance.GetCurrentInstant();

			// Old-school non-blocking
			List<Task> tasks = await PostSendAsStringContent(jsons);
			tasks.AddRange(await PostSendAsStringContent(xmls));
			await Task.WhenAll(tasks);

			Duration duration = SystemClock.Instance.GetCurrentInstant() - start;
			Console.WriteLine("Done! {0:N2} documents/s", tasks.Count / (decimal)duration.TotalSeconds);
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

		private async Task<List<Task>> PostSendAsStringContent(List<string> documents)
		{
			List<Task> tasks = new List<Task>();

			try
			{
				await httpSemaphore.WaitAsync();

				foreach (string document in documents)
				{
					HttpClient httpClient = httpClientFactory.CreateClient();

					//using (HttpClient httpClient = httpClientFactory.CreateClient())
					//{
						StringContent content = new StringContent(document);
						tasks.Add(httpClient.PostAsync("http://localhost:9000/send", content));
					//}
				}
			}
			finally
			{
				httpSemaphore.Release();
			}

			return tasks;

		}
	}
}
