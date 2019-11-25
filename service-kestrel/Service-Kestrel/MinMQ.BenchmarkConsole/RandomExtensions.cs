using System;
using System.Collections.Generic;

namespace MinMQ.BenchmarkConsole
{
	public static class RandomExtensions
	{
		/// <summary>
		/// Python inspired random choice
		/// </summary>
		/// <typeparam name="T">Object or value</typeparam>
		/// <param name="random">Random</param>
		/// <param name="options">Options</param>
		/// <returns>Returns a random choice out of several options</returns>
		public static T Choice<T>(this Random random, params T[] options)
		{
			var choice = random.Next(1, options.Length) - 1;
			return options[choice];
		}

		/// <summary>
		/// Python inspired random choice for enums
		/// </summary>
		/// <typeparam name="T">enum</typeparam>
		/// <param name="random">Random</param>
		/// <returns>Returns one enum</returns>
		public static T Choice<T>(this Random random)
		    where T : struct, IConvertible
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
