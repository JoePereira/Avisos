using Bernhoeft.GRT.Core.Interfaces.Results;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;
using MediatR;

namespace Bernhoeft.GRT.Teste.Application.Requests.Commands.v1
{
    /// <summary>
    /// Request para criar um novo aviso.
    /// </summary>
    public class CreateAvisoRequest : IRequest<IOperationResult<CreateAvisoResponse>>
    {
        /// <summary>
        /// Título do aviso. Obrigatório e não pode ser vazio.
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Mensagem do aviso. Obrigatória e não pode ser vazia.
        /// </summary>
        public string Mensagem { get; set; }
    }
}


