using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Monri.Core.Services;
using Xunit;

namespace Monri.Tests
{
    public class HttpClientProviderServiceTests
    {
        class FakeHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;
            public FakeHandler(HttpResponseMessage response) => _response = response;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
                Task.FromResult(_response);
        }

        [Fact]
        public async Task PostRequest_ReturnsSuccess_On200WithContent()
        {
            var obj = 123;
            var json = JsonSerializer.Serialize(obj);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var handler = new FakeHandler(response);
            var client = new HttpClient(handler);
            var factoryMock = new Mock<System.Net.Http.IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

            var provider = new HttpClientProviderService(factoryMock.Object);

            var result = await provider.PostRequest<int, object>("http://test", new { });

            Assert.True(result.IsSuccess);
            Assert.Equal(123, result.Value);
        }

        [Fact]
        public async Task GetRequest_ReturnsFailure_OnNonSuccessStatus()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = "Bad"
            };

            var handler = new FakeHandler(response);
            var client = new HttpClient(handler);
            var factoryMock = new Mock<System.Net.Http.IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

            var provider = new HttpClientProviderService(factoryMock.Object);

            var result = await provider.GetRequest<int>("http://test");

            Assert.True(result.IsFailure);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.Error.Code);
        }
    }
}