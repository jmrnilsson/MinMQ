namespace MinMQ.Service.Controllers.Dto
{
	public class PeekResponseDto
	{
		public long CurrentAddress { get; set; }
		public long NextAddress { get; set; }
		public string Content { get; set; }
	}
}
