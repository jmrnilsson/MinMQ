using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinMq.Service.Models
{
	public class MessageQueueContext : DbContext
	{
		public MessageQueueContext(DbContextOptions<MessageQueueContext> options)
            : base(options)
		{
		}

		public DbSet<Queue> Queues { get; set; }
		public DbSet<Message> Messages { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			=> optionsBuilder.UseNpgsql("Host=localhost;Database=mmq;Username=5a4ba2e9-6c44-49dd-bc6c-b9ea2b901114;Password=effe908d-158d-47c5-a2eb-ad6814ce6083");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Message>()
				.HasOne(p => p.Queue)
				.WithMany(b => b.Messages)
				.HasForeignKey(p => p.QueueId)
				.HasConstraintName("FK_Message_Queue");
		}
	}

	public class Queue
	{
		public int QueueId { get; set; }
		public string Name { get; set; }

		public List<Message> Messages { get; set; }
	}

	public class Message
	{
		public int MessageId { get; set; }
		public long ReferenceId { get; set; }
		public string MimeType { get; set; }
		public string Content { get; set; }
		public int QueueId { get; set; }
		public Queue Queue { get; set; }

	}
}
