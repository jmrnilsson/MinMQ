using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace MinMQ.BenchmarkConsole
{
    class Program
    {
		private static string[] words = GetWords().ToArray();
		private const int showProgressEvery = 500;
		private const int len = 7_500;
		private static int wordIndex = 0;

		static void Main(string[] args)
		{
			var jsons = new List<string>();
			var xmls = new List<string>();
			var seed = new Random();

			Console.WriteLine("Preparing payload");

			for(int i = 0; i < len; i++)
			{
				if (i > 0 && i % showProgressEvery == 0)
				{
					Console.WriteLine($"{Math.Floor((decimal) i * 100 / len)}%");
				}

				jsons.Add(GenerateJson(seed));
				xmls.Add(GenerateXml(seed));
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

		/// <summary>
		/// XML-generator. Combines variable depth with attributes and sets a text value at the lowest child.
		/// </summary>
		/// <param name="seed"></param>
		/// <returns></returns>
		private static string GenerateXml(Random seed)
		{
			int propCount = seed.Next(10, 1000);
			
			XmlDocument doc = new XmlDocument();
			doc.AppendChild(doc.CreateElement(Pick(words, seed)));
			var root = doc.DocumentElement;

			int i = 0;
			int depth = 0;
			while (i < propCount)
			{
				depth++;
				var child = doc.CreateElement(Pick(words, seed));
				root.InsertAfter(child, root.FirstChild);
				i++;

				while (depth < 20)
				{
					var appendChild = seed.Next(0, 3);
					var orMakeAttribute = seed.Next(0, 2);

					if (appendChild < 1)
					{
						break;
					}

					depth++;
					i++;

					if (orMakeAttribute < 1)
					{
						child.SetAttribute(Pick(words, seed), Pick(words, seed));
						continue;
					}
					var nextChild = doc.CreateElement(Pick(words, seed));
					child.AppendChild(nextChild);
					child = nextChild;
				}

				child.InnerText = $"{Pick(words, seed)} {Pick(words, seed)} {Pick(words, seed)} {Pick(words, seed)}";
				depth = 0;
			}

			return PrintXml(doc.InnerXml);
		}


		/// <summary>
		/// Poorly designed JSON generator. Depth is usually zero. Though lenght is variable.
		/// </summary>
		/// <param name="seed"></param>
		/// <returns></returns>
		private static string GenerateJson(Random seed)
		{
			int propCount = seed.Next(10, 20);

			JObject doc = new JObject();
			var child = new JProperty(Pick(words, seed), new JValue(Pick(words, seed)));  // Bottom child
			// var topMost = child;
			int i = 0;
			while(i < propCount)
			{
				if (i < 1 || seed.Next(0, 2) < 1)
				{
					string prop_ = Pick(words, seed);
					child = new JProperty(prop_, new JObject(child));
				}
				else 
				{
					string prop = Pick(words, seed);
					string value = Pick(words, seed);

					foreach(JToken c in child)
					{
						// c.
						//c
						//c.AddAfterSelf(new JProperty(prop, new JValue(value)));
						child.AddAfterSelf(c, new JProperty(prop, new JValue(value)));
						c.AddAfterSelf(new JProperty(prop));
						break;
					}
					//var token = child.Children()[0] as JObject;
					//token.AddAfterSelf(new JProperty(prop, new JValue(value)));
				}
				i++;
				// ((JObject)()).AddAfterSelf();
				// child.AddAfterSelf(new JProperty(Pick(words, seed), new JValue(Pick(words, seed))));
				// child = new JProperty(Pick(words, seed), child, new JProperty(Pick(words, seed), new JValue(Pick(words, seed))));
			}

			//int i = 0;
			//int depth = 0;
			//HashSet<string> used = new HashSet<string>();

			//while (i < propCount)
			//{
			//	depth++;
			//	string nextProp = Pick(words, seed);

			//	if (used.Contains(nextProp))
			//	{
			//		nextProp = Guid.NewGuid().ToString();
			//	}
			//	else
			//	{
			//		used.Add(nextProp);
			//	}

			//	var child = new JProperty(nextProp, new JValue(Pick(words, seed)));
			//	doc.Add(child);
			//	i++;
			//}

			doc.Add(child);

			return doc.ToString();
		}

		private static IEnumerable<string> GetWords()
		{
			var phrase = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris eleifend leo ut velit pellentesque
				dictum. In venenatis ipsum non lacinia luctus. Aliquam pretium mauris nec quam imperdiet, in euismod ligula
				mollis. Integer risus diam, facilisis at tellus eget, fringilla hendrerit orci. Pellentesque at ornare diam.
				Quisque semper gravida erat. Sed feugiat felis vel massa consectetur suscipit nec bibendum ligula. Nullam
				pulvinar metus quis porta ornare. Pellentesque sodales nisi sit amet rutrum faucibus. Vestibulum pulvinar
				rhoncus purus, ut bibendum ante pretium in. Suspendisse potenti.

				Vestibulum facilisis ullamcorper lacus id iaculis. Nunc laoreet odio non nisi vulputate fringilla. Etiam erat
				nisi, viverra ut aliquam ac, dignissim eget felis. Integer sit amet augue suscipit, dignissim turpis semper,
				consectetur leo. Phasellus venenatis, libero ac pulvinar facilisis, elit augue ullamcorper ante, quis egestas mi
				justo ut dolor. Fusce nec diam nec est pulvinar faucibus. Vestibulum ante ipsum primis in faucibus orci luctus
				et ultrices posuere cubilia Curae; Morbi sit amet condimentum mi. Pellentesque habitant morbi tristique senectus
				et netus et malesuada fames ac turpis egestas.

				In rutrum ultrices sapien et venenatis. Nulla ornare euismod lectus ac facilisis. Curabitur imperdiet dignissim
				massa quis congue. Ut varius, dui et ornare finibus, lacus ligula porttitor ex, vel fringilla nisl risus et
				odio. Morbi ultricies volutpat mauris non bibendum. Aliquam at arcu sed massa venenatis sodales vitae nec ipsum.
				Suspendisse euismod lobortis massa eu molestie. Vivamus quis erat quis erat sollicitudin tristique. Praesent
				ornare consequat ipsum vel porta. Vivamus dignissim at diam sed faucibus.

				Duis nisi purus, lacinia eget magna vel, cursus ultricies erat. Suspendisse id sapien ullamcorper, maximus nisi
				ut, sodales dolor. Aenean consequat, est vitae venenatis rutrum, est massa dapibus magna, ac faucibus mauris
				augue eget purus. Suspendisse vel eros sapien. Curabitur vel sem eu sapien suscipit commodo. Vivamus vulputate
				nisl et ligula aliquet, nec dapibus lacus hendrerit. Etiam lobortis ornare nulla rutrum dignissim. Nulla
				sollicitudin non ipsum id ullamcorper. Nulla facilisi.

				Cras neque diam, dapibus eu felis sit amet, sollicitudin pellentesque diam. Ut et tincidunt dolor, vel varius
				urus. Etiam ut nibh sit amet tellus vestibulum pulvinar. Phasellus ante felis, venenatis interdum tempor eu,
				ornare vitae mi. Duis porttitor ipsum ac sapien dictum, in scelerisque lacus imperdiet. Nullam venenatis
				pellentesque elit a volutpat. Sed hendrerit tristique felis nec sagittis. Quisque in diam vulputate, porta purus
				nec, viverra mauris. Morbi accumsan consectetur lectus, eget semper sapien sagittis in. Donec quam lacus,
				consequat semper nunc eu, suscipit scelerisque magna. Curabitur porta arcu nec iaculis convallis. Duis mattis
				tempor mi tristique commodo. Pellentesque nec consequat dolor. ";

			foreach (Match match in Regex.Matches(phrase, "\\w+"))
			{
				yield return match.Value;
			}
		}

		public static string Pick(string[] words, Random seed)
		{
			if (++wordIndex > 0 && wordIndex % words.Length == 0)
			{
				wordIndex = 0;
			}

			return words[wordIndex];
		}

		public static string PrintXml(string xml)
		{
			string formattedXml = "";

			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(memoryStream, Encoding.Unicode);
			XmlDocument document = new XmlDocument();

			try
			{
				document.LoadXml(xml);
				writer.Formatting = Formatting.Indented;
				document.WriteContentTo(writer);
				writer.Flush();
				memoryStream.Flush();
				memoryStream.Position = 0;
				StreamReader sReader = new StreamReader(memoryStream);
				formattedXml = sReader.ReadToEnd();
			}
			finally
			{
				memoryStream.Close();
				writer.Close();
			}

			return formattedXml;
		}

	}

}
