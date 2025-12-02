using Bernhoeft.GRT.Core.Interfaces.Results;
using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bernhoeft.GRT.Teste.Application.Requests.Queries.v1
{
    /// <summary>
    /// Request para buscar um aviso específico por ID.
    /// </summary>
    public class GetAvisoRequest : IRequest<IOperationResult<GetAvisoResponse>>
    {
        /// <summary>
        /// ID do aviso a ser buscado. Deve ser maior que zero.
        /// </summary>
        [FromRoute(Name = "id")]
        public int Id { get; set; }
    }
}


