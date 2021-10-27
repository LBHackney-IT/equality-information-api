using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Controllers;
using EqualityInformationApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Hackney.Core.Http;
using Hackney.Core.JWT;
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
    [Collection("LogCall collection")]
    public class EqualityInformationApiControllerTests
    {
        private readonly EqualityInformationApiController _equalityController;
        private readonly Mock<ICreateUseCase> _mockCreateUseCase;

        private readonly Mock<ITokenFactory> _mockTokenFactory;
        private readonly Mock<IHttpContextWrapper> _mockContextWrapper;

        private readonly Fixture _fixture = new Fixture();

        public EqualityInformationApiControllerTests()
        {
            _mockCreateUseCase = new Mock<ICreateUseCase>();

            _mockTokenFactory = new Mock<ITokenFactory>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();

            _equalityController = new EqualityInformationApiController(
                _mockCreateUseCase.Object,
                _mockTokenFactory.Object,
                _mockContextWrapper.Object);
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
            var response = await _equalityController.Create(request).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(CreatedResult));
            (response as CreatedResult).Value.Should().BeEquivalentTo(useCaseResponse);
        }
    }
}
