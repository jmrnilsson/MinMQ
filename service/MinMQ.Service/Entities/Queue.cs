namespace MinMq.Service.Entities
{
	public class Queue
	{
		public Queue(short byteKey, string name)
		{
			QueueId = byteKey;
			Name = name;
		}

		public Queue(string name)
		{
			QueueId = null;
			Name = name;
		}

		public short? QueueId { get; }
		public string Name { get; set; }
	}
}
