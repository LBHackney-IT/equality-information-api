using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Domain;

namespace EqualityInformationApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static EqualityInformationResponseObject ToResponse(this EqualityInformation domain)
        {
            return new EqualityInformationResponseObject
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
                ArmedForces = domain.ArmedForces
            };
        }
    }
}
