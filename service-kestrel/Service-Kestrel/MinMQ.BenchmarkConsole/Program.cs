using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MinMQ.BenchmarkConsole
{
	class Program
	{
		private const int showProgressEvery = 30;
		private const int len = 1000;
		private const int ntree = 5;  // n = 2 == binary tree

		static void Main(string[] args)
		{
			var jsons = new List<string>();
			var xmls = new List<string>();
			var jsonGenerator = new JsonGenerator(ntree);
			var xmlGenerator = new XmlGenerator(ntree);

			Console.WriteLine("Preparing payload");

			for (int i = 0; i < len; i++)
			{
				if (i > 0 && i % showProgressEvery == 0)
				{
					Console.WriteLine($"{Math.Floor((decimal)i * 100 / len)}%");
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
