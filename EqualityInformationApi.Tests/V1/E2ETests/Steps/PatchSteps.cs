using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.Tests.V1.E2ETests.Fixtures;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.Infrastructure.Constants;
using FluentAssertions;
using Hackney.Core.Sns;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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

        private void VerifyEventData(object eventDataJsonObj, Dictionary<string, object> expected)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(eventDataJsonObj.ToString(), CreateJsonOptions());
            data["nationalInsuranceNumber"].ToString().Should().Be(expected["nationalInsuranceNumber"].ToString());

            var eventLanguages = System.Text.Json.JsonSerializer.Deserialize<List<LanguageInfo>>(data["languages"].ToString(), CreateJsonOptions());
            eventLanguages.Should().BeEquivalentTo(expected["languages"] as List<LanguageInfo>);
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

        public async Task ThenTheUpdatedSnsEventIsRaised(EqualityInformationFixture fixture, SnsEventVerifier<EntityEventSns> snsVerifier)
        {
            var responseContent = DecodeResponse<EqualityInformationDb>(_lastResponse);

            var databaseResponse = await fixture.DbFixture.DynamoDbContext
                                                .LoadAsync<EqualityInformationDb>(responseContent.TargetId, responseContent.Id)
                                                .ConfigureAwait(false);

            Action<EntityEventSns> verifyFunc = (actual) =>
            {
                actual.CorrelationId.Should().NotBeEmpty();
                actual.DateTime.Should().BeCloseTo(DateTime.UtcNow, 5000);
                actual.EntityId.Should().Be(databaseResponse.TargetId);

                var expectedOldData = new Dictionary<string, object>
                {
                    { "nationalInsuranceNumber", fixture.Entity.NationalInsuranceNumber },
                    { "languages", fixture.Entity.Languages }
                };
                var expectedNewData = new Dictionary<string, object>
                {
                    { "nationalInsuranceNumber", databaseResponse.NationalInsuranceNumber },
                    { "languages", databaseResponse.Languages }
                };
                VerifyEventData(actual.EventData.OldData, expectedOldData);
                VerifyEventData(actual.EventData.NewData, expectedNewData);

                actual.EventType.Should().Be(UpdateEventConstants.EVENTTYPE);
                actual.Id.Should().NotBeEmpty();
                actual.SourceDomain.Should().Be(UpdateEventConstants.SOURCEDOMAIN);
                actual.SourceSystem.Should().Be(UpdateEventConstants.SOURCESYSTEM);
                actual.User.Email.Should().Be("e2e-testing@development.com");
                actual.User.Name.Should().Be("Tester");
                actual.Version.Should().Be(UpdateEventConstants.V1VERSION);
            };

            snsVerifier.VerifySnsEventRaised(verifyFunc).Should().BeTrue(snsVerifier.LastException?.Message);
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
