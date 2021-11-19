using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.UseCase;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Moq;
using System;
using System.Collections.Generic;
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
        private const string RAWBODY = "";

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

            _mockGateway.Setup(x => x.Update(mockQuery, RAWBODY, It.IsAny<int?>()))
                        .ReturnsAsync((UpdateEntityResult<EqualityInformationDb>) null);

            var response = await _classUnderTest.Execute(mockQuery, RAWBODY, mockToken, null)
                                                .ConfigureAwait(false);

            response.Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WhenCalledReturnsResponse(bool hasChanges)
        {
            // Arrange
            var request = _fixture.Create<PatchEqualityInformationObject>();
            var token = new Token();

            var updateResponse = hasChanges? MockUpdateEntityResultWhereChangesAreMade()
                : MockUpdateEntityResultWhereNoChangesAreMade();
            var snsMessage = _fixture.Create<EntityEventSns>();

            _mockGateway.Setup(x => x.Update(request, RAWBODY, It.IsAny<int?>()))
                        .ReturnsAsync(updateResponse);
            _mockSnsFactory.Setup(x => x.Update(updateResponse, token))
                           .Returns(snsMessage);

            // Act
            var response = await _classUnderTest.Execute(request, RAWBODY, token, null)
                                                .ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(updateResponse.UpdatedEntity.ToDomain().ToResponse());

            _mockSnsFactory.Verify(x => x.Update(updateResponse, token), hasChanges? Times.Once() : Times.Never());
            _mockSnsGateway.Verify(x => x.Publish(snsMessage, ARN, It.IsAny<string>()), hasChanges ? Times.Once() : Times.Never());
        }

        private UpdateEntityResult<EqualityInformationDb> MockUpdateEntityResultWhereChangesAreMade()
        {
            return new UpdateEntityResult<EqualityInformationDb>
            {
                UpdatedEntity = _fixture.Create<EqualityInformationDb>(),
                OldValues = new Dictionary<string, object>
                {
                    { "disabled", null }
                },
                NewValues = new Dictionary<string, object>
                {
                    { "disabled", "Wheelchair" }
                }
            };
        }

        private UpdateEntityResult<EqualityInformationDb> MockUpdateEntityResultWhereNoChangesAreMade()
        {
            return new UpdateEntityResult<EqualityInformationDb>
            {
                UpdatedEntity = _fixture.Create<EqualityInformationDb>()
                // empty
            };
        }
    }
}
