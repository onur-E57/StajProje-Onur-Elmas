using StajProje.WebApi.Entities;
using FluentValidation;

namespace StajProje.WebApi.ValidationRules
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.ProductName).NotEmpty().WithMessage("Ürün adı boş bırakılamaz.").MinimumLength(2).WithMessage("Ürün adı en az 2 karakter olmalıdır.").MaximumLength(100).WithMessage("Ürün adı en fazla 100 karakter olmalıdır.");
        }
    }
}
