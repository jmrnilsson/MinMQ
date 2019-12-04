namespace MinMq.Service.Entities
{
	public class Queue
	{
		public Queue(short byteKey, string name)
		{
			ByteKey = byteKey;
			Name = name;
		}

		public short ByteKey { get; }
		public string Name { get; set; }
	}
}
