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
    public class CaringResponsibilitiesValidatorTests
    {
        private readonly CaringResponsibilitiesValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public CaringResponsibilitiesValidatorTests()
        {
            _sut = new CaringResponsibilitiesValidator();
        }

        [Fact]
        public void ShouldErrorWhenHoursSpentProvidingUnpaidCareContainsTags()
        {
            // Arrange
            var query = new CaringResponsibilities { HoursSpentProvidingUnpaidCare = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.HoursSpentProvidingUnpaidCare)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
