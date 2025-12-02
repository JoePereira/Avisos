using FluentValidation;

namespace Bernhoeft.GRT.Teste.Application.Requests.Commands.v1.Validations
{
    /// <summary>
    /// Validador para UpdateAvisoRequest.
    /// Regra: ID deve ser maior que zero e Mensagem é obrigatória e não pode ser vazia.
    /// </summary>
    public class UpdateAvisoRequestValidator : AbstractValidator<UpdateAvisoRequest>
    {
        public UpdateAvisoRequestValidator()
        {

            RuleFor(x => x.Mensagem)
                .NotNull()
                .WithMessage("A mensagem é obrigatória.")
                .NotEmpty()
                .WithMessage("A mensagem não pode ser vazia.");
        }
    }
}

