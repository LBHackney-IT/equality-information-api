using EqualityInformationApi.V1.Boundary.Response;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EqualityInformationApi.Tests.V1.E2ETests.Steps
{
    public class GetAllSteps : BaseSteps
    {
        public GetAllSteps(HttpClient httpClient) : base(httpClient)
        { }

        public void WhenTheApiIsCalled(string targetId)
        {
            var url = $"api/v1/equality-information/?targetId={targetId}";

            _lastResponse = _httpClient.GetAsync(url).GetAwaiter().GetResult();
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
    }
}
