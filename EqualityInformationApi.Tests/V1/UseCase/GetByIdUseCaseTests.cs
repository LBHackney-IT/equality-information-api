using EqualityInformationApi.V1.Gateways;
using EqualityInformationApi.V1.UseCase;
using Moq;
using Xunit;

namespace EqualityInformationApi.Tests.V1.UseCase
{
    public class GetByIdUseCaseTests : LogCallAspectFixture
    {
        private Mock<IExampleDynamoGateway> _mockGateway;
        private GetByIdUseCase _classUnderTest;

        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<IExampleDynamoGateway>();
            _classUnderTest = new GetByIdUseCase(_mockGateway.Object);
        }

        //TODO: test to check that the use case retrieves the correct record from the database.
        //Guidance on unit testing and example of mocking can be found here https://github.com/LBHackney-IT/lbh-equality-information-api/wiki/Writing-Unit-Tests
    }
}
