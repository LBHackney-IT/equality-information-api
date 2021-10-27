using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation.ChildValidators
{
    public class LanguageValidatorTests
    {
        private readonly LanguageValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public LanguageValidatorTests()
        {
            _sut = new LanguageValidator();
        }

        [Fact]
        public void ShouldErrorWhenLanguageContainsTags()
        {
            // Arrange
            var query = new Languages { Language = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Language)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
