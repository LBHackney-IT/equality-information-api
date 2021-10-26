using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Infrastructure;
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
    public class GetByIdSteps : BaseSteps
    {
        public GetByIdSteps(HttpClient httpClient) : base(httpClient)
        { }

        public void WhenTheApiIsCalled(string id, string targetId)
        {
            var url = $"api/v1/equality-information/{id}/?targetId={targetId}";

            _lastResponse = _httpClient.GetAsync(url).GetAwaiter().GetResult();
        }

        public void ThenNotFoundIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void ThenBadRequestIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void ThenTheEntityIsReturned(EqualityInformationDb entity)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = DecodeResponse<EqualityInformationResponseObject>(_lastResponse);

            responseContent.Should().BeEquivalentTo(entity);
        }
    }
}
