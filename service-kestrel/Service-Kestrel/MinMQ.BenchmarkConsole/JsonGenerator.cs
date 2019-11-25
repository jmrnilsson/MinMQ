using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMQ.BenchmarkConsole
{
	public class JsonGenerator : GeneratorBase<JObject>
	{
		public JsonGenerator(int n) : base(n) { }

		/// <summary>
		/// Poorly designed JSON generator.
		/// </summary>
		/// <returns></returns>
		public override string GenerateObject()
		{
			JObject child = new JObject(new JProperty(Words.Pick(), new JValue(Words.Pick())));
			int depth = 0;
			IEnumerable<JObject> children = GenerateChildren(++depth);
			AddChildren(children, child);

			return child.ToString();
		}

		private static JToken GenerateJValue(Random seed)
		{
			switch (seed.Next(0, 5))
			{
				case 0:
					var values = Enumerable.Range(0, seed.Next(10)).Select(_ => new JValue(seed.Next(0, 999)));
					return new JArray(values);
				case 1: return new JValue(DateTime.Now);
				case 2: return new JValue(seed.Next(0, 10_000));
				case 3: return new JValue(seed.NextDouble());
				case 4: return new JValue(seed.Next(0, 10_000));
				default: return null;
			}
		}

		protected override JObject GenerateChild(IEnumerable<JObject> innerChildren)
		{
			JToken value = GenerateJValue(seed);
			JProperty prop = new JProperty(Words.Pick(), value);
			JObject child = new JObject(prop);

			AddChildren(innerChildren, child);

			return child;
		}

		private static void AddChildren(IEnumerable<JObject> innerChildren, JObject child)
		{
			foreach (JObject innerChild in innerChildren)
			{
				try
				{
					child.Add(Words.Pick(), innerChild);
				}
				catch (ArgumentException)
				{
					child.Add(Guid.NewGuid().ToString(), innerChild);
				}
			}
		}
	}
}
