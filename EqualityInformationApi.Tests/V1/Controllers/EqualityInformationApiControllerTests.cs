using AutoFixture;
using EqualityInformationApi.V1.Controllers;
using EqualityInformationApi.V1.UseCase;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Moq;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Controllers
{
    public class EqualityInformationApiControllerTests : LogCallAspectFixture
    {
        private EqualityInformationApiController _classUnderTest;
        private Mock<IGetByIdUseCase> _mockGetByIdUseCase;
        private Mock<IGetAllUseCase> _mockGetByAllUseCase;

        public EqualityInformationApiControllerTests()
        {
            _mockGetByIdUseCase = new Mock<IGetByIdUseCase>();
            _mockGetByAllUseCase = new Mock<IGetAllUseCase>();
            _classUnderTest = new EqualityInformationApiController(_mockGetByAllUseCase.Object, _mockGetByIdUseCase.Object);
        }


        //Add Tests Here
    }
}
