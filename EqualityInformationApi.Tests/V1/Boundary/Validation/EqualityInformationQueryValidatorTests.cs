using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation
{
    public class EqualityInformationQueryValidatorTests
    {
        private readonly EqualityInformationQueryValidator _sut;

        public EqualityInformationQueryValidatorTests()
        {
            _sut = new EqualityInformationQueryValidator();
        }

        [Fact]
        public void ShouldErrorWhenTargetIdNull()
        {
            // Arrange
            var query = new EqualityInformationQuery();

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TargetId);
        }

        [Fact]
        public void ShouldErrorWhenTargetIdEmpty()
        {
            // Arrange
            var query = new EqualityInformationQuery() { TargetId = Guid.Empty };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TargetId);
        }
    }
}
