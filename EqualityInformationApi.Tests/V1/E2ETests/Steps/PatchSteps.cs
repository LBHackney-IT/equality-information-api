using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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

        public async Task WhenTheApiIsCalledToPatch(PatchEqualityInformationObject request, Guid id)
        {
            int? defaultIfMatch = 0;
            await WhenTheApiIsCalledToPatch(request, id, defaultIfMatch).ConfigureAwait(false);
        }
        public async Task WhenTheApiIsCalledToPatch(PatchEqualityInformationObject request, Guid id, int? ifMatch)
        {
            request.Id = id;

            var uri = new Uri($"api/v1/equality-information/{id}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Headers.Add("Authorization", Token);
            message.Headers.TryAddWithoutValidation(HeaderConstants.IfMatch, $"\"{ifMatch?.ToString()}\"");

            var jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new[] { new StringEnumConverter() }
            };
            var requestJson = JsonConvert.SerializeObject(request, jsonSettings);
            message.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
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

        public void ThenNotFoundIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public async Task ThenTheEntityIsReturned(IDynamoDBContext databaseContext)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = DecodeResponse<EqualityInformationResponseObject>(_lastResponse);

            var databaseResponse = await databaseContext.LoadAsync<EqualityInformationDb>(responseContent.TargetId, responseContent.Id).ConfigureAwait(false);
            databaseResponse.Should().BeEquivalentTo(responseContent);
            databaseResponse.VersionNumber.Should().Be(1);
        }

        public async Task ThenTheValidationErrorsAreReturned(string errorPropertyName)
        {
            await ThenTheValidationErrorsAreReturned(errorPropertyName, null);
        }
        public async Task ThenTheValidationErrorsAreReturned(string errorPropertyName, string errorCode)
        {
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            JObject jo = JObject.Parse(responseContent);
            var errors = jo["errors"].Children();

            ShouldHaveErrorFor(errors, errorPropertyName, errorCode);
        }

        public async Task ThenConflictIsReturned(int? versionNumber)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            var sentVersionNumberString = (versionNumber is null) ? "{null}" : versionNumber.ToString();
            responseContent.Should().Contain($"The version number supplied ({sentVersionNumberString}) does not match the current value on the entity (0).");
        }
    }
}
