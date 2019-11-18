using NodaTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Service_Kestrel.Models
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
