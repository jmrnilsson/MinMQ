using Microsoft.Extensions.Logging;

namespace MinMQ.Service.Faster
{
	internal class FasterHostedServiceCommitState
	{
		public FasterHostedServiceCommitState(CommitAsyncDelegate commitAsync, ILogger logger, int loggingInterval)
		{
			CommitAsync = commitAsync;
			Logger = logger;
			LoggingInterval = loggingInterval;
			InvokationCount = 0;
		}

		public CommitAsyncDelegate CommitAsync { get; }
		public ILogger Logger { get; }
		public int LoggingInterval { get; }
		public int InvokationCount { get; set; }
	}
}
