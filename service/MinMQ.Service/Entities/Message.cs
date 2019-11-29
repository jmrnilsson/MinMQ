using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
			QueueByteKey = 0x00;
			HashCode = content.ToFnv1aHashInt64();
		}

		public string Content { get; }
		public long ReferenceId { get; }
		public long NextReferenceId { get; }
		public byte MimeTypeByteKey { get; }
		public byte QueueByteKey { get; }
		public string HashCode { get; set; }
	}
}
