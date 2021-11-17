using AutoFixture;
using EqualityInformationApi.V1.Controllers;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace EqualityInformationApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class GetEqualityInformationApiControllerTests
    {
        private readonly GetEqualityInformationApiController _classUnderTest;
        private readonly Mock<IGetUseCase> _mockGetUseCase;

        private readonly Mock<HttpResponse> _mockHttpResponse;
        private readonly HeaderDictionary _responseHeaders;

        private readonly Fixture _fixture = new Fixture();

        public GetEqualityInformationApiControllerTests()
        {
            _mockGetUseCase = new Mock<IGetUseCase>();

            _mockHttpResponse = new Mock<HttpResponse>();

            _classUnderTest = new GetEqualityInformationApiController(_mockGetUseCase.Object);

            _responseHeaders = new HeaderDictionary();
            _mockHttpResponse.SetupGet(x => x.Headers).Returns(_responseHeaders);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);

            var controllerContext = new ControllerContext(new ActionContext(mockHttpContext.Object, new RouteData(), new ControllerActionDescriptor()));
            _classUnderTest.ControllerContext = controllerContext;
        }

        [Fact]
        public async Task GetNotFoundIdReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _mockGetUseCase.Setup(x => x.Execute(id)).ReturnsAsync((EqualityInformation) null);

            var response = await _classUnderTest.Get(id).ConfigureAwait(false);
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(id);
        }

        [Fact]
        public async Task GetWithValidIdReturnsOKResponse()
        {
            var equalityInfo = _fixture.Create<EqualityInformation>();
            var id = Guid.NewGuid();
            _mockGetUseCase.Setup(x => x.Execute(id)).ReturnsAsync(equalityInfo);

            var response = await _classUnderTest.Get(id).ConfigureAwait(false);
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(equalityInfo.ToResponse());

            var expectedEtagValue = $"\"{equalityInfo.VersionNumber}\"";
            _classUnderTest.HttpContext.Response.Headers.TryGetValue(HeaderConstants.ETag, out StringValues val).Should().BeTrue();
            val.First().Should().Be(expectedEtagValue);
        }

        [Fact]
        public async Task GetTenureWhenVersionNumberIsNullReturnsEmptyETAG()
        {
            // when versionNumber is null, ETAG is set to 0

            // Arrange
            var equalityInfo = _fixture.Build<EqualityInformation>()
                .With(x => x.VersionNumber, (int?) null)
                .Create();
            var id = Guid.NewGuid();
            _mockGetUseCase.Setup(x => x.Execute(id)).ReturnsAsync(equalityInfo);

            // Act
            var response = await _classUnderTest.Get(id).ConfigureAwait(false);

            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(equalityInfo.ToResponse());

            // Assert ETAG value is 0, no error thrown
            var expectedEtagValue = $"\"\"";
            _classUnderTest.HttpContext.Response.Headers.TryGetValue(HeaderConstants.ETag, out StringValues val).Should().BeTrue();
            val.First().Should().Be(expectedEtagValue);
        }
    }
}
