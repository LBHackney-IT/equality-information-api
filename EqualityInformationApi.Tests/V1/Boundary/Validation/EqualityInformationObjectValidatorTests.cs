using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Request.Validation;
using EqualityInformationApi.V1.Domain;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Boundary.Validation
{
    public class EqualityInformationObjectValidatorTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly EqualityInformationObjectValidator _sut;

        private const string StringWithTags = "Some string with <tag> in it.";

        public EqualityInformationObjectValidatorTests()
        {
            _sut = new EqualityInformationObjectValidator();
        }

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

        [Fact]
        public void ShouldErrorWhenAgeGroupContainsTags()
        {
            // Arrange
            var query = new EqualityInformationObject() { AgeGroup = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AgeGroup)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Fact]
        public void ShouldErrorWhenDisabledContainsTags()
        {
            // Arrange
            var query = new EqualityInformationObject() { Disabled = StringWithTags };

            // Act
            var result = _sut.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Disabled)
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NZ335598D")]
        [InlineData("nz335598d")]
        public void NationalInsuranceNumberShouldNotErrorWithValidValue(string value)
        {
            var model = new EqualityInformationObject() { NationalInsuranceNumber = value };
            var result = _sut.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.NationalInsuranceNumber);
        }

        [Theory]
        [InlineData("BG335598D")]
        [InlineData("bg335598d")]
        [InlineData("fghdfhfgh")]
        public void NationalInsuranceNumberShouldErrorWithInvalidValue(string invalid)
        {
            var model = new EqualityInformationObject() { NationalInsuranceNumber = invalid };
            var result = _sut.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.NationalInsuranceNumber);
        }

        [Fact]
        public void LanguagesShouldErrorWithTooMany()
        {
            var languages = _fixture.Build<LanguageInfo>()
                                    .With(x => x.IsPrimary, false)
                                    .CreateMany(10).ToList();
            languages.Add(new LanguageInfo() { Language = "Primary", IsPrimary = true });
            var model = new EqualityInformationObject() { Languages = languages };
            var result = _sut.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Languages);
        }

        [Fact]
        public void LanguagesShouldErrorWithNoPrimary()
        {
            var languages = _fixture.Build<LanguageInfo>()
                                    .With(x => x.IsPrimary, false)
                                    .CreateMany(5).ToList();
            var model = new EqualityInformationObject() { Languages = languages };
            var result = _sut.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Languages);
        }

        [Fact]
        public void LanguagesShouldErrorWithTooManyPrimary()
        {
            var languages = _fixture.Build<LanguageInfo>()
                                    .With(x => x.IsPrimary, true)
                                    .CreateMany(5).ToList();
            var model = new EqualityInformationObject() { Languages = languages };
            var result = _sut.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Languages);
        }

        [Fact]
        public void LanguagesShouldNotErrorWhenNull()
        {
            var model = new EqualityInformationObject() { Languages = null };
            var result = _sut.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Languages);
        }

        [Fact]
        public void LanguagesShouldNotErrorWhenEmpty()
        {
            var model = new EqualityInformationObject() { Languages = new List<LanguageInfo>() };
            var result = _sut.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Languages);
        }
    }
}
