using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EqualityInformationApi.Tests.V1.UseCase
{
    public class GetByIdUseCaseTests : LogCallAspectFixture
    {
        private readonly Mock<IEqualityInformationGateway> _mockGateway;
        private readonly GetByIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<IEqualityInformationGateway>();
            _classUnderTest = new GetByIdUseCase(_mockGateway.Object);
        }

        [Fact]
        public async Task WhenEntityDoesntExistReturnsNull()
        {
            // Arrange
            var query = _fixture.Create<GetByIdQuery>();

            // Act
            var response = await _classUnderTest.Execute(query).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task WhenEntityExistsReturnsResponseObject()
        {
            // Arrange
            var query = _fixture.Create<GetByIdQuery>();
            var gatewayResponse = _fixture.Create<EqualityInformationDb>();

            _mockGateway
                .Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            var response = await _classUnderTest.Execute(query).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(gatewayResponse);
        }
    }
}
