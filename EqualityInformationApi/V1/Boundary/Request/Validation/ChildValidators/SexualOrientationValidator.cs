using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class SexualOrientationValidator : AbstractValidator<SexualOrientation>
    {
        public SexualOrientationValidator()
        {
            RuleFor(x => x.SexualOrientationValue).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.SexualOrientationValueIfOther).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
