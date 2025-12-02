using Bernhoeft.GRT.Core.Interfaces.Results;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Bernhoeft.GRT.Teste.Application.Requests.Commands.v1
{
    /// <summary>
    /// Request para atualizar um aviso existente.
    /// Decisão de design: Apenas o campo Mensagem pode ser editado, conforme regra de negócio.
    /// </summary>
    public class UpdateAvisoRequest : IRequest<IOperationResult<UpdateAvisoResponse>>
    {
        /// <summary>
        /// ID do aviso a ser atualizado.
        /// </summary>
        [FromRoute(Name = "id")]
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Nova mensagem do aviso. Obrigatória e não pode ser vazia.
        /// </summary>
        public string Mensagem { get; set; }
    }
}


