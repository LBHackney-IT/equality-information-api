using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class GenderValidator : AbstractValidator<Gender>
    {
        public GenderValidator()
        {
            RuleFor(x => x.GenderValue).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.GenderValueIfOther).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.GenderDifferentToBirthSex).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
