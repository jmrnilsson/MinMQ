using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service_Kestrel.Faster
{
	public class BlockingCollectionQueue
	{
		private BlockingCollection<byte[]> _jobs = new BlockingCollection<byte[]>();

		public BlockingCollectionQueue()
		{
			var thread = new Thread(new ThreadStart(OnStart));
			thread.IsBackground = true;
			thread.Start();
		}

		public void Enqueue(byte[] job)
		{
			_jobs.Add(job);
		}

		private void OnStart()
		{
			foreach (var job in _jobs.GetConsumingEnumerable(CancellationToken.None))
			{
				Console.WriteLine(job);
			}
		}
	}
}
