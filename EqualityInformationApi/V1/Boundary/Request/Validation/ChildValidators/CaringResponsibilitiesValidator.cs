using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class CaringResponsibilitiesValidator : AbstractValidator<CaringResponsibilities>
    {
        public CaringResponsibilitiesValidator()
        {
            RuleFor(x => x.HoursSpentProvidingUnpaidCare).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
