using Bernhoeft.GRT.Core.Enums;
using Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Domain.Entities;
using Bernhoeft.GRT.Teste.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Handlers.Commands
{
    /// <summary>
    /// Testes unitários para o handler de atualização de avisos.
    /// </summary>
    public class UpdateAvisoHandlerTests
    {
        private readonly Mock<IAvisoRepository> _avisoRepositoryMock;
        private readonly UpdateAvisoHandler _handler;

        public UpdateAvisoHandlerTests()
        {
            _avisoRepositoryMock = new Mock<IAvisoRepository>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_avisoRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _handler = new UpdateAvisoHandler(serviceProvider);
        }

        [Fact]
        public async Task Handle_DeveRetornarSucesso_QuandoAvisoExistir()
        {
            // Arrange
            var avisoId = 1;
            var novaMensagem = "Nova mensagem atualizada";

            var request = new UpdateAvisoRequest
            {
                Id = avisoId,
                Mensagem = novaMensagem
            };

            var avisoExistente = new AvisoEntity
            {
                Titulo = "Título Original",
                Mensagem = "Mensagem Original",
                Ativo = true,
                DataCriacao = DateTime.UtcNow.AddDays(-1)
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(avisoId, It.IsAny<TrackingBehavior>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisoExistente);

            _avisoRepositoryMock
                .Setup(x => x.AtualizarAvisoAsync(It.IsAny<AvisoEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AvisoEntity aviso, CancellationToken _) => aviso);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull("atualização bem-sucedida deve retornar dados");
            result.Data!.Mensagem.Should().Be("Aviso atualizado com sucesso.");
            result.Data.Aviso.Should().NotBeNull();
            result.Data.Aviso.MensagemAviso.Should().Be(novaMensagem);
        }

        [Fact]
        public async Task Handle_DeveRetornarNotFound_QuandoAvisoNaoExistir()
        {
            // Arrange
            var request = new UpdateAvisoRequest
            {
                Id = 999,
                Mensagem = "Mensagem qualquer"
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(999, It.IsAny<TrackingBehavior>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AvisoEntity?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeNull("aviso não encontrado deve retornar Data nulo");
        }

        [Fact]
        public async Task Handle_DeveAtualizarApenasMensagem_ManterOutrosCampos()
        {
            // Arrange
            var avisoId = 1;
            var tituloOriginal = "Título Original Inalterado";
            var dataCriacaoOriginal = DateTime.UtcNow.AddDays(-5);

            var request = new UpdateAvisoRequest
            {
                Id = avisoId,
                Mensagem = "Nova mensagem"
            };

            var avisoExistente = new AvisoEntity
            {
                Titulo = tituloOriginal,
                Mensagem = "Mensagem Original",
                Ativo = true,
                DataCriacao = dataCriacaoOriginal
            };

            AvisoEntity? avisoAtualizado = null;

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(avisoId, It.IsAny<TrackingBehavior>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisoExistente);

            _avisoRepositoryMock
                .Setup(x => x.AtualizarAvisoAsync(It.IsAny<AvisoEntity>(), It.IsAny<CancellationToken>()))
                .Callback<AvisoEntity, CancellationToken>((aviso, _) => avisoAtualizado = aviso)
                .ReturnsAsync((AvisoEntity aviso, CancellationToken _) => aviso);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            avisoAtualizado.Should().NotBeNull();
            avisoAtualizado!.Titulo.Should().Be(tituloOriginal);
            avisoAtualizado.Mensagem.Should().Be(request.Mensagem);
            avisoAtualizado.DataCriacao.Should().Be(dataCriacaoOriginal);
            avisoAtualizado.Ativo.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_DeveChamarAtualizarAvisoAsync_QuandoAvisoExistir()
        {
            // Arrange
            var avisoId = 1;
            var request = new UpdateAvisoRequest
            {
                Id = avisoId,
                Mensagem = "Mensagem Atualizada"
            };

            var avisoExistente = new AvisoEntity
            {
                Titulo = "Título",
                Mensagem = "Mensagem",
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(avisoId, It.IsAny<TrackingBehavior>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisoExistente);

            _avisoRepositoryMock
                .Setup(x => x.AtualizarAvisoAsync(It.IsAny<AvisoEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AvisoEntity aviso, CancellationToken _) => aviso);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _avisoRepositoryMock.Verify(
                x => x.AtualizarAvisoAsync(It.IsAny<AvisoEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_NaoDeveChamarAtualizarAvisoAsync_QuandoAvisoNaoExistir()
        {
            // Arrange
            var request = new UpdateAvisoRequest
            {
                Id = 999,
                Mensagem = "Mensagem"
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterAvisoPorIdAsync(999, It.IsAny<TrackingBehavior>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AvisoEntity?)null);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _avisoRepositoryMock.Verify(
                x => x.AtualizarAvisoAsync(It.IsAny<AvisoEntity>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
