using Bernhoeft.GRT.Teste.Domain.Interfaces.Repositories;
using Bernhoeft.GRT.Core.Enums;
using Bernhoeft.GRT.Core.Interfaces.Results;
using Bernhoeft.GRT.Core.Models;
using Bernhoeft.GRT.Teste.Application.Requests.Queries.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bernhoeft.GRT.Teste.Application.Handlers.Queries.v1
{
    /// <summary>
    /// Handler para buscar um aviso específico por ID.
    /// Retorna apenas avisos ativos (soft delete).
    /// </summary>
    public class GetAvisoHandler : IRequestHandler<GetAvisoRequest, IOperationResult<GetAvisoResponse>>
    {
        private readonly IServiceProvider _serviceProvider;
        private IAvisoRepository _avisoRepository => _serviceProvider.GetRequiredService<IAvisoRepository>();

        public GetAvisoHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<IOperationResult<GetAvisoResponse>> Handle(GetAvisoRequest request, CancellationToken cancellationToken)
        {
            var aviso = await _avisoRepository.ObterAvisoPorIdAsync(request.Id, TrackingBehavior.NoTracking, cancellationToken);
            
            if (aviso is null)
                return OperationResult<GetAvisoResponse>.ReturnNotFound();

            return OperationResult<GetAvisoResponse>.ReturnOk((GetAvisoResponse)aviso);
        }
    }
}
