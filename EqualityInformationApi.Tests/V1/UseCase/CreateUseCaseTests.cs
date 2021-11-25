using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace EqualityInformationApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class CreateUseCaseTests
    {
        private readonly Mock<IEqualityInformationGateway> _mockGateway;
        private readonly Mock<ISnsGateway> _mockSnsGateway;
        private readonly Mock<ISnsFactory> _mockSnsFactory;


        private readonly CreateUseCase _classUnderTest;
        private readonly Fixture _fixture;

        public CreateUseCaseTests()
        {
            _mockGateway = new Mock<IEqualityInformationGateway>();
            _mockSnsGateway = new Mock<ISnsGateway>();
            _mockSnsFactory = new Mock<ISnsFactory>();

            _classUnderTest = new CreateUseCase(
                _mockGateway.Object,
                _mockSnsGateway.Object,
                _mockSnsFactory.Object);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task WhenCalledReturnsResponse()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();
            var token = new Token();

            var gatewayResponse = _fixture.Create<EqualityInformation>();

            _mockGateway
                .Setup(x => x.Create(It.IsAny<EqualityInformationObject>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            var response = await _classUnderTest.Execute(request, token).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(gatewayResponse.ToResponse());

            _mockSnsGateway.Verify(x => x.Publish(It.IsAny<EntityEventSns>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
