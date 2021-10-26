using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Controllers;
using EqualityInformationApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Controllers
{
    public class EqualityInformationApiControllerTests : LogCallAspectFixture
    {
        private readonly EqualityInformationApiController _equalityController;
        private readonly Mock<IGetByIdUseCase> _mockGetByIdUseCase;
        private readonly Mock<ICreateUseCase> _mockCreateUseCase;
        private readonly Mock<IGetAllUseCase> _mockGetByAllUseCase;
        private readonly Mock<IUpdateUseCase> _mockUpdateUseCase;

        private readonly Mock<HttpRequest> _mockHttpRequest;
        private readonly Mock<HttpResponse> _mockHttpResponse;
        private readonly HeaderDictionary _responseHeaders;

        private readonly Fixture _fixture = new Fixture();

        private const string RequestBodyText = "Some request body text";

        public EqualityInformationApiControllerTests()
        {
            _mockGetByIdUseCase = new Mock<IGetByIdUseCase>();
            _mockCreateUseCase = new Mock<ICreateUseCase>();
            _mockGetByAllUseCase = new Mock<IGetAllUseCase>();
            _mockUpdateUseCase = new Mock<IUpdateUseCase>();

            _mockHttpRequest = new Mock<HttpRequest>();
            _mockHttpResponse = new Mock<HttpResponse>();

            _equalityController = new EqualityInformationApiController(
                _mockGetByAllUseCase.Object,
                _mockCreateUseCase.Object,
                _mockGetByIdUseCase.Object,
                _mockUpdateUseCase.Object
            );

            // changes to allow reading of raw request body
            _mockHttpRequest
                .SetupGet(x => x.Body)
                .Returns(new MemoryStream(Encoding.Default.GetBytes(RequestBodyText)));

            _responseHeaders = new HeaderDictionary();
            _mockHttpResponse.SetupGet(x => x.Headers).Returns(_responseHeaders);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);

            _equalityController.ControllerContext = new ControllerContext(
                new ActionContext(mockHttpContext.Object, new RouteData(), new ControllerActionDescriptor())
            );
        }

        [Fact]
        public async Task GetByIdWhenEntityDoesntExistReturnsNotFoundResult()
        {
            // Arrange
            var query = _fixture.Create<GetByIdQuery>();

            _mockGetByIdUseCase
                .Setup(x => x.Execute(It.IsAny<GetByIdQuery>()))
                .ReturnsAsync((EqualityInformationResponseObject) null);

            // Act
            var response = await _equalityController.GetById(query).ConfigureAwait(false);

            // Assert   
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.Id);
        }

        [Fact]
        public async Task GetByIdWhenEntityExistsReturnsEntity()
        {
            // Arrange
            var query = _fixture.Create<GetByIdQuery>();

            var useCaseResponse = _fixture.Create<EqualityInformationResponseObject>();

            _mockGetByIdUseCase
                .Setup(x => x.Execute(It.IsAny<GetByIdQuery>()))
                .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _equalityController.GetById(query).ConfigureAwait(false);

            // Assert   
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(useCaseResponse);
        }

        [Fact]
        public async Task GetAllWhenManyEntitiesExistReturnsMany()
        {
            // Arrange
            var query = _fixture.Create<EqualityInformationQuery>();

            var useCaseResponse = new GetAllResponseObject
            {
                EqualityData = _fixture.CreateMany<EqualityInformationResponseObject>(10).ToList()
            };

            _mockGetByAllUseCase
               .Setup(x => x.Execute(It.IsAny<EqualityInformationQuery>()))
               .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _equalityController.GetAll(query).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(useCaseResponse);
        }

        [Fact]
        public async Task CreateWhenCalledReturnsCreatedResponse()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();

            var useCaseResponse = _fixture.Create<EqualityInformationResponseObject>();

            _mockCreateUseCase
              .Setup(x => x.Execute(It.IsAny<EqualityInformationObject>()))
              .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _equalityController.Create(request).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(CreatedResult));
            (response as CreatedResult).Value.Should().BeEquivalentTo(useCaseResponse);
        }

        [Fact]
        public async Task UpdateWhenEntityDoesntExistReturnsNotFoundResponse()
        {
            // Arrange
            var query = _fixture.Create<UpdateQualityInformationQuery>();
            var request = _fixture.Create<EqualityInformationObject>();

            _mockUpdateUseCase
                .Setup(x => x.Execute(It.IsAny<UpdateQualityInformationQuery>(), It.IsAny<EqualityInformationObject>(), It.IsAny<string>()))
                .ReturnsAsync((EqualityInformationResponseObject) null);

            // Act
            var response = await _equalityController.Update(query, request).ConfigureAwait(false);

            // Assert   
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.Id);

        }

        [Fact]
        public async Task UpdateWhenEntityExistsReturnsOkResponse()
        {
            // Arrange
            var query = _fixture.Create<UpdateQualityInformationQuery>();
            var request = _fixture.Create<EqualityInformationObject>();

            var useCaseResponse = _fixture.Create<EqualityInformationResponseObject>();

            _mockUpdateUseCase
                .Setup(x => x.Execute(It.IsAny<UpdateQualityInformationQuery>(), It.IsAny<EqualityInformationObject>(), It.IsAny<string>()))
                .ReturnsAsync(useCaseResponse);

            // Act
            var response = await _equalityController.Update(query, request).ConfigureAwait(false);

            // Assert   
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(useCaseResponse);
        }
    }
}
