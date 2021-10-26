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
    public class GetByIdQueryValidatorTests
    {
        private readonly GetByIdQueryValidator _sut;

        public GetByIdQueryValidatorTests()
        {
            _sut = new GetByIdQueryValidator();
        }

        [Fact]
        public void ShouldErrorWhenIdNull()
        {
            // Arrange
            var query = new GetByIdQuery();

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void ShouldErrorWhenIdEmpty()
        {
            // Arrange
            var query = new GetByIdQuery() { Id = Guid.Empty };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void ShouldErrorWhenTargetIdNull()
        {
            // Arrange
            var query = new GetByIdQuery();

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TargetId);
        }

        [Fact]
        public void ShouldErrorWhenTargetIdEmpty()
        {
            // Arrange
            var query = new GetByIdQuery() { TargetId = Guid.Empty };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TargetId);
        }
    }
}
