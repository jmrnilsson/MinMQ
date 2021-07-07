using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MinMQ.BenchmarkConsole.Tests
{
	public class JsonGeneratorTests
	{
		const string guidPattern = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";
		JsonGenerator jsonGenerator = new JsonGenerator(5);

		[Fact]
		public void JsonGenerator_should_contain_guid()
		{
			string json = jsonGenerator.GenerateObject();

			json.ShouldMatch(guidPattern);
		}

		[Fact]
		public void Generated_objects_should_be_unique()
		{
			var jsons = new List<string>();

			for (int i = 0; i < 500; i++)
			{
				jsons.Add(jsonGenerator.GenerateObject());
			}

			jsons.Take(250).Union(jsons.Skip(250)).Count().ShouldBe(500);
		}
	}
}
