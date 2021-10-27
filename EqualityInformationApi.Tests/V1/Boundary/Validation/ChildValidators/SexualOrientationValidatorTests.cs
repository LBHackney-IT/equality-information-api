using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using FluentValidation.TestHelper;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation.ChildValidators
{
    public class SexualOrientationValidatorTests
    {
        private readonly SexualOrientationValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public SexualOrientationValidatorTests()
        {
            _sut = new SexualOrientationValidator();
        }

        [Fact]
        public void ShouldErrorWhenSexualOrientationValueContainsTags()
        {
            // Arrange
            var query = new SexualOrientation { SexualOrientationValue = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SexualOrientationValue)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenSexualOrientationValueIfOtherContainsTags()
        {
            // Arrange
            var query = new SexualOrientation { SexualOrientationValueIfOther = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SexualOrientationValueIfOther)
                  .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
