using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Requests.Queries.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;

namespace Bernhoeft.GRT.Teste.Api.Controllers.v1
{
    /// <summary>
    /// Controller para gerenciamento de Avisos.
    /// Implementa CRUD completo com soft delete e validações via FluentValidation.
    /// </summary>
    /// <response code="401">Não Autenticado.</response>
    /// <response code="403">Não Autorizado.</response>
    /// <response code="500">Erro Interno no Servidor.</response>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = null)]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = null)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = null)]
    public class AvisosController : RestApiController
    {
        /// <summary>
        /// Retorna um Aviso específico por ID.
        /// </summary>
        /// <param name="request">Request com o ID do aviso.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Aviso encontrado.</returns>
        /// <response code="200">Sucesso.</response>
        /// <response code="400">Dados Inválidos (ID menor ou igual a zero).</response>
        /// <response code="404">Aviso Não Encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDocumentationRestResult<GetAvisoResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAviso([FromModel] GetAvisoRequest request, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(request, cancellationToken);

            if ((int)result.StatusCode == 404)
                return NotFound(new { Mensagem = "Aviso Não Encontrado." });

            return Ok(result);
        }


        /// <summary>
        /// Retorna Todos os Avisos Ativos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Lista com Todos os Avisos ativos.</returns>
        /// <response code="200">Sucesso.</response>
        /// <response code="204">Sem Avisos.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDocumentationRestResult<IEnumerable<GetAvisosResponse>>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAvisos(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetAvisosRequest(), cancellationToken);

            if ((int)result.StatusCode == 204)
                return Ok(new { Mensagem = "Sem Avisos." });

            return Ok(result);
        }

        /// <summary>
        /// Cria um novo Aviso.
        /// </summary>
        /// <param name="request">Request com título e mensagem do aviso.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Aviso criado.</returns>
        /// <response code="201">Aviso criado com sucesso.</response>
        /// <response code="400">Dados Inválidos (título ou mensagem vazios).</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IDocumentationRestResult<CreateAvisoResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<object> CreateAviso([FromBody] CreateAvisoRequest request, CancellationToken cancellationToken)
            => await Mediator.Send(request, cancellationToken);

        /// <summary>
        /// Atualiza a mensagem de um Aviso existente.
        /// Decisão de design: Apenas o campo Mensagem pode ser editado, conforme regra de negócio.
        /// </summary>
        /// <param name="id">ID do aviso a ser atualizado.</param>
        /// <param name="request">Request com a nova mensagem.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Aviso atualizado.</returns>
        /// <response code="200">Aviso atualizado com sucesso.</response>
        /// <response code="400">Dados Inválidos (ID inválido ou mensagem vazia).</response>
        /// <response code="404">Aviso Não Encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDocumentationRestResult<UpdateAvisoResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateAviso([FromRoute] int id, [FromBody] UpdateAvisoRequest request, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest(new { Mensagem = "Dados Inválidos (ID menor ou igual a zero)." });

            request.Id = id;

            var result = await Mediator.Send(request, cancellationToken);

            if ((int)result.StatusCode == 404)
                return NotFound(new { Mensagem = "Aviso Não Encontrado." });

            return Ok(await Mediator.Send(request, cancellationToken));
        }
        /// <summary>
        /// Remove um Aviso (Soft Delete).
        /// Decisão de design: O aviso não é excluído fisicamente, apenas marcado como inativo (Ativo=false).
        /// </summary>
        /// <param name="request">Request com o ID do aviso.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Aviso removido com sucesso.</response>
        /// <response code="400">Dados Inválidos (ID menor ou igual a zero).</response>
        /// <response code="404">Aviso Não Encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAviso([FromModel] DeleteAvisoRequest request, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(request, cancellationToken);

            if ((int)result.StatusCode == 404)
                return NotFound(new { Mensagem = "Aviso Não Encontrado." });

            return Ok(result);
        }
    }
}

