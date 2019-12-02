using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinMq.Service
{
	// TODO: Keep for now
	public class AsyncEnumerable
	{
		private async Task<List<TResult>> ToListAsync<TResult, T>(IAsyncEnumerable<T> scan, Func<T, TResult> valueFactory)
		{
			List<TResult> result = new List<TResult>();

			await foreach (T value in scan)
			{
				result.Add(valueFactory(value));
			}

			return result;
		}

		private async Task<Dictionary<TKey, T>> ToDictionaryAsync<TKey, T>(IAsyncEnumerable<T> scan, Func<T, TKey> keyFactory)
		{
			Dictionary<TKey, T> result = new Dictionary<TKey, T>();

			await foreach (T value in scan)
			{
				result[keyFactory(value)] = value;
			}

			return result;
		}
	}
}
