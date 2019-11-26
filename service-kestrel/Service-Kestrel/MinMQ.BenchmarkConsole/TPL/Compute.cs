using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MinMQ.BenchmarkConsole
{
	public static class Compute
	{
		private const int taskSize = 30;
		public static async Task<(List<string> Jsons, List<string> Xmls)> ComputeObjects(int ntree, int count)
		{
			//var jsonGenerator = new JsonGenerator(ntree);
			//var xmlGenerator = new XmlGenerator(ntree);
			int iter = count / taskSize;
			Console.WriteLine("Iter={0}", iter);
			int modulus = count % taskSize;
			var tasks = new List<Task<(List<string> Json, List<string> Xml)>>();
			for (int i = 0; i < iter; i++)
			{
				var task = Task.Factory.StartNew(() =>
				{
					var jsonGenerator = new JsonGenerator(ntree);
					var xmlGenerator = new XmlGenerator(ntree);
					return GenerateObjects(xmlGenerator, jsonGenerator, taskSize);
				}, TaskCreationOptions.LongRunning);
				tasks.Add(task.Unwrap());
			}

			if (modulus > 0)
			{
				var task = Task.Factory.StartNew(() =>
				{
					var jsonGenerator = new JsonGenerator(ntree);
					var xmlGenerator = new XmlGenerator(ntree);
					return GenerateObjects(xmlGenerator, jsonGenerator, modulus);
				}, TaskCreationOptions.LongRunning);
				tasks.Add(task.Unwrap());
			}

			List<string> jsons = new List<string>();
			List<string> xmls = new List<string>();
			// (List<string> Json, List<string> Xml)[] results = await Task.WhenAll(tasks);
			var results = await Task.WhenAll(tasks);

			foreach ((List<string> json, List<string> xml) in results)
			{
				jsons.AddRange(json);
				xmls.AddRange(json);
			}

			return (jsons, xmls);
		}

		public static Task<(List<string> Json, List<string> Xml)> GenerateObjects(XmlGenerator xmlGenerator, JsonGenerator jsonGenerator, int taskSize)
		{
			var xmls = new List<string>();
			var jsons = new List<string>();
			for (int i = 0; i < taskSize; i++)
			{
				jsons.Add(jsonGenerator.GenerateObject());
				xmls.Add(xmlGenerator.GenerateObject());
			}

			return Task.FromResult((jsons, xmls));
		}
	}
}
