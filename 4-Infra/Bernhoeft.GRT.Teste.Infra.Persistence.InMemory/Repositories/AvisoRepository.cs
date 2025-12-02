#nullable enable
using Bernhoeft.GRT.Teste.Domain.Entities;
using Bernhoeft.GRT.Teste.Domain.Interfaces.Repositories;
using Bernhoeft.GRT.Core.Attributes;
using Bernhoeft.GRT.Core.EntityFramework.Infra;
using Bernhoeft.GRT.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Bernhoeft.GRT.Teste.Infra.Persistence.InMemory.Repositories
{
    /// <summary>
    /// Implementação do repositório de Avisos.
    /// Decisão de design: Todas as buscas filtram por Ativo=true para suportar soft delete.
    /// </summary>
    [InjectService(Interface: typeof(IAvisoRepository))]
    public class AvisoRepository : Repository<AvisoEntity>, IAvisoRepository
    {
        public AvisoRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Obtém todos os avisos ativos (Ativo=true).
        /// </summary>
        public Task<List<AvisoEntity>> ObterTodosAvisosAsync(TrackingBehavior tracking = TrackingBehavior.Default, CancellationToken cancellationToken = default)
        {
            var query = tracking is TrackingBehavior.NoTracking ? Set.AsNoTrackingWithIdentityResolution() : Set;
            return query.Where(x => x.Ativo).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtém um aviso ativo específico por ID.
        /// </summary>
        public Task<AvisoEntity?> ObterAvisoPorIdAsync(int id, TrackingBehavior tracking = TrackingBehavior.Default, CancellationToken cancellationToken = default)
        {
            var query = tracking is TrackingBehavior.NoTracking ? Set.AsNoTrackingWithIdentityResolution() : Set;
            return query.FirstOrDefaultAsync(x => x.Id == id && x.Ativo, cancellationToken);
        }

        /// <summary>
        /// Adiciona um novo aviso ao banco de dados.
        /// </summary>
        public async Task<AvisoEntity> AdicionarAvisoAsync(AvisoEntity aviso, CancellationToken cancellationToken = default)
        {
            aviso.DataCriacao = DateTime.UtcNow;
            await Set.AddAsync(aviso, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            return aviso;
        }

        /// <summary>
        /// Atualiza um aviso existente.
        /// </summary>
        public async Task<AvisoEntity> AtualizarAvisoAsync(AvisoEntity aviso, CancellationToken cancellationToken = default)
        {
            aviso.DataEdicao = DateTime.UtcNow;
            Set.Update(aviso);
            await Context.SaveChangesAsync(cancellationToken);
            return aviso;
        }

        /// <summary>
        /// Realiza soft delete de um aviso (define Ativo=false).
        /// </summary>
        public async Task<bool> DeletarAvisoAsync(int id, CancellationToken cancellationToken = default)
        {
            var aviso = await Set.FirstOrDefaultAsync(x => x.Id == id && x.Ativo, cancellationToken);
            if (aviso is null)
                return false;

            aviso.Ativo = false;
            aviso.DataEdicao = DateTime.UtcNow;
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
