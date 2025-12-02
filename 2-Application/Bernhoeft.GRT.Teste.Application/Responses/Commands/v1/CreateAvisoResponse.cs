using Bernhoeft.GRT.Teste.Domain.Entities;

namespace Bernhoeft.GRT.Teste.Application.Responses.Commands.v1
{
    /// <summary>
    /// Response para retorno após criação de aviso.
    /// Retorna o aviso criado com todos os campos, incluindo ID gerado e DataCriacao.
    /// </summary>
    public class CreateAvisoResponse
    {
        public int Id { get; set; }
        public bool Ativo { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Conversão implícita de AvisoEntity para CreateAvisoResponse.
        /// </summary>
        public static implicit operator CreateAvisoResponse(AvisoEntity entity) => new()
        {
            Id = entity.Id,
            Ativo = entity.Ativo,
            Titulo = entity.Titulo,
            Mensagem = entity.Mensagem,
            DataCriacao = entity.DataCriacao
        };
    }
}

