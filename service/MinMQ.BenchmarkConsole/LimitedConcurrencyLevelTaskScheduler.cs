using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MinMQ.BenchmarkConsole
{
	// https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler?view=netframework-4.8
	public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
	{
		[ThreadStatic]
		private static bool currentThreadIsProcessingItems;
		private readonly LinkedList<Task> tasks = new LinkedList<Task>();
		private readonly int maxDegreeOfParallelism;
		private int delegatesQueuedOrRunning = 0;

		public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
		{
			if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
			this.maxDegreeOfParallelism = maxDegreeOfParallelism;
		}

		public sealed override int MaximumConcurrencyLevel { get { return maxDegreeOfParallelism; } }

		protected sealed override void QueueTask(Task task)
		{
			// Add the task to the list of tasks to be processed.  If there aren't enough 
			// delegates currently queued or running to process tasks, schedule another. 
			lock (tasks)
			{
				tasks.AddLast(task);
				if (delegatesQueuedOrRunning < maxDegreeOfParallelism)
				{
					++delegatesQueuedOrRunning;
					NotifyThreadPoolOfPendingWork();
				}
			}
		}

		private void NotifyThreadPoolOfPendingWork()
		{
			ThreadPool.UnsafeQueueUserWorkItem(_ =>
			{
				currentThreadIsProcessingItems = true;
				try
				{
					while (true)
					{
						Task item;
						lock (tasks)
						{
							if (tasks.Count == 0)
							{
								--delegatesQueuedOrRunning;
								break;
							}

							item = tasks.First.Value;
							tasks.RemoveFirst();
						}

						base.TryExecuteTask(item);
					}
				}
				finally
				{
					currentThreadIsProcessingItems = false;
				}
			}, null);
		}

		protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			if (!currentThreadIsProcessingItems) return false;

			if (taskWasPreviouslyQueued)
			{
				if (TryDequeue(task))
				{
					return base.TryExecuteTask(task);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return base.TryExecuteTask(task);
			}
		}

		protected sealed override bool TryDequeue(Task task)
		{
			lock (tasks) return tasks.Remove(task);
		}

		protected sealed override IEnumerable<Task> GetScheduledTasks()
		{
			bool lockTaken = false;
			try
			{
				Monitor.TryEnter(tasks, ref lockTaken);
				if (lockTaken) return tasks;
				else throw new NotSupportedException();
			}
			finally
			{
				if (lockTaken) Monitor.Exit(tasks);
			}
		}
	}
}
