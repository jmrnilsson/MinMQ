﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service_Kestrel.Models
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