using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Infrastructure;
using System;

namespace EqualityInformationApi.V1.Factories
{
    public static class EntityFactory
    {
        public static EqualityInformation ToDomain(this EqualityInformationObject request, Guid? id = null)
        {
            return new EqualityInformation
            {
                Id = id ?? Guid.NewGuid(),
                TargetId = request.TargetId,
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

        public static EqualityInformationDb ToDatabase(this EqualityInformation domain)
        {
            return new EqualityInformationDb
            {
                Id = domain.Id,
                TargetId = domain.TargetId,
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
                ArmedForces = domain.ArmedForces
            };
        }
    }
}
