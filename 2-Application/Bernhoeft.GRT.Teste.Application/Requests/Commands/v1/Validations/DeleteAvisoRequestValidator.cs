using FluentValidation;

namespace Bernhoeft.GRT.Teste.Application.Requests.Commands.v1.Validations
{
    /// <summary>
    /// Validador para DeleteAvisoRequest.
    /// Regra: O ID deve ser maior que zero para prevenir requisições inválidas.
    /// </summary>
    public class DeleteAvisoRequestValidator : AbstractValidator<DeleteAvisoRequest>
    {
        public DeleteAvisoRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("O ID do aviso deve ser maior que zero.");
        }
    }
}

