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
    public class HomeSituationValidatorTests
    {
        private readonly HomeSituationValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public HomeSituationValidatorTests()
        {
            _sut = new HomeSituationValidator();
        }

        [Fact]
        public void ShouldErrorWhenHomeSituationValueContainsTags()
        {
            // Arrange
            var query = new HomeSituation { HomeSituationValue = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.HomeSituationValue)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenHomeSituationValueIfOtherUntilContainsTags()
        {
            // Arrange
            var query = new HomeSituation { HomeSituationValueIfOther = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.HomeSituationValueIfOther)
                  .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
