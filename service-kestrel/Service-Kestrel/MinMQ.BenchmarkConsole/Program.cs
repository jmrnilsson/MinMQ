using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MinMQ.BenchmarkConsole
{
	public interface IDocumentGenerator<T> where T : class
	{
		T GenerateObject(T child, bool allowCollisions);
	}

	class Program
	{
		private const int showProgressEvery = 30;
		private const int len = 1000;

		static void Main(string[] args)
		{
			var jsons = new List<string>();
			var xmls = new List<string>();
			var seed = new Random();
			var jsonGenerator = new JsonGenerator(500);
			var xmlGenerator = new XmlGenerator(1000);

			Console.WriteLine("Preparing payload");

			for (int i = 0; i < len; i++)
			{
				if (i > 0 && i % showProgressEvery == 0)
				{
					Console.WriteLine($"{Math.Floor((decimal)i * 100 / len)}%");
				}

				jsons.Add(jsonGenerator.Generate());
				xmls.Add(xmlGenerator.Generate());
			}

			Console.WriteLine("-----------------XML-----------------");
			Console.WriteLine();
			Console.WriteLine(xmls[0]);
			Console.WriteLine();
			Console.WriteLine("-----------------JSON----------------");
			Console.WriteLine();
			Console.WriteLine(jsons[0]);
			Console.WriteLine();
		}


	}

}
