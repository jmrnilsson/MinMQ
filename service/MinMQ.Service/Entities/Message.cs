namespace MinMq.Service.Entities
{
	public class Message
	{
		public Message(string content, long referenceId, long nextReferenceId, short mimeTypeId, short queueId)
		{
			Content = content;
			ReferenceId = referenceId;
			NextReferenceId = nextReferenceId;
			MimeTypeId = mimeTypeId;
			QueueId = queueId;
			HashCode = content.ToFnv1aHashInt64();
		}

		public Message(string content, long referenceId, long nextReferenceId, string hashCode, short mimeTypeId, short queueId)
		{
			Content = content;
			ReferenceId = referenceId;
			NextReferenceId = nextReferenceId;
			MimeTypeId = mimeTypeId;
			QueueId = queueId;
			HashCode = hashCode;
		}

		public string Content { get; }
		public long ReferenceId { get; }
		public long NextReferenceId { get; }
		public short MimeTypeId { get; }
		// Work-around since queue should be probably be created before
		public short QueueId { get; set; }
		public string HashCode { get; set; }
	}
}
