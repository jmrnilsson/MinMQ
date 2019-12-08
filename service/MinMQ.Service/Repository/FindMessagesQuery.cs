using System.Collections.Generic;
using MinMq.Service.Entities;

namespace MinMq.Service.Repository
{
	public class FindMessagesQuery
	{
		public FindMessagesQuery(List<Message> messages)
		{
			Messages = messages;
		}

		public List<Message> Messages { get; }
	}
}
