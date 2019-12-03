using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinMQ.Service.Configuration;
using MinMq.Service.Repository;
using Optional;
using System;

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

		public DbSet<tQueue> Queues { get; set; }
		public DbSet<tMessage> Messages { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = configuration.Value.ConnectionStringPostgres.SomeNotNull();
			var defaultConnectionString = "Host=localhost;Database=mmq;Username=5a4ba2e9-6c44-49dd-bc6c-b9ea2b901114;Password=effe908d-158d-47c5-a2eb-ad6814ce6083";
			optionsBuilder.UseNpgsql(connectionString.ValueOr(defaultConnectionString));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<tMessage>()
				.HasOne(p => p.Queue)
				.WithMany(b => b.Messages)
				.HasForeignKey(p => p.QueueId)
				.HasConstraintName("FK_Message_Queue");

			modelBuilder.Entity<tMessage>()
				.Property(f => f.MessageId)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<tQueue>()
				.Property(f => f.QueueId)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<tMimeType>()
				.Property(f => f.ByteKey)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<tQueue>()
				.Property(f => f.ByteKey)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<tMessage>()
				.HasOne(p => p.MimeType)
				.WithMany(b => b.Messages)
				.HasForeignKey(p => p.MimeTypeId)
				.HasConstraintName("FK_Message_MimeType");
		}
	}

	public class tQueue
	{
		[Key]
		public int QueueId { get; set; }
		public string Name { get; set; }
		public byte ByteKey { get; set; }

		public List<tMessage> Messages { get; set; }
		public DateTime Added { get; set; }
		public DateTime Changed { get; set; }
	}

	public class tMessage : IMessage
	{
		[Key]
		public int MessageId { get; set; }
		public long ReferenceId { get; set; }
		public long NextReferenceId { get; set; }
		public string Content { get; set; }
		public int QueueId { get; set; }
		public tQueue Queue { get; set; }
		public int MimeTypeId { get; set; }
		public tMimeType MimeType { get; set; }
		public string HashCode { get; set; }
		public DateTime Added { get; set; }
		public DateTime Changed { get; set; }
	}

	public class tMimeType
	{
		[Key]
		public int MimeTypeId { get; set; }
		public byte ByteKey { get; set; }
		public List<tMessage> Messages { get; set; }
		public DateTime Added { get; set; }
		public DateTime Changed { get; set; }
	}

	public class tCursor
	{
		[Key]
		public int MimeTypeId { get; set; }
		public DateTime Added { get; set; }
		public DateTime Changed { get; set; }
		public long NextReferenceId { get; set; }
	}
}
