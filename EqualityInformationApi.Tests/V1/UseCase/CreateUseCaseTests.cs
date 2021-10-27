using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
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
    [Collection("LogCall collection")]
    public class CreateUseCaseTests
    {
        private readonly Mock<IEqualityInformationGateway> _mockGateway;
        private readonly CreateUseCase _classUnderTest;
        private readonly Fixture _fixture;

        public CreateUseCaseTests()
        {
            _mockGateway = new Mock<IEqualityInformationGateway>();
            _classUnderTest = new CreateUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task WhenCalledReturnsResponse()
        {
            // Arrange
            var request = _fixture.Create<EqualityInformationObject>();

            var gatewayResponse = _fixture.Create<EqualityInformation>();

            _mockGateway
                .Setup(x => x.Create(It.IsAny<EqualityInformationObject>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            var response = await _classUnderTest.Execute(request).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(gatewayResponse);
        }
    }
}
