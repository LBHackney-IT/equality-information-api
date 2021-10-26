using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EqualityInformationApi.Tests.V1.UseCase
{
    public class UpdateUseCaseTests : LogCallAspectFixture
    {
        private readonly Mock<IEqualityInformationGateway> _mockGateway;
        private readonly UpdateUseCase _classUnderTest;
        private readonly Fixture _fixture;

        public UpdateUseCaseTests()
        {
            _mockGateway = new Mock<IEqualityInformationGateway>();
            _classUnderTest = new UpdateUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task WhenEntityDoesntExistReturnsNull()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();
            var query = _fixture.Create<UpdateEqualityInformationQuery>();
            var requestBody = "";

            _mockGateway
                .Setup(x => x.Update(It.IsAny<Guid>(), It.IsAny<EqualityInformationObject>(), It.IsAny<string>()))
                .ReturnsAsync((EqualityInformationDb) null);

            // Act
            var response = await _classUnderTest.Execute(query, request, requestBody).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task WhenCalledReturnsResponse()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();
            var query = _fixture.Create<UpdateEqualityInformationQuery>();
            var requestBody = "";

            var gatewayResponse = _fixture.Create<EqualityInformationDb>();

            _mockGateway
                .Setup(x => x.Update(It.IsAny<Guid>(), It.IsAny<EqualityInformationObject>(), It.IsAny<string>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            var response = await _classUnderTest.Execute(query, request, requestBody).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(gatewayResponse);
        }
    }
}
