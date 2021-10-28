using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class PregnancyOrMaternityValidator : AbstractValidator<PregnancyOrMaternity>
    {
        public PregnancyOrMaternityValidator()
        {
            RuleFor(x => x.PregnancyDate).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.PregnancyValidUntil).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
