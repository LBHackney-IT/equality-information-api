using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using FluentValidation.TestHelper;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation.ChildValidators
{
    public class GenderValidatorTests
    {
        private readonly GenderValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public GenderValidatorTests()
        {
            _sut = new GenderValidator();
        }

        [Fact]
        public void ShouldErrorWhenGenderValueContainsTags()
        {
            // Arrange
            var query = new Gender { GenderValue = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.GenderValue)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenGenderValueIfOtherUntilContainsTags()
        {
            // Arrange
            var query = new Gender { GenderValueIfOther = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.GenderValueIfOther)
                  .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenGenderDifferentToBirthSexUntilContainsTags()
        {
            // Arrange
            var query = new Gender { GenderDifferentToBirthSex = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.GenderDifferentToBirthSex)
                  .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
