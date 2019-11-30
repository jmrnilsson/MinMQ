using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NodaTime;

namespace MinMQ.BenchmarkConsole
{
	public sealed class Benchmarker
	{
		private readonly int ntree;
		private readonly int showProgressEvery;
		private readonly int numberOfObjects;

		public Benchmarker(int ntree, int showProgressEvery, int numberOfObjects)
		{
			this.ntree = ntree;
			this.showProgressEvery = showProgressEvery;
			this.numberOfObjects = numberOfObjects;
		}

		internal async Task Start()
		{
			(List<string> jsons, List<string> xmls) = TimedFunction(() => GenerateObjects(numberOfObjects, out jsons, out xmls));
			await TimedFunction(() => SendAsStringContent(jsons), "Sending JSON..");
			await TimedFunction(() => SendAsStringContent(xmls), "Sending XML..");
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

		private async Task<int> SendAsStringContent(List<string> documents)
		{
			foreach (string document in documents)
			{
				using (HttpClient httpClient = new HttpClient())
				{
					StringContent content = new StringContent(document);
					await httpClient.PostAsync("http://localhost:9000/send", content);
				}
			}

			return documents.Count;
		}
	}
}
