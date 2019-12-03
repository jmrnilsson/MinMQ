namespace MinMq.Service.Repository
{
	public interface IMessage
	{
		public long ReferenceId { get; }
		public string HashCode { get; }
	}
}
