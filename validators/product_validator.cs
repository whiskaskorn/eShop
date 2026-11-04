using FluentValidation;
using Product_DTOs;

namespace Product_Validator;

public class AddValidator : AbstractValidator<PostProductDTO>
{
    public AddValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Поле не може бути пустим!").Length(3,50).WithMessage("Поле повинно містити від 3 до 50 символів!");
        RuleFor(x => x.Decription).NotEmpty().WithMessage("Поле не може бути пустим!").Length(10,255).WithMessage("Поле повинно містити від 10 до 255 символів!");
        RuleFor(x => x.Price).NotEmpty().WithMessage("Поле не може бути пустим!").GreaterThan(0).WithMessage("Ціна повинна бути більше 0");
    } 
}