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
    public class EqualityInformationObjectValidatorTests
    {
        private readonly EqualityInformationObjectValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public EqualityInformationObjectValidatorTests()
        {
            _sut = new EqualityInformationObjectValidator();
        }

        // target id null
        // target id empty

        // nationality
        // xss

        // nationalInsuranceNumber
        // xss

        // communicationRequirements#
        // xss

        // armedForces
        // xss


        [Fact]
        public void ShouldErrorWhenTargetIdNull()
        {
            // Arrange
            var query = new EqualityInformationObject();

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TargetId);
        }

        [Fact]
        public void ShouldErrorWhenTargetIdEmpty()
        {
            // Arrange
            var query = new EqualityInformationObject() { TargetId = Guid.Empty };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TargetId);
        }

        [Fact]
        public void ShouldErrorWhenNationalityContainsTags()
        {
            // Arrange
            var query = new EqualityInformationObject() { Nationality = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Nationality)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenNationalInsuranceNumberContainsTags()
        {
            // Arrange
            var query = new EqualityInformationObject() { NationalInsuranceNumber = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NationalInsuranceNumber)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenCommunicationRequirementsContainsTags()
        {
            // Arrange
            var query = new EqualityInformationObject()
            {
                CommunicationRequirements = new List<string>()
            };

            query.CommunicationRequirements.Add(StringWithTags);

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CommunicationRequirements)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenArmedForcesContainsTags()
        {
            // Arrange
            var query = new EqualityInformationObject() { ArmedForces = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ArmedForces)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
