using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Infrastructure;
using FluentAssertions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EqualityInformationApi.Tests.V1.E2ETests.Steps
{
    public class GetSteps : BaseSteps
    {
        public GetSteps(HttpClient httpClient) : base(httpClient)
        { }

        public async Task WhenTheApiIsCalledToGet(Guid targetId)
        {
            var uri = new Uri($"api/v1/equality-information/?targetId={targetId}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            message.Headers.Add("Authorization", Token);

            message.Method = HttpMethod.Get;

            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _lastResponse = await _httpClient.SendAsync(message).ConfigureAwait(false);

            message.Dispose();
        }

        public void ThenBadRequestIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void ThenNotFoundIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public async Task ThenTheEntityIsReturned(IDynamoDBContext databaseContext)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = DecodeResponse<EqualityInformationResponseObject>(_lastResponse);

            var databaseResponse = await databaseContext.QueryAsync<EqualityInformationDb>(responseContent.TargetId).GetNextSetAsync().ConfigureAwait(false);

            databaseResponse.FirstOrDefault().Should().BeEquivalentTo(responseContent);
        }
    }

}
