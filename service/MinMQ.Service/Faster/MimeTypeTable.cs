using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinMq.Service.Faster
{
	public class MimeTypeTable
	{
		private readonly Dictionary<byte, string> lookup;
		public MimeTypeTable()
		{
			lookup = new Dictionary<byte, string>();
			lookup.Add(0, "");
		}
	}
}
