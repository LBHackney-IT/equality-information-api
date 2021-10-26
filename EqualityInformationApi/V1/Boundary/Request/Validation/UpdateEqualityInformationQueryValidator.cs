using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class UpdateEqualityInformationQueryValidator : AbstractValidator<UpdateEqualityInformationQuery>
    {
        public UpdateEqualityInformationQueryValidator()
        {
            RuleFor(x => x.Id).NotNull()
               .NotEqual(Guid.Empty);
        }
    }
}
