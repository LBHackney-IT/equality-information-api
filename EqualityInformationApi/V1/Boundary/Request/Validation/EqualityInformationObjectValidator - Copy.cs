using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Linq;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class PatchEqualityInformationObjectValidator : AbstractValidator<PatchEqualityInformationObject>
    {
        public PatchEqualityInformationObjectValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.TargetId)
               .NotNull()
               .NotEqual(Guid.Empty);

            RuleFor(x => x.NationalInsuranceNumber)
                     .IsNationalInsuranceNumber()
                     .When(x => !string.IsNullOrEmpty(x.NationalInsuranceNumber));

            RuleFor(x => x.Gender).SetValidator(new GenderValidator());
            RuleFor(x => x.Ethnicity).SetValidator(new EthnicityValidator());
            RuleFor(x => x.ReligionOrBelief).SetValidator(new ReligionOrBeliefValidator());
            RuleFor(x => x.SexualOrientation).SetValidator(new SexualOrientationValidator());
            RuleForEach(x => x.PregnancyOrMaternity).SetValidator(new PregnancyOrMaternityValidator());
            RuleForEach(x => x.Languages).SetValidator(new LanguageInfoValidator());
            RuleFor(x => x.MarriageOrCivilPartnership).SetValidator(new MarriageOrCivilPartnershipValidator());
            RuleFor(x => x.CaringResponsibilities).SetValidator(new CaringResponsibilitiesValidator());
            RuleFor(x => x.EconomicSituation).SetValidator(new EconomicSituationValidator());
            RuleFor(x => x.HomeSituation).SetValidator(new HomeSituationValidator());

            RuleFor(x => x.Languages).Must(x => x.Count < 10)
                         .WithMessage("Please only enter up to 10 languages")
                         .WithErrorCode(ErrorCodes.TooManyLanguages)
                         .When(x => x.Languages != null);
            RuleFor(x => x.Languages).Must(x => x.Count(y => y.IsPrimary) == 1)
                                     .WithMessage("You must choose one language as the primary")
                                     .WithErrorCode(ErrorCodes.OnePrimaryLanguage)
                                     .When(x => (x.Languages != null) && x.Languages.Any());

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

            RuleFor(x => x.Disabled).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
