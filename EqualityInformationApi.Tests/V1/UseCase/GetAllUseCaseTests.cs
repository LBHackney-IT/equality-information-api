using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase;
using FluentAssertions;
using Moq;
using Xunit;
using System;
using EqualityInformationApi.V1.Infrastructure;
using System.Collections.Generic;

namespace EqualityInformationApi.Tests.V1.UseCase
{
    public class GetAllUseCaseTests : LogCallAspectFixture
    {
        private readonly Mock<IEqualityInformationGateway> _mockGateway;
        private readonly GetAllUseCase _classUnderTest;
        private readonly Fixture _fixture;
        private readonly Random _random = new Random();

        public GetAllUseCaseTests()
        {
            _mockGateway = new Mock<IEqualityInformationGateway>();
            _classUnderTest = new GetAllUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task WhenNoEntitiesReturnsEmptyList()
        {
            // Arrange
            var query = _fixture.Create<EqualityInformationQuery>();

            _mockGateway
                .Setup(x => x.GetAll(It.IsAny<Guid>()))
                .ReturnsAsync(new List<EqualityInformationDb>());

            // Act
            var response = await _classUnderTest.Execute(query).ConfigureAwait(false);

            // Assert
            response.EqualityData.Should().HaveCount(0);
        }

        [Fact]
        public async Task WhenManyEntitiesExistReturnsMany()
        {
            // Arrange
            var query = _fixture.Create<EqualityInformationQuery>();
            var numberOfEntities = _random.Next(2, 5);

            var gatewayResponse = _fixture
                .Build<EqualityInformationDb>()
                .CreateMany(numberOfEntities)
                .ToList();

            _mockGateway
                .Setup(x => x.GetAll(It.IsAny<Guid>()))
                .ReturnsAsync(gatewayResponse);

            // Act
            var response = await _classUnderTest.Execute(query).ConfigureAwait(false);

            // Assert
            response.EqualityData.Should().HaveCount(numberOfEntities);
        }
    }
}
