using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class EqualityInformationObjectValidator : AbstractValidator<EqualityInformationObject>
    {
        public EqualityInformationObjectValidator()
        {
            RuleFor(x => x.TargetId)
               .NotNull()
               .NotEqual(Guid.Empty);

            RuleFor(x => x.Gender).SetValidator(new GenderValidator());
            RuleFor(x => x.Ethnicity).SetValidator(new EthnicityValidator());
            RuleFor(x => x.ReligionOrBelief).SetValidator(new ReligionOrBeliefValidator());
            RuleFor(x => x.SexualOrientation).SetValidator(new SexualOrientationValidator());
            RuleForEach(x => x.PregnancyOrMaternity).SetValidator(new PregnancyOrMaternityValidator());
            RuleForEach(x => x.Languages).SetValidator(new LanguageValidator());
            RuleFor(x => x.CaringResponsibilities).SetValidator(new CaringResponsibilitiesValidator());
            RuleFor(x => x.EconomicSituation).SetValidator(new EconomicSituationValidator());
            RuleFor(x => x.HomeSituation).SetValidator(new HomeSituationValidator());

            // XSS Rules
            RuleFor(x => x.AgeGroup).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.Nationality).NotXssString()
               .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.NationalInsuranceNumber).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleForEach(x => x.CommunicationRequirements).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.ArmedForces).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
