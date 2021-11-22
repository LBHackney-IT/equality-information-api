using FluentValidation;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class PatchEqualityInformationRequestValidator : AbstractValidator<PatchEqualityInformationRequest>
    {
        public PatchEqualityInformationRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEmpty();
        }
    }
}
