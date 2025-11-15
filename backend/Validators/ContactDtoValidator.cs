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
            // Accept generic E.164 format with optional spaces/hyphens: +<country><digits>
            // Ex.: +5581986941088, +1 212 555 1234, +351 912 345 678
            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^\+\d(?:[\s-]?\d){7,14}$")
                .WithMessage("Telefone inválido. Use o formato internacional (E.164), ex.: +5581986941088.");
        }
    }
}
