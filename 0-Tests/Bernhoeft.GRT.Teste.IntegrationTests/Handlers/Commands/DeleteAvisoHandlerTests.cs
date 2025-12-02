using Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Handlers.Commands
{
    /// <summary>
    /// Testes unitários para o handler de exclusão de avisos (soft delete).
    /// </summary>
    public class DeleteAvisoHandlerTests
    {
        private readonly Mock<IAvisoRepository> _avisoRepositoryMock;
        private readonly DeleteAvisoHandler _handler;

        public DeleteAvisoHandlerTests()
        {
            _avisoRepositoryMock = new Mock<IAvisoRepository>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_avisoRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _handler = new DeleteAvisoHandler(serviceProvider);
        }

        [Fact]
        public async Task Handle_DeveRetornarSucesso_QuandoAvisoExistir()
        {
            // Arrange
            var avisoId = 1;
            var request = new DeleteAvisoRequest { Id = avisoId };

            _avisoRepositoryMock
                .Setup(x => x.DeletarAvisoAsync(avisoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull("deleção bem-sucedida deve retornar dados");
            result.Data!.Mensagem.Should().Contain(avisoId.ToString());
            result.Data.Mensagem.Should().Contain("deletado com sucesso");
        }

        [Fact]
        public async Task Handle_DeveRetornarNotFound_QuandoAvisoNaoExistir()
        {
            // Arrange
            var avisoId = 999;
            var request = new DeleteAvisoRequest { Id = avisoId };

            _avisoRepositoryMock
                .Setup(x => x.DeletarAvisoAsync(avisoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeNull("aviso não encontrado deve retornar Data nulo");
        }

        [Fact]
        public async Task Handle_DeveChamarDeletarAvisoAsync_ComIdCorreto()
        {
            // Arrange
            var avisoId = 42;
            var request = new DeleteAvisoRequest { Id = avisoId };

            _avisoRepositoryMock
                .Setup(x => x.DeletarAvisoAsync(avisoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _avisoRepositoryMock.Verify(
                x => x.DeletarAvisoAsync(avisoId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_DeveRetornarMensagemComId_QuandoDeletadoComSucesso()
        {
            // Arrange
            var avisoId = 123;
            var request = new DeleteAvisoRequest { Id = avisoId };

            _avisoRepositoryMock
                .Setup(x => x.DeletarAvisoAsync(avisoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Data.Should().NotBeNull();
            result.Data!.Mensagem.Should().Be($"Aviso {avisoId} deletado com sucesso!");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task Handle_DeveProcessarDiferentesIds_ComSucesso(int avisoId)
        {
            // Arrange
            var request = new DeleteAvisoRequest { Id = avisoId };

            _avisoRepositoryMock
                .Setup(x => x.DeletarAvisoAsync(avisoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Data.Should().NotBeNull("deleção bem-sucedida deve retornar dados");
            result.Data!.Mensagem.Should().Contain(avisoId.ToString());
        }
    }
}
