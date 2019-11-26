using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MinMQ.BenchmarkConsole
{
	public static class Compute
	{
		private const int taskSize = 50;
		private static SemaphoreSlim semaphore = new SemaphoreSlim(116, 116);
		public static async Task<(List<string> Jsons, List<string> Xmls)> ComputeObjects(int ntree, int count)
		{
			var tasks = new List<Task<(List<string> Json, List<string> Xml)>>();
			int iter = count / taskSize;
			Console.WriteLine("Iter={0}", iter);
			int modulus = count % taskSize;

			for (int i = 0; i < iter; i++)
			{
				await semaphore.WaitAsync();
				tasks.Add(ComputeTask(taskSize, ntree, taskSize));
				semaphore.Release();
			}

			if (modulus > 0)
			{
				tasks.Add(ComputeTask(taskSize, ntree, modulus));
			}

			List<string> jsons = new List<string>();
			List<string> xmls = new List<string>();
			var results = await Task.WhenAll(tasks);

			foreach ((List<string> json, List<string> xml) in results)
			{
				jsons.AddRange(json);
				xmls.AddRange(json);
			}

			return (jsons, xmls);
		}

		private static async Task<(List<string> Json, List<string> Xml)> ComputeTask(int taskSize, int ntree, int sizeOrMod)
		{
			var jsonGenerator = new JsonGenerator(ntree);
			var xmlGenerator = new XmlGenerator(ntree);
			return await GenerateObjects(xmlGenerator, jsonGenerator, sizeOrMod);
		}

		public static async Task<(List<string> Json, List<string> Xml)> GenerateObjects(XmlGenerator xmlGenerator, JsonGenerator jsonGenerator, int taskSize)
		{
			var xmls = new List<string>();
			var jsons = new List<string>();
			for (int i = 0; i < taskSize; i++)
			{
				jsons.Add(jsonGenerator.GenerateObject());
				xmls.Add(xmlGenerator.GenerateObject());
			}

			return await Task.FromResult((jsons, xmls));
		}
	}
}
