using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MinMQ.BenchmarkConsole
{
	public static class Compute
	{
		private const int ThreadCount = 8;
		// private static SemaphoreSlim semaphore = new SemaphoreSlim(116, 116);
		public static (List<string> Jsons, List<string> Xmls) ComputeObjects(int ntree, int count)
		{
			var tasks = new List<Task<(List<string> Json, List<string> Xml)>>();
			int iter = count / ThreadCount;
			Console.WriteLine("Iter={0}", iter);
			int modulus = count % ThreadCount;

			List<Thread> workerThreads = new List<Thread>();
			var results = new List<(List<string> Json, List<string> Xml)>();

			for (int i = 0; i < ThreadCount; i++)
			{
				Thread thread = new Thread(() =>
				{
					lock (results)
					{
						results.Add(ComputeTask(ntree, iter));
					}
				});

				workerThreads.Add(thread);
				thread.Start();
			}

			if (modulus > 0)
			{
				Thread thread = new Thread(() =>
				{
					lock (results)
					{
						results.Add(ComputeTask(ntree, modulus));
					}
				});

				workerThreads.Add(thread);
				thread.Start();
			}

			// Wait for all the threads to finish so that the results list is populated.
			// If a thread is already finished when Join is called, Join will return immediately.
			foreach (Thread thread in workerThreads)
			{
				thread.Join();
			}

			//for (int i = 0; i < iter; i++)
			//{
			//	await semaphore.WaitAsync();
			//	tasks.Add(ComputeTask(taskSize, ntree, taskSize));
			//	semaphore.Release();
			//}

			List<string> jsons = new List<string>();
			List<string> xmls = new List<string>();

			foreach ((List<string> json, List<string> xml) in results)
			{
				jsons.AddRange(json);
				xmls.AddRange(json);
			}

			return (jsons, xmls);
		}

		private static (List<string> Json, List<string> Xml) ComputeTask(int ntree, int sizeOrMod)
		{
			var jsonGenerator = new JsonGenerator(ntree);
			var xmlGenerator = new XmlGenerator(ntree);
			return GenerateObjects(xmlGenerator, jsonGenerator, sizeOrMod);
		}

		public static (List<string> Json, List<string> Xml) GenerateObjects(XmlGenerator xmlGenerator, JsonGenerator jsonGenerator, int taskSize)
		{
			var xmls = new List<string>();
			var jsons = new List<string>();

			for (int i = 0; i < taskSize; i++)
			{
				jsons.Add(jsonGenerator.GenerateObject());
				xmls.Add(xmlGenerator.GenerateObject());
			}

			return (jsons, xmls);
		}
	}
}
