﻿using System.Threading;
using System.Threading.Tasks;
using FASTER.core;

namespace MinMQ.Service
{
	public static class FasterLogWriterExtensions
	{
		public static async ValueTask CommitAsync(this FasterLog logger, CancellationToken token = default)
		{
			await logger.CommitAsync(token);
		}

		public static async ValueTask WaitForCommitAsync(this FasterLog logger, long untilAddress = 0, CancellationToken token = default)
		{
			await logger.WaitForCommitAsync(untilAddress, token);
		}

		public static async ValueTask<long> EnqueueAsync(this FasterLog logger, byte[] entry, CancellationToken token = default)
		{
			return await logger.EnqueueAsync(entry, token);
		}
	}
}
