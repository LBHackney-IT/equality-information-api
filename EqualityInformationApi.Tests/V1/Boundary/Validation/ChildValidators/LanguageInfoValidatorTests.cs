using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using FluentValidation.TestHelper;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation.ChildValidators
{
    public class LanguageInfoValidatorTests
    {
        private readonly LanguageInfoValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public LanguageInfoValidatorTests()
        {
            _sut = new LanguageInfoValidator();
        }

        [Fact]
        public void ShouldErrorWhenLanguageContainsTags()
        {
            // Arrange
            var query = new LanguageInfo { Language = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Language)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
