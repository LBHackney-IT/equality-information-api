using EqualityInformationApi.V1.Domain;
using FluentValidation;
using Hackney.Core.Validation;

namespace EqualityInformationApi.V1.Boundary.Request.Validation
{
    public class CaringResponsibilitiesValidator : AbstractValidator<CaringResponsibilities>
    {
        public CaringResponsibilitiesValidator()
        {
            RuleFor(x => x.HoursSpentProvidingUnpaidCare).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);

            RuleFor(x => x.ProvideUnpaidCare).NotXssString()
                .WithErrorCode(ErrorCodes.XssCheckFailure);
        }
    }
}
