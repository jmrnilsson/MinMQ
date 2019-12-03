using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinMq.Service.Repository;
using MinMQ.Service.Configuration;
using Optional;

namespace MinMq.Service.Models
{
	public class MessageQueueContext : DbContext
	{
		private readonly IOptions<MinMQConfiguration> configuration;

		public MessageQueueContext(DbContextOptions<MessageQueueContext> options, IOptions<MinMQConfiguration> configuration)
			: base(options)
		{
			this.configuration = configuration;
		}

		public DbSet<Queue> Queues { get; set; }
		public DbSet<Message> Messages { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = configuration.Value.ConnectionStringPostgres.SomeNotNull();
			var defaultConnectionString = "Host=localhost;Database=mmq;Username=5a4ba2e9-6c44-49dd-bc6c-b9ea2b901114;Password=effe908d-158d-47c5-a2eb-ad6814ce6083";
			optionsBuilder.UseNpgsql(connectionString.ValueOr(defaultConnectionString));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Message>()
				.HasOne(p => p.Queue)
				.WithMany(b => b.Messages)
				.HasForeignKey(p => p.QueueId)
				.HasConstraintName("FK_Message_Queue");

			modelBuilder.Entity<Message>()
				.Property(f => f.MessageId)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<Queue>()
				.Property(f => f.QueueId)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<MimeType>()
				.Property(f => f.ByteKey)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<Queue>()
				.Property(f => f.ByteKey)
				.ValueGeneratedOnAdd();

			// No FK for mime-type yet
		}
	}

	public class Queue
	{
		public int QueueId { get; set; }
		public string Name { get; set; }
		public byte ByteKey { get; set; }

		public List<Message> Messages { get; set; }
	}

	public class Message : IMessage
	{
		[Key]
		public int MessageId { get; set; }
		public long ReferenceId { get; set; }
		public long NextReferenceId { get; set; }
		public string MimeType { get; set; }
		public string Content { get; set; }
		public int QueueId { get; set; }
		public Queue Queue { get; set; }
		public string HashCode { get; set; }
	}

	public class MimeType
	{
		[Key]
		public string MimeTypeId { get; set; }
		public byte ByteKey { get; set; }
	}
}
