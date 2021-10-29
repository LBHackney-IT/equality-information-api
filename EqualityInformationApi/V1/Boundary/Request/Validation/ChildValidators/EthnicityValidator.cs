using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class EthnicityValidator : AbstractValidator<Ethnicity>
    {
        public EthnicityValidator()
        {
            RuleFor(x => x.EthnicGroupValue).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.EthnicGroupValueIfOther).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
