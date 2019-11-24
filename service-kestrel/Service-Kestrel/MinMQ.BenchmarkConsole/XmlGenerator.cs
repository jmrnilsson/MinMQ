using System;
using System.IO;
using System.Text;
using System.Xml;

namespace MinMQ.BenchmarkConsole
{

	public class XmlGenerator : IDocumentGenerator<XmlElement>
	{
		private static Random seed = new Random();
		private readonly int maxNumberOfProps;
		private const int minNoOfProps = 10;

		public XmlGenerator(int maxNumberOfProps)
		{
			this.maxNumberOfProps = Math.Max(minNoOfProps + 1, maxNumberOfProps);
		}

		/// <summary>
		/// XML-generator. Combines variable depth with attributes and sets a text value at the lowest child.
		/// </summary>
		/// <param name="seed"></param>
		/// <returns></returns>
		public string Generate()
		{
			Func<string> wordFactory = Words.Pick;
			int propCount = seed.Next(minNoOfProps, maxNumberOfProps);

			XmlDocument doc = new XmlDocument();
			doc.AppendChild(doc.CreateElement(wordFactory()));
			var root = doc.DocumentElement;

			int i = 0;
			int depth = 0;
			while (i < propCount)
			{
				// Every prop-count, 
				depth++;
				var child = doc.CreateElement(wordFactory());
				root.InsertAfter(child, root.FirstChild);
				i++;

				while (depth < 20)
				{
					// Per prop-count and 20 times
					// - Append another child or set attr
					if (seed.OneIn(5)) break;

					bool orSetAttr = seed.OneIn(2);

					//if (@break)
					//{
					//	// depth++;
					//	break;
					//	// continue;
					//}


					if (orSetAttr)
					{
						child.SetAttribute(wordFactory(), wordFactory());
						continue;
					}

					var nextChild = doc.CreateElement(wordFactory());
					child.AppendChild(nextChild);
					child = nextChild;
				}
				i++;

				child.InnerText = $"{wordFactory()} {wordFactory()} {wordFactory()} {wordFactory()}";
				depth = 0;
			}

			return PrintXml(doc.InnerXml);
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

		public XmlElement GenerateObject(XmlElement child, bool allowCollisions)
		{
			throw new NotImplementedException();
		}
	}
}
