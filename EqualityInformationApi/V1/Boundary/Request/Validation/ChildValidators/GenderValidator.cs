using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        }
    }
}
