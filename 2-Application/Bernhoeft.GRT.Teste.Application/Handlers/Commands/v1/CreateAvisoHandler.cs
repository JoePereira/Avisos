using Bernhoeft.GRT.Teste.Domain.Entities;
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
    /// Handler para criar um novo aviso.
    /// A DataCriacao é definida automaticamente no momento da criação.
    /// </summary>
    public class CreateAvisoHandler : IRequestHandler<CreateAvisoRequest, IOperationResult<CreateAvisoResponse>>
    {
        private readonly IServiceProvider _serviceProvider;
        private IAvisoRepository _avisoRepository => _serviceProvider.GetRequiredService<IAvisoRepository>();

        public CreateAvisoHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<IOperationResult<CreateAvisoResponse>> Handle(CreateAvisoRequest request, CancellationToken cancellationToken)
        {
            var aviso = new AvisoEntity
            {
                Titulo = request.Titulo,
                Mensagem = request.Mensagem,
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            var result = await _avisoRepository.AdicionarAvisoAsync(aviso, cancellationToken);

            return OperationResult<CreateAvisoResponse>.ReturnOk((CreateAvisoResponse)result);
        }
    }
}
