using System.Collections.Generic;
using System.Linq;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Infrastructure;

namespace EqualityInformationApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static EqualityInformation ToDomain(this EqualityInformationDb entity)
        {
            return new EqualityInformation
            {
                Id = entity.Id,
                TargetId = entity.TargetId,
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
                ArmedForces = entity.ArmedForces
            };
        }

        public static EqualityInformationResponseObject ToResponse(this EqualityInformation domain)
        {
            return new EqualityInformationResponseObject
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

        public static IEnumerable<EqualityInformation> ToDomain(this IEnumerable<EqualityInformationDb> entity)
        {
            return entity.Select(x => x.ToDomain());
        }

        public static GetAllResponseObject ToResponse(this IEnumerable<EqualityInformation> domain)
        {
            return new GetAllResponseObject
            {
                EqualityData = domain.Select(x => x.ToResponse()).ToList()
            };
        }
    }
}
