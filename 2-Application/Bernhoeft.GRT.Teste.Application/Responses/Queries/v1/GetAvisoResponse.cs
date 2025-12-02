using Bernhoeft.GRT.Teste.Domain.Entities;

namespace Bernhoeft.GRT.Teste.Application.Responses.Queries.v1
{
    /// <summary>
    /// Response para retorno de um aviso específico.
    /// Inclui campos de auditoria (DataCriacao e DataEdicao).
    /// </summary>
    public class GetAvisoResponse
    {
        public int Id { get; set; }
        public bool Ativo { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataEdicao { get; set; }

        /// <summary>
        /// Conversão implícita de AvisoEntity para GetAvisoResponse.
        /// </summary>
        public static implicit operator GetAvisoResponse(AvisoEntity entity) => new()
        {
            Id = entity.Id,
            Ativo = entity.Ativo,
            Titulo = entity.Titulo,
            Mensagem = entity.Mensagem,
            DataCriacao = entity.DataCriacao,
            DataEdicao = entity.DataEdicao
        };
    }
}

