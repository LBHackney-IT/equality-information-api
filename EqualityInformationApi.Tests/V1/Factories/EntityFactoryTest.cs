using AutoFixture;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using FluentAssertions;
using System;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Factories
{
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void EqualityInformationDbToDomain()
        {
            // Arrange
            var entity = _fixture.Create<EqualityInformationDb>();

            // Act
            var response = entity.ToDomain();

            // Assert
            response.Id.Should().Be(entity.Id);
            response.TargetId.Should().Be(entity.TargetId);
            response.Gender.Should().Be(entity.Gender);
            response.Nationality.Should().Be(entity.Nationality);
            response.Ethnicity.Should().Be(entity.Ethnicity);
            response.ReligionOrBelief.Should().Be(entity.ReligionOrBelief);
            response.SexualOrientation.Should().Be(entity.SexualOrientation);
            response.MarriageOrCivilPartnership.Should().Be(entity.MarriageOrCivilPartnership);
            response.PregnancyOrMaternity.Should().BeEquivalentTo(entity.PregnancyOrMaternity);
            response.NationalInsuranceNumber.Should().Be(entity.NationalInsuranceNumber);
            response.Languages.Should().BeEquivalentTo(entity.Languages);
            response.CaringResponsibilities.Should().Be(entity.CaringResponsibilities);
            response.Disabled.Should().Be(entity.Disabled);
            response.CommunicationRequirements.Should().BeEquivalentTo(entity.CommunicationRequirements);
            response.EconomicSituation.Should().Be(entity.EconomicSituation);
            response.HomeSituation.Should().Be(entity.HomeSituation);
            response.ArmedForces.Should().Be(entity.ArmedForces);
            response.VersionNumber.Should().Be(entity.VersionNumber);
        }


        [Fact]
        public void EqualityInformationToDatabase()
        {
            // Arrange
            var domain = _fixture.Create<EqualityInformation>();

            // Act
            var response = domain.ToDatabase();

            // Assert
            response.Id.Should().Be(domain.Id);
            response.TargetId.Should().Be(domain.TargetId);
            response.Gender.Should().Be(domain.Gender);
            response.Nationality.Should().Be(domain.Nationality);
            response.Ethnicity.Should().Be(domain.Ethnicity);
            response.ReligionOrBelief.Should().Be(domain.ReligionOrBelief);
            response.SexualOrientation.Should().Be(domain.SexualOrientation);
            response.MarriageOrCivilPartnership.Should().Be(domain.MarriageOrCivilPartnership);
            response.PregnancyOrMaternity.Should().BeEquivalentTo(domain.PregnancyOrMaternity);
            response.NationalInsuranceNumber.Should().Be(domain.NationalInsuranceNumber);
            response.Languages.Should().BeEquivalentTo(domain.Languages);
            response.CaringResponsibilities.Should().Be(domain.CaringResponsibilities);
            response.Disabled.Should().Be(domain.Disabled);
            response.CommunicationRequirements.Should().BeEquivalentTo(domain.CommunicationRequirements);
            response.EconomicSituation.Should().Be(domain.EconomicSituation);
            response.HomeSituation.Should().Be(domain.HomeSituation);
            response.ArmedForces.Should().Be(domain.ArmedForces);
            response.VersionNumber.Should().Be(domain.VersionNumber);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void EqualityInformationObjectToDomain(bool hasId)
        {
            var id = hasId ? Guid.NewGuid() : Guid.Empty;
            // Arrange
            var request = _fixture.Build<EqualityInformationObject>()
                                  .With(x => x.TargetId, id)
                                  .Create();

            // Act
            var response = request.ToDomain();

            // Assert
            if (hasId) response.Id.Should().Be(id);
            else response.Id.Should().NotBeEmpty();

            response.TargetId.Should().Be(request.TargetId);
            response.Gender.Should().Be(request.Gender);
            response.Nationality.Should().Be(request.Nationality);
            response.Ethnicity.Should().Be(request.Ethnicity);
            response.ReligionOrBelief.Should().Be(request.ReligionOrBelief);
            response.SexualOrientation.Should().Be(request.SexualOrientation);
            response.MarriageOrCivilPartnership.Should().Be(request.MarriageOrCivilPartnership);
            response.PregnancyOrMaternity.Should().BeEquivalentTo(request.PregnancyOrMaternity);
            response.NationalInsuranceNumber.Should().Be(request.NationalInsuranceNumber);
            response.Languages.Should().BeEquivalentTo(request.Languages);
            response.CaringResponsibilities.Should().Be(request.CaringResponsibilities);
            response.Disabled.Should().Be(request.Disabled);
            response.CommunicationRequirements.Should().BeEquivalentTo(request.CommunicationRequirements);
            response.EconomicSituation.Should().Be(request.EconomicSituation);
            response.HomeSituation.Should().Be(request.HomeSituation);
            response.ArmedForces.Should().Be(request.ArmedForces);
        }
    }
}
