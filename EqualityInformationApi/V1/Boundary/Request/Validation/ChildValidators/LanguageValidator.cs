using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class LanguageValidator : AbstractValidator<Languages>
    {
        public LanguageValidator()
        {
            RuleFor(x => x.Language).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
