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
    public class UpdateEqualityInformationQueryValidatorTests
    {
        private readonly UpdateEqualityInformationQueryValidator _sut;

        public UpdateEqualityInformationQueryValidatorTests()
        {
            _sut = new UpdateEqualityInformationQueryValidator();
        }

        [Fact]
        public void ShouldErrorWhenIdNull()
        {
            // Arrange
            var query = new UpdateEqualityInformationQuery();

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void ShouldErrorWhenIdEmpty()
        {
            // Arrange
            var query = new UpdateEqualityInformationQuery() { Id = Guid.Empty };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}
