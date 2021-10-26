using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class ReligionOrBeliefValidator : AbstractValidator<ReligionOrBelief>
    {
        public ReligionOrBeliefValidator()
        {
            RuleFor(x => x.ReligionOrBeliefValue).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.ReligionOrBeliefValueIfOther).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
