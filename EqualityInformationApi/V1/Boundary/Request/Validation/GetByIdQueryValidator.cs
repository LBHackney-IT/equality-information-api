using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class GetByIdQueryValidator : AbstractValidator<GetByIdQuery>
    {
        public GetByIdQueryValidator()
        {
            RuleFor(x => x.TargetId).NotNull()
                .NotEqual(Guid.Empty);

            RuleFor(x => x.Id).NotNull()
               .NotEqual(Guid.Empty);
        }
    }
}
