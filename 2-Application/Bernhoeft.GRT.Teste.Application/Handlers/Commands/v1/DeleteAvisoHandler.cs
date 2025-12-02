using Bernhoeft.GRT.Teste.Domain.Interfaces.Repositories;
using Bernhoeft.GRT.Core.Interfaces.Results;
using Bernhoeft.GRT.Core.Models;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1
{
    /// <summary>
    /// Handler para deletar um aviso.
    /// Decisão de design: Implementa soft delete (define Ativo=false) para manter histórico e possibilitar recuperação.
    /// </summary>
    public class DeleteAvisoHandler : IRequestHandler<DeleteAvisoRequest, IOperationResult<DeleteAvisoResponse>>
    {
        private readonly IServiceProvider _serviceProvider;
        private IAvisoRepository _avisoRepository => _serviceProvider.GetRequiredService<IAvisoRepository>();

        public DeleteAvisoHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<IOperationResult<DeleteAvisoResponse>> Handle(DeleteAvisoRequest request, CancellationToken cancellationToken)
        {
            // Realiza soft delete (define Ativo=false)
            var deleted = await _avisoRepository.DeletarAvisoAsync(request.Id, cancellationToken);

            if (!deleted)
                return OperationResult<DeleteAvisoResponse>.ReturnNotFound();

            return OperationResult<DeleteAvisoResponse>.ReturnOk(new DeleteAvisoResponse
            {
                Mensagem = $"Aviso {request.Id} deletado com sucesso!"
            });
        }
    }
}
