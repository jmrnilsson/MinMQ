using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace MinMQ.BenchmarkConsole
{

	public class XmlGenerator : GeneratorBase<XmlElement>
	{
		XmlDocument doc;
		public XmlGenerator(int n) : base(n) { }

		/// <summary>
		/// XML-generator. Combines variable depth with attributes and sets a text value at the lowest child.
		/// </summary>
		/// <returns></returns>
		public override string GenerateObject()
		{
			doc = new XmlDocument();
			doc.AppendChild(doc.CreateElement(Words.Pick()));
			var root = doc.DocumentElement;

			int depth = 0;
			var children = GenerateChildren(++depth);

			foreach(XmlElement child in children)
			{
				root.AppendChild(child);
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

		protected override XmlElement GenerateChild(IEnumerable<XmlElement> innerChildren)
		{
			Func<string> wordFactory = Words.Pick;
			int numberOfProps = NumberOfProperties();
			var child = doc.CreateElement(wordFactory());

			for(int i = 0; i < numberOfProps; i++)
			{
				child.SetAttribute(wordFactory(), wordFactory());
			}

			foreach(XmlElement innerChild in innerChildren)
			{
				child.AppendChild(innerChild);
			}

			if (child.ChildNodes.Count < 1)
			{
				child.InnerText = $"{wordFactory()} {wordFactory()} {wordFactory()} {wordFactory()}";
			}

			return child;

		}
	}
}
