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
			benchmarker = new Benchmarker(httpClientFactoryMock.Object, 5, 250, cts.Token);
			httpHandler = new DelegatingHandlerStub();
			var client = new HttpClient(httpHandler);
			httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
		}

		[Fact]
        public async Task Benchmarker_should_send_http_requests_for_every_document()
        {	
			await benchmarker.Start();
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
			private int invocationCount = 0;
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
	}
}
