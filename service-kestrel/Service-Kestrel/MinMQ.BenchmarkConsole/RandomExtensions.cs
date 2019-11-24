using System;
using System.Collections.Generic;

namespace MinMQ.BenchmarkConsole
{
	public static class RandomExtensions
	{
		/// <summary>
		/// Python-inspired random choice
		/// </summary>
		/// <param name="randon"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static T Choice<T>(this Random random, params T[] options)
		{
			var choice = random.Next(1, options.Length) - 1;
			return options[choice];
		}

		/// <summary>
		/// Python-inspired random choice for enums
		/// </summary>
		/// <param name="randon"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static T Choice<T>(this Random random) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum) throw new ArgumentException("Type must be an enumerated type");

			List<T> options = new List<T>();

			foreach (T option in (T[])Enum.GetValues(typeof(T)))
			{
				options.Add(option);
			}

			var choice = random.Next(1, options.Count) - 1;
			return options[choice];
		}

		public static bool OneIn(this Random random, int count)
		{
			return random.Next(0, count) < 1;
		}
	}
}
