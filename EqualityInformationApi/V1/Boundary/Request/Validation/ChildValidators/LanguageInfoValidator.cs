using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class LanguageInfoValidator : AbstractValidator<LanguageInfo>
    {
        public LanguageInfoValidator()
        {
            RuleFor(x => x.Language).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
