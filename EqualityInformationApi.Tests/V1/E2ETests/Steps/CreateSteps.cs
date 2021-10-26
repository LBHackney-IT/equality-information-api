using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EqualityInformationApi.Tests.V1.E2ETests.Steps
{
    public class CreateSteps : BaseSteps
    {
        public CreateSteps(HttpClient httpClient) : base(httpClient)
        { }

        public void WhenTheApiIsCalled(EqualityInformationObject request)
        {
            var uri = new Uri($"api/v1/equality-information/", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Post, uri);

            message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;

            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _lastResponse = _httpClient.SendAsync(message).GetAwaiter().GetResult();

            message.Dispose();
        }

        public void ThenBadRequestIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void ThenTheEntityIsReturned(IDynamoDBContext databaseContext)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = DecodeResponse<EqualityInformationResponseObject>(_lastResponse);

            var databaseResponse = databaseContext.LoadAsync<EqualityInformationDb>(responseContent.TargetId, responseContent.Id).GetAwaiter().GetResult();

            databaseResponse.Should().BeEquivalentTo(responseContent);
        }
    }

}
