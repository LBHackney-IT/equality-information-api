using AutoFixture;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace EqualityInformationApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void EqualityInformationToResponse()
        {
            // Arrange
            var domain = _fixture.Create<EqualityInformation>();

            // Act
            var response = domain.ToResponse();

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
