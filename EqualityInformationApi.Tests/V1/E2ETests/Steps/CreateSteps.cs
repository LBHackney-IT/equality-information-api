using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.Tests.V1.E2ETests.Fixtures;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.Infrastructure.Constants;
using FluentAssertions;
using Hackney.Core.Sns;
using Hackney.Core.Testing.Sns;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EqualityInformationApi.Tests.V1.E2ETests.Steps
{
    public class CreateSteps : BaseSteps
    {
        public CreateSteps(HttpClient httpClient) : base(httpClient)
        { }

        private static void ShouldHaveErrorFor(JEnumerable<JToken> errors, string propertyName, string errorCode = null)
        {
            var error = errors.FirstOrDefault(x => (x.Path.Split('.').Last().Trim('\'', ']')) == propertyName) as JProperty;
            error.Should().NotBeNull();
            if (!string.IsNullOrEmpty(errorCode))
                error.Value.ToString().Should().Contain(errorCode);
        }

        public async Task WhenTheApiIsCalledToCreate(EqualityInformationObject request)
        {
            var uri = new Uri($"api/v1/equality-information/", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Headers.Add("Authorization", Token);

            message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;

            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _lastResponse = await _httpClient.SendAsync(message).ConfigureAwait(false);

            message.Dispose();
        }

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

        public async Task ThenTheEntityIsReturned(IDynamoDBContext databaseContext)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = DecodeResponse<EqualityInformationResponseObject>(_lastResponse);

            var databaseResponse = await databaseContext.LoadAsync<EqualityInformationDb>(responseContent.TargetId, responseContent.Id).ConfigureAwait(false);

            databaseResponse.Should().BeEquivalentTo(responseContent);
        }

        public async Task ThenTheEqualityInformationCreatedEventIsRaised(EqualityInformationFixture fixture, ISnsFixture snsFixture)
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

                var expected = databaseResponse.ToDomain();
                var actualNewData = JsonSerializer.Deserialize<EqualityInformation>(actual.EventData.NewData.ToString(), CreateJsonOptions());

                actualNewData.Should().BeEquivalentTo(expected);
                actual.EventData.OldData.Should().BeNull();

                actual.EventType.Should().Be(CreateEventConstants.EVENTTYPE);
                actual.Id.Should().NotBeEmpty();
                actual.SourceDomain.Should().Be(CreateEventConstants.SOURCEDOMAIN);
                actual.SourceSystem.Should().Be(CreateEventConstants.SOURCESYSTEM);
                actual.User.Email.Should().Be("e2e-testing@development.com");
                actual.User.Name.Should().Be("Tester");
                actual.Version.Should().Be(CreateEventConstants.V1VERSION);
            };

            var snsVerifer = snsFixture.GetSnsEventVerifier<EntityEventSns>();
            (await snsVerifer.VerifySnsEventRaised<EntityEventSns>(verifyFunc)).Should().BeTrue(snsVerifer.LastException?.Message);
        }

        public async Task ThenTheValidationErrorsAreReturned(List<KeyValuePair<string, string>> expectedErrors)
        {
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            JObject jo = JObject.Parse(responseContent);
            var errors = jo["errors"].Children();

            foreach (var err in expectedErrors)
                ShouldHaveErrorFor(errors, err.Key, err.Value);
        }
    }

}
