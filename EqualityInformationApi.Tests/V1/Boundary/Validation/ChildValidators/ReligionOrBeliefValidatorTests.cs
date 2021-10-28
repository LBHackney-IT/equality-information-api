using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using FluentValidation.TestHelper;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation.ChildValidators
{
    public class ReligionOrBeliefValidatorTests
    {
        private readonly ReligionOrBeliefValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public ReligionOrBeliefValidatorTests()
        {
            _sut = new ReligionOrBeliefValidator();
        }

        [Fact]
        public void ShouldErrorWhenReligionOrBeliefValueContainsTags()
        {
            // Arrange
            var query = new ReligionOrBelief { ReligionOrBeliefValue = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ReligionOrBeliefValue)
              .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenReligionOrBeliefValueIfOtherContainsTags()
        {
            // Arrange
            var query = new ReligionOrBelief { ReligionOrBeliefValueIfOther = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ReligionOrBeliefValueIfOther)
                  .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
