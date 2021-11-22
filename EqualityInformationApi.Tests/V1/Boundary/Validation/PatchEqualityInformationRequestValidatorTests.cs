using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation
{
    public class PatchEqualityInformationRequestValidatorTests
    {
        private readonly PatchEqualityInformationRequestValidator _sut;

        public PatchEqualityInformationRequestValidatorTests()
        {
            _sut = new PatchEqualityInformationRequestValidator();
        }

        [Fact]
        public void ShouldErrorWhenIdNull()
        {
            // Arrange
            var query = new PatchEqualityInformationRequest();

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void ShouldErrorWhenIdEmpty()
        {
            // Arrange
            var query = new PatchEqualityInformationRequest() { Id = Guid.Empty };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void ShouldNotErrorWhenIdValid()
        {
            // Arrange
            var query = new PatchEqualityInformationRequest() { Id = Guid.NewGuid() };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}
