using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class HomeSituationValidator : AbstractValidator<HomeSituation>
    {
        public HomeSituationValidator()
        {
            RuleFor(x => x.HomeSituationValue).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.HomeSituationValueIfOther).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
