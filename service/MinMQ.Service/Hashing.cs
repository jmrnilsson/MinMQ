using System;
using System.Text;

namespace MinMq.Service
{
	public static class Hashing
	{
		/// <summary>
		/// Fowler-Noll-Vo 1 a in 64 bit flavor as base 64
		/// </summary>
		/// <param name="text">Text to be hashed</param>
		/// <returns>A base 64 encoded string</returns>
		public static string ToFnv1aHashInt64(this string text)
		{
			string Fnv1a(byte[] bytes_)
			{
				const ulong offset = 14695981039346656037;
				const ulong prime = 1099511628211;
				ulong hash = offset;

				for (var i = 0; i < bytes_.Length; i++)
				{
					unchecked
					{
						hash ^= bytes_[i];
						hash *= prime;
					}
				}

				return Convert.ToBase64String(BitConverter.GetBytes(hash));
			}

			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return Fnv1a(bytes);
		}
	}
}
