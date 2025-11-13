using FluentValidation;
using ContactApi.DTOs;

namespace ContactApi.Validators
{
    public class ContactDtoValidator : AbstractValidator<ContactDto>
    {
        public ContactDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("E-mail inválido.");
            RuleFor(x => x.Phone).NotEmpty().Matches(@"^\d{10,11}$").WithMessage("Telefone inválido.");
        }
    }
}
