using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using FluentValidation.TestHelper;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation.ChildValidators
{
    public class MarriageOrCivilPartnershipValidatorTests
    {
        private readonly MarriageOrCivilPartnershipValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public MarriageOrCivilPartnershipValidatorTests()
        {
            _sut = new MarriageOrCivilPartnershipValidator();
        }

        [Fact]
        public void ShouldErrorWhenMarriedContainsTags()
        {
            // Arrange
            var query = new MarriageOrCivilPartnership { Married = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Married)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenCivilPartnershipContainsTags()
        {
            // Arrange
            var query = new MarriageOrCivilPartnership { CivilPartnership = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CivilPartnership)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
