namespace MinMQ.Service.Controllers.Dto
{
	public class ListResponseDto
	{
		public long FirstAddress { get; set; }
		public long LastAddress { get; set; }
		public long NextAddress { get; set; }
		public long[] Addresses { get; set; }
		public string[] Contents { get; set; }
	}
}
