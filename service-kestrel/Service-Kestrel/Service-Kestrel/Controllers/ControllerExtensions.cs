using System.Collections.Generic;
using System.Linq;
using MinMQ.Service.Controllers.Dto;

namespace MinMQ.Service.Controllers
{
	public static class ControllerExtensions
	{
		public static PeekResponseDto ToPeekResponse(this (string, long, long) peek)
		{
			var (content, currentAddress, nextAddress) = peek;
			return new PeekResponseDto
			{
				Content = content,
				CurrentAddress = currentAddress,
				NextAddress = nextAddress,
			};
		}

		/// <summary>
		/// Hopefully runtime optimizers will have a look at this..
		/// </summary>
		/// <param name="list">Unformatted list</param>
		/// <returns>Response of a message list</returns>
		public static ListResponseDto ToListResponse(this List<(string, long, long)> list)
		{
			long min = list.Min(e => e.Item2);
			long max = list.Max(e => e.Item2);
			long next = list.Max(e => e.Item3);
			long[] addresses = list.Select(e => e.Item2).ToArray();
			string[] contents = list.Select(e => e.Item1).ToArray();

			return new ListResponseDto
			{
				FirstAddress = min,
				LastAddress = max,
				NextAddress = next,
				Addresses = addresses,
				Contents = contents
			};
		}
	}
}
