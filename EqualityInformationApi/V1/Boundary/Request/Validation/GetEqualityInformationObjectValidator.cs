using FluentValidation;
using Hackney.Core.Validation;
using System;
using System.Linq;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class GetEqualityInformationObjectValidator : AbstractValidator<Guid>
    {
        public GetEqualityInformationObjectValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .NotEmpty();
        }
    }
}
