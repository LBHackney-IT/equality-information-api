using AutoFixture;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using FluentAssertions;
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
        }
    }
}
