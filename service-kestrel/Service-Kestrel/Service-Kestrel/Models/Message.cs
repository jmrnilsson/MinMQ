using System;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace MinMQ.Service.Models
{
	public sealed class Message
	{
		public Message()
		{
			Instant = SystemClock.Instance.GetCurrentInstant();
		}

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
		public Instant Instant { get; set; }
		public string Content { get; set; }
	}
}
