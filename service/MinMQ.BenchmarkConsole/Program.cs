using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NodaTime;

namespace MinMQ.BenchmarkConsole
{
	public class Program
	{
		private const int ShowProgressEvery = 200;
		private const int TotalNumberOfObject = 1000;
		private const int NTree = 5;  // NTree = 2 == binary tree

		public static async Task Main(string[] args)
		{
			var jsons = new List<string>();
			var xmls = new List<string>();
			var jsonGenerator = new JsonGenerator(NTree);
			var xmlGenerator = new XmlGenerator(NTree);

			Console.WriteLine("Preparing payload");
			Instant start = SystemClock.Instance.GetCurrentInstant();

			for (int i = 0; i < TotalNumberOfObject; i++)
			{
				if (i > 0 && i % ShowProgressEvery == 0)
				{
					Console.WriteLine($"{Math.Floor((decimal)i * 100 / TotalNumberOfObject)}%");
				}

				jsons.Add(jsonGenerator.GenerateObject());
				xmls.Add(xmlGenerator.GenerateObject());
			}
			Duration duration = SystemClock.Instance.GetCurrentInstant() - start;
			Console.WriteLine("Done! {0:N2} documents/s", TotalNumberOfObject / (decimal)duration.TotalSeconds);

			Console.Write($"Sending JSON...");
			start = SystemClock.Instance.GetCurrentInstant();
			for (int i = 0; i < jsons.Count; i++)
			{
				using (HttpClient httpClient = new HttpClient())
				{
					StringContent content = new StringContent(jsons[i]);
					await httpClient.PostAsync("http://localhost:9000/send", content);
				}
			}

			duration = SystemClock.Instance.GetCurrentInstant() - start;
			Console.WriteLine("Done! {0:N2} requests/s", TotalNumberOfObject / (decimal)duration.TotalSeconds);

			Console.Write("Sending XML..");
			start = SystemClock.Instance.GetCurrentInstant();
			for (int i = 0; i < xmls.Count; i++)
			{
				using (HttpClient httpClient = new HttpClient())
				{
					StringContent content = new StringContent(xmls[i]);
					await httpClient.PostAsync("http://localhost:9000/send", content);
				}
			}
			duration = SystemClock.Instance.GetCurrentInstant() - start;
			Console.WriteLine("Done! {0:N2} requests/s", TotalNumberOfObject / (decimal)duration.TotalSeconds);

			//Console.WriteLine("-----------------JSON----------------");
			//Console.WriteLine();
			//Console.WriteLine(jsons[0]);
			//Console.WriteLine();
			//Console.WriteLine("-----------------XML-----------------");
			//Console.WriteLine();
			//Console.WriteLine(xmls[0]);
			//Console.WriteLine();
		}
	}
}
