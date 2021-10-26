using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class EconomicSituationValidator : AbstractValidator<EconomicSituation>
    {
        public EconomicSituationValidator()
        {
            RuleFor(x => x.EconomicSituationValue).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.EconomicSituationValueIfOther).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
