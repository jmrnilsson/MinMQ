using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace MinMQ.BenchmarkConsole
{
	public class JsonGenerator // : IDocumentGenerator<JToken>
	{
		private static Random seed = new Random();
		private readonly int maxNoOfProps;
		private const int minNoOfProps = 10;
		public JsonGenerator(int maxNoOfProps)
		{
			this.maxNoOfProps = Math.Max(minNoOfProps + 1, maxNoOfProps);
		}

		/// <summary>
		/// Poorly designed JSON generator.
		/// </summary>
		/// <returns></returns>
		public string Generate()
		{
			Func<string> wordFactory = Words.Pick;
			int propCount = seed.Next(minNoOfProps, maxNoOfProps);

			JObject child = new JObject(new JProperty(wordFactory(), new JValue(wordFactory())));  // Bottom child
			int i = 0;
			while (i < propCount)
			{
				if (i < 1 || seed.Next(0, 2) < 1)
				{
					// Set parent and assign child to a property
					child = new JObject(GenerateObject(true, child));
				}
				else
				{
					foreach ((string c, JToken a) in child)
					{
						try
						{
							// Attach sibling
							child.Property(c).AddAfterSelf(GenerateObject(true));
						}
						catch (ArgumentException)
						{
							// Retry with GUID as property name since property may already be in use.
							child.Property(c).AddAfterSelf(GenerateObject(false));
						}
						break;
					}
				}
				i++;
			}

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

		public JToken GenerateObject(bool allowCollisions, JToken child = null)
		{
			JToken value = GenerateJValue(seed);

			if (allowCollisions)
			{
				return new JProperty(Words.Pick(), child ?? value);
			}

			return new JProperty(Guid.NewGuid().ToString(), child ?? value);
		}
	}
}
