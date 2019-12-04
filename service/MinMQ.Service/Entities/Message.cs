namespace MinMq.Service.Entities
{
	public class Message
	{
		public Message(string content, long referenceId, long nextReferenceId)
		{
			Content = content;
			ReferenceId = referenceId;
			NextReferenceId = nextReferenceId;
			MimeTypeByteKey = 0x01;
			QueueId = 0x00;
			HashCode = content.ToFnv1aHashInt64();
		}

		public Message(string content, long referenceId, long nextReferenceId, string hashCode, short mimeTypeByteKey, short queueByteKey)
		{
			Content = content;
			ReferenceId = referenceId;
			NextReferenceId = nextReferenceId;
			MimeTypeByteKey = mimeTypeByteKey;
			QueueId = queueByteKey;
			HashCode = hashCode;
		}

		public string Content { get; }
		public long ReferenceId { get; }
		public long NextReferenceId { get; }
		public short MimeTypeByteKey { get; }
		public short QueueId { get; }
		public string HashCode { get; set; }
	}
}
