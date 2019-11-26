using System.Collections.Generic;

namespace MinMQ.Service.Controllers.Dto
{
	public class ListResponseDto
	{
		public long FirstAddress { get; set; }
		public long LastAddress { get; set; }
		public long NextAddress { get; set; }
		public List<long> Addresses { get; set; }
		public List<string> Contents { get; set; }
	}
}
