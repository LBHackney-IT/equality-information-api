using AutoFixture;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EqualityInformationApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetUseCaseTests
    {
        private readonly Mock<IEqualityInformationGateway> _mockGateway;

        private readonly GetUseCase _classUnderTest;
        private readonly Fixture _fixture;
        private readonly Guid _id = Guid.NewGuid();

        public GetUseCaseTests()
        {
            _mockGateway = new Mock<IEqualityInformationGateway>();
            _classUnderTest = new GetUseCase(_mockGateway.Object);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task IdFoundReturnsResponseObject()
        {
            var gatewayResponse = _fixture.Create<EqualityInformation>();
            _mockGateway.Setup(x => x.Get(_id)).ReturnsAsync(gatewayResponse);

            // Act
            var response = await _classUnderTest.Execute(_id).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(gatewayResponse);
        }

        [Fact]
        public async Task IdNotFoundReturnsNull()
        {
            _mockGateway.Setup(x => x.Get(_id)).ReturnsAsync((EqualityInformation) null);

            // Act
            var response = await _classUnderTest.Execute(_id).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public void ThrowsException()
        {
            var exception = new ApplicationException("Test Exception");
            _mockGateway.Setup(x => x.Get(_id)).ThrowsAsync(exception);

            // Act
            Func<Task<EqualityInformation>> func = async () => await _classUnderTest.Execute(_id).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}
