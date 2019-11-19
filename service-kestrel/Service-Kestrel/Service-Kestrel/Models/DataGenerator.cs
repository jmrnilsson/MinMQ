using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service_Kestrel.Models
{
	public class DataGenerator
	{
		private static readonly Random random = new Random();
		public static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public static void Initialize(IServiceProvider serviceProvider)
		{
			using (var context = new MessagesContext(serviceProvider.GetRequiredService<DbContextOptions<MessagesContext>>()))
			{
				if (context.Messages.Any())
				{
					return;
				}

				for (int i = 0; i < 1000; i++)
				{
					context.Messages.Add
					(
						new Message
						{
							Content = RandomString(256)
						}
					);
				}

				context.SaveChanges();
			}
		}
	}

}
