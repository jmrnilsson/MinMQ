using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MinMQ.BenchmarkConsole.Tests
{
	public class XmlGeneratorTests
	{
		const string guidPattern = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";
		XmlGenerator jsonGenerator = new XmlGenerator(5);

		[Fact]
		public void XmlGenerator_should_contain_guid()
		{
			string xml = jsonGenerator.GenerateObject();

			xml.ShouldMatch(guidPattern);
		}

		[Fact]
		public void XmlGenerator_should_not_merge_if_unioned()
		{
			var xmls = new List<string>();

			for (int i = 0; i < 500; i++)
			{
				xmls.Add(jsonGenerator.GenerateObject());
			}

			xmls.Take(250).Union(xmls.Skip(250)).Count().ShouldBe(500);
		}
	}
}
