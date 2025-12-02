using Bernhoeft.GRT.Teste.Domain.Interfaces.Repositories;
using Bernhoeft.GRT.Core.Enums;
using Bernhoeft.GRT.Core.Interfaces.Results;
using Bernhoeft.GRT.Core.Models;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1
{
    /// <summary>
    /// Handler para atualizar um aviso existente.
    /// Decisão de design: Apenas o campo Mensagem pode ser editado, conforme regra de negócio.
    /// A DataEdicao é atualizada automaticamente.
    /// </summary>
    public class UpdateAvisoHandler : IRequestHandler<UpdateAvisoRequest, IOperationResult<UpdateAvisoResponse>>
    {
        private readonly IServiceProvider _serviceProvider;
        private IAvisoRepository _avisoRepository => _serviceProvider.GetRequiredService<IAvisoRepository>();

        public UpdateAvisoHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<IOperationResult<UpdateAvisoResponse>> Handle(UpdateAvisoRequest request, CancellationToken cancellationToken)
        {
            var aviso = await _avisoRepository.ObterAvisoPorIdAsync(request.Id, TrackingBehavior.Default, cancellationToken);

            if (aviso is null)
                return OperationResult<UpdateAvisoResponse>.ReturnNotFound();

            aviso.Mensagem = request.Mensagem;

            var result = await _avisoRepository.AtualizarAvisoAsync(aviso, cancellationToken);

            return OperationResult<UpdateAvisoResponse>.ReturnOk(new UpdateAvisoResponse
            {
                Mensagem = "Aviso atualizado com sucesso.",
                Aviso = result
            });
        }
    }
}
