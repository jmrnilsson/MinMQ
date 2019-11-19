using FASTER.core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Kestrel.Handlers
{
	class FasterCommitCallback
	{
		public async void ExecuteAsync(object stateInfo)
		{
			FasterCommitState state = stateInfo as FasterCommitState;

			//if (!state.IsCancellationRequested)
			//{
				await state.Log.CommitAsync();
			//}

			if (state.InvokationCount > 0 && state.InvokationCount % 499 == 0)
			{
				state.InvokationCount = 0;
				state.Logger.LogInformation("state.InvokationCount % 499 == 0");
			}
			else
			{
				state.InvokationCount++;
			}
		}

		public void Execute(object stateInfo)
		{
			FasterCommitState state = stateInfo as FasterCommitState;

			//if (!state.IsCancellationRequested)
			//{
			state.Log.Commit(true);
			//}

			if (state.InvokationCount > 0 && state.InvokationCount % 499 == 0)
			{
				state.InvokationCount = 0;
				state.Logger.LogInformation("state.InvokationCount % 499 == 0");
			}
			else
			{
				state.InvokationCount++;
			}
		}
	}
}
