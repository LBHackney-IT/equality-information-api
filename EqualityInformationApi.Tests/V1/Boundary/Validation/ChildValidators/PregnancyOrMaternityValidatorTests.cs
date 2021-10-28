using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using FluentValidation.TestHelper;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation.ChildValidators
{
    public class PregnancyOrMaternityValidatorTests
    {
        private readonly PregnancyOrMaternityValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public PregnancyOrMaternityValidatorTests()
        {
            _sut = new PregnancyOrMaternityValidator();
        }

        [Fact]
        public void ShouldErrorWhenPregnancyDateContainsTags()
        {
            // Arrange
            var query = new PregnancyOrMaternity { PregnancyDate = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PregnancyDate)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenPregnancyValidUntilContainsTags()
        {
            // Arrange
            var query = new PregnancyOrMaternity { PregnancyValidUntil = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PregnancyValidUntil)
                  .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
