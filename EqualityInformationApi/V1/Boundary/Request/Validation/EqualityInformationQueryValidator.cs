using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class EqualityInformationQueryValidator : AbstractValidator<EqualityInformationQuery>
    {
        public EqualityInformationQueryValidator()
        {
            RuleFor(x => x.TargetId).NotNull()
                .NotEqual(Guid.Empty);
        }
    }
}
