using ECommerce.Api.Dtos;
using FluentValidation;

namespace ECommerce.Api.Validators;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0.01m);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}
