using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Controllers;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.Infrastructure.Exceptions;
using EqualityInformationApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class EqualityInformationApiControllerTests
    {
        private readonly EqualityInformationApiController _classUnderTest;
        private readonly Mock<ICreateUseCase> _mockCreateUseCase;
        private readonly Mock<IGetUseCase> _mockGetUseCase;
        private readonly Mock<IPatchUseCase> _mockPatchUseCase;

        private readonly Mock<ITokenFactory> _mockTokenFactory;
        private readonly Mock<IHttpContextWrapper> _mockContextWrapper;

        private readonly Mock<HttpRequest> _mockHttpRequest;
        private readonly HeaderDictionary _requestHeaders;
        private readonly Mock<HttpResponse> _mockHttpResponse;
        private readonly HeaderDictionary _responseHeaders;

        private readonly Fixture _fixture = new Fixture();
        private const string RequestBodyText = "Some request body text";

        public EqualityInformationApiControllerTests()
        {
            _mockCreateUseCase = new Mock<ICreateUseCase>();
            _mockGetUseCase = new Mock<IGetUseCase>();
            _mockPatchUseCase = new Mock<IPatchUseCase>();

            _mockTokenFactory = new Mock<ITokenFactory>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();

            _classUnderTest = new EqualityInformationApiController(
                _mockCreateUseCase.Object,
                _mockGetUseCase.Object,
                _mockPatchUseCase.Object,
                _mockTokenFactory.Object,
                _mockContextWrapper.Object);

            // changes to allow reading of raw request body
            _mockHttpRequest = new Mock<HttpRequest>();
            _mockHttpRequest.SetupGet(x => x.Body).Returns(new MemoryStream(Encoding.Default.GetBytes(RequestBodyText)));

            _requestHeaders = new HeaderDictionary();
            _mockHttpRequest.SetupGet(x => x.Headers).Returns(_requestHeaders);

            _mockContextWrapper
                .Setup(x => x.GetContextRequestHeaders(It.IsAny<HttpContext>()))
                .Returns(_requestHeaders);

            _mockHttpResponse = new Mock<HttpResponse>();
            _responseHeaders = new HeaderDictionary();
            _mockHttpResponse.SetupGet(x => x.Headers).Returns(_responseHeaders);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);

            var controllerContext = new ControllerContext(new ActionContext(mockHttpContext.Object, new RouteData(), new ControllerActionDescriptor()));
            _classUnderTest.ControllerContext = controllerContext;
        }

        [Fact]
        public async Task CreateWhenCalledReturnsCreatedResponse()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();

            var useCaseResponse = _fixture.Create<EqualityInformationResponseObject>();

            _mockCreateUseCase
              .Setup(x => x.Execute(It.IsAny<EqualityInformationObject>(), It.IsAny<Token>()))
              .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _classUnderTest.Create(request).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(CreatedResult));
            (response as CreatedResult).Value.Should().BeEquivalentTo(useCaseResponse);
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

        [Fact]
        public async Task PatchTargetIdNotFoundIdReturnsNotFound()
        {
            var request = _fixture.Create<PatchEqualityInformationObject>();
            _mockPatchUseCase.Setup(x => x.Execute(request, It.IsAny<string>(), It.IsAny<Token>(), It.IsAny<int?>())).ReturnsAsync((EqualityInformationResponseObject) null);

            var response = await _classUnderTest.Patch(request.TargetId, request).ConfigureAwait(false);
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(request.TargetId);
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData(0, 1)]
        [InlineData(0, null)]
        [InlineData(2, 1)]
        public async Task PatchVersionNumberConflictExceptionReturns409(int? expected, int? actual)
        {
            // Arrange
            var request = _fixture.Create<PatchEqualityInformationObject>();

            _requestHeaders.Add(HeaderConstants.IfMatch, $"\"{new StringValues(expected?.ToString())}\"");
            var exception = new VersionNumberConflictException(expected, actual);

            _mockPatchUseCase.Setup(x => x.Execute(request, It.IsAny<string>(), It.IsAny<Token>(), It.IsAny<int?>())).ThrowsAsync(exception);

            // Act
            var response = await _classUnderTest.Patch(request.TargetId, request).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(ConflictObjectResult));
            (response as ConflictObjectResult).Value.Should().BeEquivalentTo(exception.Message);
        }

        [Fact]
        public async Task PatchWhenValidReturns200OkResponse()
        {
            var request = _fixture.Create<PatchEqualityInformationObject>();

            var equalityInfo = _fixture.Create<EqualityInformationResponseObject>();
            _mockPatchUseCase.Setup(x => x.Execute(request, It.IsAny<string>(), It.IsAny<Token>(), It.IsAny<int?>()))
                             .ReturnsAsync(equalityInfo);

            var response = await _classUnderTest.Patch(request.TargetId, request).ConfigureAwait(false);

            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(equalityInfo);
        }
    }
}
