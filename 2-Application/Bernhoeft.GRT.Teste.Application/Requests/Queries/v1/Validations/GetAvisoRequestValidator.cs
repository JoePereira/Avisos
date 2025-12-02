using FluentValidation;

namespace Bernhoeft.GRT.Teste.Application.Requests.Queries.v1.Validations
{
    /// <summary>
    /// Validador para GetAvisoRequest.
    /// Regra: O ID deve ser maior que zero para prevenir requisições inválidas.
    /// </summary>
    public class GetAvisoRequestValidator : AbstractValidator<GetAvisoRequest>
    {
        public GetAvisoRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("O ID do aviso deve ser maior que zero.");
        }
    }
}

