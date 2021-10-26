using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
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
    public class UpdateSteps : BaseSteps
    {
        private readonly Fixture _fixture = new Fixture();

        public UpdateSteps(HttpClient httpClient) : base(httpClient)
        { }

        public void WhenTheApiIsCalled(EqualityInformationObject request, string id)
        {
            var uri = new Uri($"api/v1/equality-information/{id}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Patch, uri);

            message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Patch;

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

        public void ThenOkResponseIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndNoEntitiesReturned()
        {
            var responseContent = DecodeResponse<GetAllResponseObject>(_lastResponse);

            responseContent.EqualityData.Should().HaveCount(0);
        }

        public void AndManyEntitiesReturned(int numberOfEntities)
        {
            var responseContent = DecodeResponse<GetAllResponseObject>(_lastResponse);

            responseContent.EqualityData.Should().HaveCount(numberOfEntities);
        }

        public void ThenNotFoundRequestIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void ThenTheEntityIsReturned(IDynamoDBContext databaseContext)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = DecodeResponse<EqualityInformationResponseObject>(_lastResponse);

            var databaseResponse = databaseContext.LoadAsync<EqualityInformationDb>(responseContent.TargetId, responseContent.Id).GetAwaiter().GetResult();

            databaseResponse.Should().BeEquivalentTo(responseContent);
        }

        public void WhenTheApiIsCalledWithChangesInTheRequest(EqualityInformationDb entity)
        {
            var request = _fixture.Build<EqualityInformationObject>()
                .With(x => x.TargetId, entity.TargetId)
                .Create();

            WhenTheApiIsCalled(request, entity.Id.ToString());
        }

        public void WhenTheApiIsCalledWithNoChangesInTheRequest(EqualityInformationDb entity)
        {
            var request = new EqualityInformationObject
            {
                TargetId = entity.TargetId,
                Gender = entity.Gender,
                Nationality = entity.Nationality,
                Ethnicity = entity.Ethnicity,
                ReligionOrBelief = entity.ReligionOrBelief,
                SexualOrientation = entity.SexualOrientation,
                MarriageOrCivilPartnership = entity.MarriageOrCivilPartnership,
                PregnancyOrMaternity = entity.PregnancyOrMaternity,
                NationalInsuranceNumber = entity.NationalInsuranceNumber,
                Languages = entity.Languages,
                CaringResponsibilities = entity.CaringResponsibilities,
                Disabled = entity.Disabled,
                CommunicationRequirements = entity.CommunicationRequirements,
                EconomicSituation = entity.EconomicSituation,
                HomeSituation = entity.HomeSituation,
                ArmedForces = entity.ArmedForces
            };

            WhenTheApiIsCalled(request, entity.Id.ToString());
        }

        internal void AndTheEntityWasntUpdatedInTheDatabase(IDynamoDBContext dbContext, EqualityInformationDb entity)
        {
            var responseContent = DecodeResponse<EqualityInformationResponseObject>(_lastResponse);

            var databaseResponse = dbContext.LoadAsync<EqualityInformationDb>(responseContent.TargetId, responseContent.Id).GetAwaiter().GetResult();

            // Should match original entity
            databaseResponse.Should().BeEquivalentTo(entity);
        }

        internal void AndTheEntityWasUpdatedInTheDatabase(IDynamoDBContext dbContext, EqualityInformationDb entity)
        {
            var responseContent = DecodeResponse<EqualityInformationResponseObject>(_lastResponse);

            var databaseResponse = dbContext.LoadAsync<EqualityInformationDb>(responseContent.TargetId, responseContent.Id).GetAwaiter().GetResult();

            // Should differ from original entity
            databaseResponse.Should().NotBeEquivalentTo(entity);
        }
    }
}
