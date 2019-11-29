using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinMq.Service
{
	public static class FNV1a
	{
		public static string ToFnv1aHashInt64(this string text)
		{
			string Fnv1a(byte[] bytes_)
			{
				const uint offset = 0x811C9DC5;
				const uint prime = 0x01000193;
				uint hash = offset;

				for (var i = 0; i < bytes_.Length; i++)
				{
					unchecked
					{
						hash ^= bytes_[i];
						hash *= prime;
					}
				}

				// return BitConverter.ToInt64(bytes_, 0);
				return Convert.ToBase64String(BitConverter.GetBytes(hash));
			}

			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return Fnv1a(bytes);
		}
	}
}
