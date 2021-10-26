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
    public class EconomicSituationValidatorTests
    {
        private readonly EconomicSituationValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public EconomicSituationValidatorTests()
        {
            _sut = new EconomicSituationValidator();
        }

        [Fact]
        public void ShouldErrorWhenEconomicSituationValueContainsTags()
        {
            // Arrange
            var query = new EconomicSituation { EconomicSituationValue = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EconomicSituationValue)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenEconomicSituationValueIfOtherContainsTags()
        {
            // Arrange
            var query = new EconomicSituation { EconomicSituationValueIfOther = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EconomicSituationValueIfOther)
                  .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
