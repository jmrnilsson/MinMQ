using Microsoft.EntityFrameworkCore;

namespace MinMQ.Service.Models
{
	public class MessagesContext : DbContext
	{
		public MessagesContext(DbContextOptions<MessagesContext> options)
			: base(options)
		{
		}

		public DbSet<Message> Messages { get; set; }
	}
}
