using System.Threading;
using System.Threading.Tasks;

namespace MinMQ.Service
{
	public interface IFasterWriter
	{
		ValueTask CommitAsync(CancellationToken token = default);
		ValueTask WaitForCommitAsync(long untilAddress = 0, CancellationToken token = default);
		ValueTask<long> EnqueueAsync(byte[] entry, CancellationToken token = default);
	}
}
