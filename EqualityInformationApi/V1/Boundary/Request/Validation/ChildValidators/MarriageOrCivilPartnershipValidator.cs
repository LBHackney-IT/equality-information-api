using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class MarriageOrCivilPartnershipValidator : AbstractValidator<MarriageOrCivilPartnership>
    {
        public MarriageOrCivilPartnershipValidator()
        {
            RuleFor(x => x.Married).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.CivilPartnership).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
