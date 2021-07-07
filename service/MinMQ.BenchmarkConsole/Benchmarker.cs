using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NodaTime;
using Serilog;

namespace MinMQ.BenchmarkConsole
{
	public sealed class Benchmarker
	{
		private const int ConcurrentHttpRequests = 400;
		private readonly IHttpClientFactory httpClientFactory;
		private readonly IMinMQEnvironmentVariables minMQEnvironmentVariables;
		private readonly Duration showProgressEvery = Duration.FromMilliseconds(400);
		private CancellationToken cancellationToken;

		public Benchmarker
		(
			IHttpClientFactory httpClientFactory,
			IMinMQEnvironmentVariables minMQEnvironmentVariables
		)
		{
			this.httpClientFactory = httpClientFactory;
			this.minMQEnvironmentVariables = minMQEnvironmentVariables;
		}

		public event OnCompleteDelegate OnComplete;

		public async Task Start(CancellationToken cancellationToken)
		{
			this.cancellationToken = cancellationToken;
			int jsonCount, xmlCount;
			(List<string> jsons, List<string> xmls) = TimedFunction(() => GenerateObjects(minMQEnvironmentVariables.NumberOfObjects, out jsons, out xmls));
			jsonCount = jsons.Count;
			xmlCount = xmls.Count;

			Log.Information("Sending JSON and XML..");
			Instant start = SystemClock.Instance.GetCurrentInstant();

			// Old-school non-blocking
			await PostSendAsStringContent(jsons);
			await PostSendAsStringContent(xmls);
			Duration duration = SystemClock.Instance.GetCurrentInstant() - start;
			decimal throughtput = (jsonCount + xmlCount) / (decimal)duration.TotalSeconds;
			Log.Information("Done! {0:N2} documents/s (Xmls: {1}, Jsons={2}))", throughtput, xmlCount, jsonCount);
			OnComplete?.Invoke();
		}

		private (List<string>, List<string>) GenerateObjects(int numberOfObjects, out List<string> jsons, out List<string> xmls)
		{
			Instant lastShowProgress = SystemClock.Instance.GetCurrentInstant();
			jsons = new List<string>();
			xmls = new List<string>();
			var jsonGenerator = new JsonGenerator(minMQEnvironmentVariables.NTree);
			var xmlGenerator = new XmlGenerator(minMQEnvironmentVariables.NTree);

			Log.Information("Preparing payload");
			for (int i = 0; i < numberOfObjects; i++)
			{
				if (cancellationToken.IsCancellationRequested) return (new List<string>(), new List<string>());

				Instant now = SystemClock.Instance.GetCurrentInstant();
				if (now - lastShowProgress > showProgressEvery)
				{
					lastShowProgress = now;
					Log.Information("{0} %", Math.Floor((decimal)i * 100 / numberOfObjects));
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
			Log.Information("Done! {0:N2} documents/s", perf);
			return objects;
		}

		private async Task TimedFunction(Func<Task<int>> action, string name)
		{
			Log.Information("Sending XML..");
			Instant start = SystemClock.Instance.GetCurrentInstant();
			var count = await action();
			Duration duration = SystemClock.Instance.GetCurrentInstant() - start;
			Log.Information("Done! {0:N2} documents/s", count / (decimal)duration.TotalSeconds);
		}

		private async Task PostSendAsStringContent(List<string> documents)
		{
			List<Task> tasks = new List<Task>();

			for (int j = 0;  j < documents.Count; j++)
			{
				if (cancellationToken.IsCancellationRequested) return;

				HttpClient httpClient = httpClientFactory.CreateClient();
				StringContent content = new StringContent(documents[j]);
				tasks.Add(httpClient.PostAsync(minMQEnvironmentVariables.RequestPath, content)); // It seems a CancellationToken here will fail the service.

				if (j % ConcurrentHttpRequests == 0 && j > 0)
				{
					await Task.WhenAll(tasks);
					tasks.Clear();
				}
			}

			// Don't miss tail documents.
			if (tasks.Count > 0)
			{
				await Task.WhenAll(tasks);
				tasks.Clear();
			}
		}
	}
}
