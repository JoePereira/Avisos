using Bernhoeft.GRT.Core.Enums;
using Bernhoeft.GRT.Teste.Application.Handlers.Queries.v1;
using Bernhoeft.GRT.Teste.Application.Requests.Queries.v1;
using Bernhoeft.GRT.Teste.Domain.Entities;
using Bernhoeft.GRT.Teste.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Handlers.Queries
{
    /// <summary>
    /// Testes unitários para o handler de busca de aviso por ID.
    /// </summary>
    public class GetAvisoHandlerTests
    {
        private readonly Mock<IAvisoRepository> _avisoRepositoryMock;
        private readonly GetAvisoHandler _handler;

        public GetAvisoHandlerTests()
        {
            _avisoRepositoryMock = new Mock<IAvisoRepository>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_avisoRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _handler = new GetAvisoHandler(serviceProvider);
        }

        [Fact]
        public async Task Handle_DeveRetornarAviso_QuandoAvisoExistir()
        {
            // Arrange
            var avisoId = 1;
            var request = new GetAvisoRequest { Id = avisoId };

            var avisoExistente = new AvisoEntity
            {
                Titulo = "Título do Aviso",
                Mensagem = "Mensagem do Aviso",
                Ativo = true,
                DataCriacao = DateTime.UtcNow.AddDays(-1),
                DataEdicao = DateTime.UtcNow
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(avisoId, TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisoExistente);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull("busca bem-sucedida deve retornar dados");
            result.Data!.Titulo.Should().Be(avisoExistente.Titulo);
            result.Data.Mensagem.Should().Be(avisoExistente.Mensagem);
            result.Data.Ativo.Should().Be(avisoExistente.Ativo);
            result.Data.DataCriacao.Should().Be(avisoExistente.DataCriacao);
            result.Data.DataEdicao.Should().Be(avisoExistente.DataEdicao);
        }

        [Fact]
        public async Task Handle_DeveRetornarNotFound_QuandoAvisoNaoExistir()
        {
            // Arrange
            var avisoId = 999;
            var request = new GetAvisoRequest { Id = avisoId };

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(avisoId, TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AvisoEntity?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeNull("aviso não encontrado deve retornar Data nulo");
        }

        [Fact]
        public async Task Handle_DeveBuscarComNoTracking()
        {
            // Arrange
            var avisoId = 1;
            var request = new GetAvisoRequest { Id = avisoId };

            var avisoExistente = new AvisoEntity
            {
                Titulo = "Título",
                Mensagem = "Mensagem",
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(avisoId, TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisoExistente);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _avisoRepositoryMock.Verify(
                x => x.ObterAvisoPorIdAsync(avisoId, TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_DeveRetornarDadosCompletos_QuandoAvisoTiverDataEdicao()
        {
            // Arrange
            var avisoId = 1;
            var request = new GetAvisoRequest { Id = avisoId };
            var dataEdicao = DateTime.UtcNow;

            var avisoExistente = new AvisoEntity
            {
                Titulo = "Aviso Editado",
                Mensagem = "Mensagem Editada",
                Ativo = true,
                DataCriacao = DateTime.UtcNow.AddDays(-2),
                DataEdicao = dataEdicao
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(avisoId, TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisoExistente);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Data.Should().NotBeNull();
            result.Data!.DataEdicao.Should().NotBeNull();
            result.Data.DataEdicao.Should().Be(dataEdicao);
        }

        [Fact]
        public async Task Handle_DeveRetornarDataEdicaoNula_QuandoAvisoNuncaFoiEditado()
        {
            // Arrange
            var avisoId = 1;
            var request = new GetAvisoRequest { Id = avisoId };

            var avisoExistente = new AvisoEntity
            {
                Titulo = "Aviso Novo",
                Mensagem = "Mensagem Nova",
                Ativo = true,
                DataCriacao = DateTime.UtcNow,
                DataEdicao = null
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(avisoId, TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisoExistente);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Data.Should().NotBeNull();
            result.Data!.DataEdicao.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(9999)]
        public async Task Handle_DeveBuscarPorIdCorreto(int avisoId)
        {
            // Arrange
            var request = new GetAvisoRequest { Id = avisoId };

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(avisoId, TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AvisoEntity?)null);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _avisoRepositoryMock.Verify(
                x => x.ObterAvisoPorIdAsync(avisoId, TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
