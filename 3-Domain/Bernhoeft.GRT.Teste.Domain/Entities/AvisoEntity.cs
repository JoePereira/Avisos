namespace Bernhoeft.GRT.Teste.Domain.Entities
{
    /// <summary>
    /// Entidade de Aviso com controle de auditoria (DataCriacao e DataEdicao) e soft delete (Ativo).
    /// </summary>
    public partial class AvisoEntity
    {
        public int Id { get; private set; }
        public bool Ativo { get; set; } = true;
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        
        /// <summary>
        /// Data de criação do aviso. Definida automaticamente no momento da criação.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Data da última edição do aviso. Atualizada automaticamente a cada modificação.
        /// </summary>
        public DateTime? DataEdicao { get; set; }
    }
}
