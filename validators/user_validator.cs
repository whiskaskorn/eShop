using FluentValidation;
using Users_DTOs;

namespace User_Validator;

public class RegistrationValidator : AbstractValidator<RegistrationUserDTO>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Поле не може бути пустим!").Length(5,30).WithMessage("Поле повинно містити від 5 до 30 символів!");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Поле не може бути пустим!").MaximumLength(255).WithMessage("Поле занадто довге!").EmailAddress().WithMessage("Некоректний формат email!");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Поле не може бути пустим!").Length(5,30).WithMessage("Поле повинно містити від 7 до 50 символів!");
    }
}

public class LoginValidator : AbstractValidator<LoginUserDTO>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Поле не може бути пустим!").Length(5,30).WithMessage("Поле повинно містити від 5 до 30 символів!");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Поле не може бути пустим!").MaximumLength(255).WithMessage("Поле занадто довге!").EmailAddress().WithMessage("Некоректний формат email!");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Поле не може бути пустим!").Length(5,30).WithMessage("Поле повинно містити від 7 до 50 символів!");
    }
}

public class PutValidator : AbstractValidator<PutUserDTO>
{
    public PutValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Поле не може бути пустим!").Length(5,30).WithMessage("Поле повинно містити від 5 до 30 символів!");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Поле не може бути пустим!").MaximumLength(255).WithMessage("Поле занадто довге!").EmailAddress().WithMessage("Некоректний формат email!");
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Поле не може бути пустим!").Length(5,30).WithMessage("Поле повинно містити від 7 до 50 символів!");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Поле не може бути пустим!").Length(5,30).WithMessage("Поле повинно містити від 7 до 50 символів!");
    }
}