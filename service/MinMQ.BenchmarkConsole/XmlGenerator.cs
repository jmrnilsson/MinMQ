using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace MinMQ.BenchmarkConsole
{
	public class XmlGenerator : GeneratorBase<XmlElement>
	{
		private XmlDocument doc;
		private Words words = new Words();

		public XmlGenerator(int n)
            : base(n)
        {
        }

		/// <summary>
		/// XML-generator. Combines variable depth with attributes and sets a text value at the lowest child.
		/// </summary>
		/// <returns>XML with random attributes and elements</returns>
		public override string GenerateObject()
		{
			doc = new XmlDocument();
			var element = doc.CreateElement(words.Pick());
			element.SetAttribute("id", Guid.NewGuid().ToString());
			doc.AppendChild(element);
			var root = doc.DocumentElement;

			int depth = 0;
			var children = GenerateChildren(++depth);

			foreach (XmlElement child in children)
			{
				root.AppendChild(child);
			}

			return PrintXml(doc.InnerXml);
		}

		protected override XmlElement GenerateChild(IEnumerable<XmlElement> innerChildren)
		{
			Func<string> wordFactory = () => words.Pick();
			int numberOfProps = NumberOfProperties();
			var child = doc.CreateElement(wordFactory());

			for (int i = 0; i < numberOfProps; i++)
			{
				child.SetAttribute(wordFactory(), wordFactory());
			}

			foreach (XmlElement innerChild in innerChildren)
			{
				child.AppendChild(innerChild);
			}

			if (child.ChildNodes.Count < 1)
			{
				child.InnerText = $"{wordFactory()} {wordFactory()} {wordFactory()} {wordFactory()}";
			}

			return child;
		}

		public static string PrintXml(string xml)
		{
			string formattedXml = "";

			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(memoryStream, Encoding.Unicode);
			XmlDocument document = new XmlDocument();
			StreamReader reader = new StreamReader(memoryStream);

			try
			{
				document.LoadXml(xml);
				writer.Formatting = Formatting.Indented;
				document.WriteContentTo(writer);
				writer.Flush();
				memoryStream.Flush();
				memoryStream.Position = 0;
				formattedXml = reader.ReadToEnd();
			}
			finally
			{
				reader.Close();
				memoryStream.Close();
				writer.Close();
			}

			return formattedXml;
		}
	}
}
