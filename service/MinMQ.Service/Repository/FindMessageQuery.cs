using MinMq.Service.Entities;
using System.Collections.Generic;

namespace MinMq.Service.Repository
{
	public class FindMessagesQuery
	{
		public List<Message> Messages { get; }

		public FindMessagesQuery(List<Entities.Message> messages)
		{
			Messages = messages;
		}
	}
}
