using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using FluentValidation.TestHelper;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation.ChildValidators
{
    public class EthnicityValidatorTests
    {
        private readonly EthnicityValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public EthnicityValidatorTests()
        {
            _sut = new EthnicityValidator();
        }

        [Fact]
        public void ShouldErrorWhenEthnicGroupValueContainsTags()
        {
            // Arrange
            var query = new Ethnicity { EthnicGroupValue = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EthnicGroupValue)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenEthnicGroupValueIfOtherContainsTags()
        {
            // Arrange
            var query = new Ethnicity { EthnicGroupValueIfOther = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EthnicGroupValueIfOther)
                  .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
