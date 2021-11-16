using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EqualityInformationApi.Tests.V1.E2ETests.Steps
{
    public class PatchSteps : BaseSteps
    {
        public PatchSteps(HttpClient httpClient) : base(httpClient)
        { }

        private static void ShouldHaveErrorFor(JEnumerable<JToken> errors, string propertyName, string errorCode = null)
        {
            var error = errors.FirstOrDefault(x => (x.Path.Split('.').Last().Trim('\'', ']')) == propertyName) as JProperty;
            error.Should().NotBeNull();
            if (!string.IsNullOrEmpty(errorCode))
                error.Value.ToString().Should().Contain(errorCode);
        }

        public async Task WhenTheApiIsCalledToPatch(PatchEqualityInformationObject request, string id)
        {
            request.Id = id;

            var uri = new Uri($"api/v1/equality-information/{request.Id}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Headers.Add("Authorization", Token);

            message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Patch;

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

        public async Task ThenTheEntityIsReturned(IDynamoDBContext databaseContext)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = DecodeResponse<EqualityInformationResponseObject>(_lastResponse);

            var databaseResponse = await databaseContext.LoadAsync<EqualityInformationDb>(responseContent.TargetId, responseContent.Id).ConfigureAwait(false);

            databaseResponse.Should().BeEquivalentTo(responseContent);
        }
    }
}
