using Bernhoeft.GRT.Core.Interfaces.Results;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bernhoeft.GRT.Teste.Application.Requests.Commands.v1
{
    /// <summary>
    /// Request para deletar um aviso (soft delete).
    /// </summary>
    public class DeleteAvisoRequest : IRequest<IOperationResult<DeleteAvisoResponse>>
    {
        /// <summary>
        /// ID do aviso a ser deletado. Deve ser maior que zero.
        /// </summary>
        [FromRoute(Name = "id")]
        public int Id { get; set; }
    }
}
