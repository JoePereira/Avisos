using Bernhoeft.GRT.Teste.Domain.Entities;

namespace Bernhoeft.GRT.Teste.Application.Responses.Commands.v1
{
    /// <summary>
    /// Response para retorno após atualização de aviso.
    /// Retorna mensagem de sucesso e o aviso atualizado.
    /// </summary>
    public class UpdateAvisoResponse
    {
        /// <summary>
        /// Mensagem de confirmação da atualização.
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Dados do aviso atualizado.
        /// </summary>
        public UpdateAvisoDto Aviso { get; set; }
    }

    /// <summary>
    /// DTO com os dados do aviso atualizado.
    /// </summary>
    public class UpdateAvisoDto
    {
        public int Id { get; set; }
        public bool Ativo { get; set; }
        public string Titulo { get; set; }
        public string MensagemAviso { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataEdicao { get; set; }

        /// <summary>
        /// Conversão implícita de AvisoEntity para UpdateAvisoDto.
        /// </summary>
        public static implicit operator UpdateAvisoDto(AvisoEntity entity) => new()
        {
            Id = entity.Id,
            Ativo = entity.Ativo,
            Titulo = entity.Titulo,
            MensagemAviso = entity.Mensagem,
            DataCriacao = entity.DataCriacao,
            DataEdicao = entity.DataEdicao
        };
    }
}
