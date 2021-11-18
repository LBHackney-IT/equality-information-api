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
using System;
using System.Threading.Tasks;
using Xunit;

namespace EqualityInformationApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class PatchUseCaseTests
    {
        private readonly Mock<IEqualityInformationGateway> _mockGateway;
        private readonly Mock<ISnsGateway> _mockSnsGateway;
        private readonly Mock<ISnsFactory> _mockSnsFactory;

        private readonly PatchUseCase _classUnderTest;
        private readonly Fixture _fixture;
        private const string ARN = "some-arn";

        public PatchUseCaseTests()
        {
            _mockGateway = new Mock<IEqualityInformationGateway>();
            _mockSnsGateway = new Mock<ISnsGateway>();
            _mockSnsFactory = new Mock<ISnsFactory>();

            _classUnderTest = new PatchUseCase(
                _mockGateway.Object,
                _mockSnsGateway.Object,
                _mockSnsFactory.Object);

            _fixture = new Fixture();

            Environment.SetEnvironmentVariable("EQUALITY_INFORMATION_SNS_ARN", ARN);
        }

        [Fact]
        public async Task WhenEqualityInfoDoesntExistReturnsNull()
        {
            var mockQuery = _fixture.Create<PatchEqualityInformationObject>();
            var mockToken = _fixture.Create<Token>();

            _mockGateway.Setup(x => x.Update(mockQuery, It.IsAny<int?>()))
                        .ReturnsAsync((EqualityInformation) null);

            var response = await _classUnderTest.Execute(mockQuery, mockToken, null)
                                                .ConfigureAwait(false);

            response.Should().BeNull();
        }

        [Fact]
        public async Task WhenCalledReturnsResponse()
        {
            // Arrange
            var request = _fixture.Create<PatchEqualityInformationObject>();
            var token = new Token();

            var gatewayResponse = _fixture.Create<EqualityInformation>();
            var snsMessage = _fixture.Create<EntityEventSns>();

            _mockGateway.Setup(x => x.Update(request, It.IsAny<int?>()))
                        .ReturnsAsync(gatewayResponse);
            _mockSnsFactory.Setup(x => x.Update(gatewayResponse, token))
                           .Returns(snsMessage);

            // Act
            var response = await _classUnderTest.Execute(request, token, null)
                                                .ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(gatewayResponse.ToResponse());

            _mockSnsFactory.Verify(x => x.Update(gatewayResponse, token), Times.Once);
            _mockSnsGateway.Verify(x => x.Publish(snsMessage, ARN, It.IsAny<string>()), Times.Once);
        }
    }
}
