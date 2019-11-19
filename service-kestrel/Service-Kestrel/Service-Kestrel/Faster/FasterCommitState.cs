using FASTER.core;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Service_Kestrel.Handlers
{
	// https://docs.microsoft.com/en-us/dotnet/api/system.threading.timercallback?view=netframework-4.8
	class FasterCommitState
	{
		// private CancellationToken token;

		public FasterCommitState(FasterLog log, ILogger logger)
		{
			// this.token = token;
			Log = log;
			Logger = logger;
			InvokationCount = 0;

		}

		// public bool IsCancellationRequested => token.IsCancellationRequested;
        public FasterLog Log { get; }
		public ILogger Logger { get; }
		public int InvokationCount { get; set; }
	}
}
