using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Infrastructure;
using System;

namespace EqualityInformationApi.V1.Factories
{
    public static class EntityFactory
    {
        public static EqualityInformationDb ToDatabase(this EqualityInformation domain)
        {
            return new EqualityInformationDb
            {
                Id = domain.Id,
                TargetId = domain.TargetId,
                AgeGroup = domain.AgeGroup,
                Gender = domain.Gender,
                Nationality = domain.Nationality,
                Ethnicity = domain.Ethnicity,
                ReligionOrBelief = domain.ReligionOrBelief,
                SexualOrientation = domain.SexualOrientation,
                MarriageOrCivilPartnership = domain.MarriageOrCivilPartnership,
                PregnancyOrMaternity = domain.PregnancyOrMaternity,
                NationalInsuranceNumber = domain.NationalInsuranceNumber,
                Languages = domain.Languages,
                CaringResponsibilities = domain.CaringResponsibilities,
                Disabled = domain.Disabled,
                CommunicationRequirements = domain.CommunicationRequirements,
                EconomicSituation = domain.EconomicSituation,
                HomeSituation = domain.HomeSituation,
                ArmedForces = domain.ArmedForces,
                VersionNumber = domain.VersionNumber
            };
        }

        public static EqualityInformation ToDomain(this EqualityInformationDb entity)
        {
            return new EqualityInformation
            {
                Id = entity.Id,
                TargetId = entity.TargetId,
                AgeGroup = entity.AgeGroup,
                Gender = entity.Gender,
                Nationality = entity.Nationality,
                Ethnicity = entity.Ethnicity,
                ReligionOrBelief = entity.ReligionOrBelief,
                SexualOrientation = entity.SexualOrientation,
                MarriageOrCivilPartnership = entity.MarriageOrCivilPartnership,
                PregnancyOrMaternity = entity.PregnancyOrMaternity,
                NationalInsuranceNumber = entity.NationalInsuranceNumber,
                Languages = entity.Languages,
                CaringResponsibilities = entity.CaringResponsibilities,
                Disabled = entity.Disabled,
                CommunicationRequirements = entity.CommunicationRequirements,
                EconomicSituation = entity.EconomicSituation,
                HomeSituation = entity.HomeSituation,
                ArmedForces = entity.ArmedForces,
                VersionNumber = entity.VersionNumber
            };
        }

        public static EqualityInformation ToDomain(this EqualityInformationObject request)
        {
            return new EqualityInformation
            {
                Id = request.TargetId == Guid.Empty ? Guid.NewGuid() : request.TargetId,
                TargetId = request.TargetId,
                AgeGroup = request.AgeGroup,
                Gender = request.Gender,
                Nationality = request.Nationality,
                Ethnicity = request.Ethnicity,
                ReligionOrBelief = request.ReligionOrBelief,
                SexualOrientation = request.SexualOrientation,
                MarriageOrCivilPartnership = request.MarriageOrCivilPartnership,
                PregnancyOrMaternity = request.PregnancyOrMaternity,
                NationalInsuranceNumber = request.NationalInsuranceNumber,
                Languages = request.Languages,
                CaringResponsibilities = request.CaringResponsibilities,
                Disabled = request.Disabled,
                CommunicationRequirements = request.CommunicationRequirements,
                EconomicSituation = request.EconomicSituation,
                HomeSituation = request.HomeSituation,
                ArmedForces = request.ArmedForces
            };
        }
    }
}
