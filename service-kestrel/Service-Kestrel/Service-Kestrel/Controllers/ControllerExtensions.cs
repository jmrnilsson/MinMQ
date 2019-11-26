using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinMQ.Service.Controllers.Dto;
using Optional;

namespace MinMQ.Service.Controllers
{
	public delegate IAsyncEnumerable<(string, long, long)> ScanCollection();

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
		/// <param name="scan">Scanner items</param>
		/// <returns>Returns an optional list of items</returns>
		public static async Task<Option<ListResponseDto>> ToListResponse(this IAsyncEnumerable<(string, long, long)> scan)
		{
			long min = long.MaxValue;
			long max = long.MinValue;
			long next = long.MinValue;
			List<long> addresses = new List<long>();
			List<string> contents = new List<string>();

			await foreach (var item in scan)
			{
				var (content, currentAddress, nextAddress) = item;
				min = Math.Min(min, currentAddress);
				max = Math.Max(max, currentAddress);
				next = Math.Max(next, nextAddress);
				addresses.Add(currentAddress);
				contents.Add(content);
			}

			if (addresses.Count() < 1)
			{
				return Option.None<ListResponseDto>();
			}

			return new ListResponseDto
			{
				FirstAddress = min,
				LastAddress = max,
				NextAddress = next,
				Addresses = addresses,
				Contents = contents
			}.Some();
		}
	}
}
