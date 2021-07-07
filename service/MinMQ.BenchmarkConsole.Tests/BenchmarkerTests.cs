using Moq;
using Shouldly;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MinMQ.BenchmarkConsole.Tests
{
    public class BenchmarkerTests : IDisposable
	{
		Benchmarker benchmarker;
		Mock<IHttpClientFactory> httpClientFactoryMock;
		DelegatingHandlerStub httpHandler;
		CancellationTokenSource cts = new CancellationTokenSource();

		public BenchmarkerTests()
		{
			httpClientFactoryMock = new Mock<IHttpClientFactory>();
			benchmarker = new Benchmarker(httpClientFactoryMock.Object, new MinMQTestEnvVar());
			httpHandler = new DelegatingHandlerStub();
			var client = new HttpClient(httpHandler);
			httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
		}

		[Fact]
        public async Task Benchmarker_should_send_http_requests_for_every_document()
        {	
			await benchmarker.Start(cts.Token);
			httpHandler.InvocationCount.ShouldBe(500);
        }

		public void Dispose()
		{
			benchmarker = null;
			httpClientFactoryMock = null;
			httpHandler = null;
			cts = null;
		}

		public class DelegatingHandlerStub : DelegatingHandler
		{
			private int invocationCount;
			private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
			public DelegatingHandlerStub()
			{
				_handlerFunc = (request, cancellationToken) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
			}

			public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
			{
				_handlerFunc = handlerFunc;
			}
			
			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				invocationCount++;
				return _handlerFunc(request, cancellationToken);
			}

			public int InvocationCount => invocationCount;
		}

		public class MinMQTestEnvVar : IMinMQEnvironmentVariables
		{
			public int NTree => 5;

			public int NumberOfObjects => 250;

			public string RequestPath => "https://localhost:666/send";
		}
	}
}
