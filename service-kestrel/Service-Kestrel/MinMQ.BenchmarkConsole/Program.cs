using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MinMQ.BenchmarkConsole
{
	public class Program
	{
		private const int ShowProgressEvery = 30;
		private const int TotalNumberOfObject = 1000;
		private const int NTree = 5;  // NTree = 2 == binary tree

		public static void Main(string[] args)
		{
			var jsons = new List<string>();
			var xmls = new List<string>();
			var jsonGenerator = new JsonGenerator(NTree);
			var xmlGenerator = new XmlGenerator(NTree);

			Console.WriteLine("Preparing payload");

			for (int i = 0; i < TotalNumberOfObject; i++)
			{
				if (i > 0 && i % ShowProgressEvery == 0)
				{
					Console.WriteLine($"{Math.Floor((decimal)i * 100 / TotalNumberOfObject)}%");
				}

				jsons.Add(jsonGenerator.GenerateObject());
				xmls.Add(xmlGenerator.GenerateObject());
			}

			Console.WriteLine("-----------------JSON----------------");
			Console.WriteLine();
			Console.WriteLine(jsons[0]);
			Console.WriteLine();
			Console.WriteLine("-----------------XML-----------------");
			Console.WriteLine();
			Console.WriteLine(xmls[0]);
			Console.WriteLine();
		}
	}
}
