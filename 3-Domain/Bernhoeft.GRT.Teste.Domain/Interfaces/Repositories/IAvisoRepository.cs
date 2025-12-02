using Bernhoeft.GRT.Teste.Domain.Entities;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Core.Enums;

namespace Bernhoeft.GRT.Teste.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interface do repositório de Avisos.
    /// Todas as operações de busca retornam apenas avisos com Ativo=true (soft delete).
    /// </summary>
    public interface IAvisoRepository : IRepository<AvisoEntity>
    {
        /// <summary>
        /// Obtém todos os avisos ativos.
        /// </summary>
        Task<List<AvisoEntity>> ObterTodosAvisosAsync(TrackingBehavior tracking = TrackingBehavior.Default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém um aviso ativo específico por ID.
        /// </summary>
        Task<AvisoEntity?> ObterAvisoPorIdAsync(int id, TrackingBehavior tracking = TrackingBehavior.Default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adiciona um novo aviso ao banco de dados.
        /// </summary>
        Task<AvisoEntity> AdicionarAvisoAsync(AvisoEntity aviso, CancellationToken cancellationToken = default);

        /// <summary>
        /// Atualiza um aviso existente.
        /// </summary>
        Task<AvisoEntity> AtualizarAvisoAsync(AvisoEntity aviso, CancellationToken cancellationToken = default);

        /// <summary>
        /// Realiza soft delete de um aviso (define Ativo=false).
        /// </summary>
        Task<bool> DeletarAvisoAsync(int id, CancellationToken cancellationToken = default);
    }
}
