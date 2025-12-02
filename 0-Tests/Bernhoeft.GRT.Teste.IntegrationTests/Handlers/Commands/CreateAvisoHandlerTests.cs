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
    /// Testes unitários para o handler de criação de avisos.
    /// </summary>
    public class CreateAvisoHandlerTests
    {
        private readonly Mock<IAvisoRepository> _avisoRepositoryMock;
        private readonly CreateAvisoHandler _handler;

        public CreateAvisoHandlerTests()
        {
            _avisoRepositoryMock = new Mock<IAvisoRepository>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_avisoRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _handler = new CreateAvisoHandler(serviceProvider);
        }

        [Fact]
        public async Task Handle_DeveRetornarSucesso_QuandoAvisoForCriado()
        {
            // Arrange
            var request = new CreateAvisoRequest
            {
                Titulo = "Título de Teste",
                Mensagem = "Mensagem de teste para o aviso"
            };

            var avisoEsperado = new AvisoEntity
            {
                Titulo = request.Titulo,
                Mensagem = request.Mensagem,
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            _avisoRepositoryMock
                .Setup(x => x.AdicionarAvisoAsync(It.IsAny<AvisoEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AvisoEntity aviso, CancellationToken _) =>
                {
                    // Simula o comportamento do banco de dados atribuindo um ID
                    return new AvisoEntity
                    {
                        Titulo = aviso.Titulo,
                        Mensagem = aviso.Mensagem,
                        Ativo = aviso.Ativo,
                        DataCriacao = aviso.DataCriacao
                    };
                });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull("criação bem-sucedida deve retornar dados");
            result.Data!.Titulo.Should().Be(request.Titulo);
            result.Data.Mensagem.Should().Be(request.Mensagem);
            result.Data.Ativo.Should().BeTrue();

            _avisoRepositoryMock.Verify(
                x => x.AdicionarAvisoAsync(It.IsAny<AvisoEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_DeveDefinirDataCriacao_QuandoAvisoForCriado()
        {
            // Arrange
            var request = new CreateAvisoRequest
            {
                Titulo = "Título com Data",
                Mensagem = "Mensagem para verificar DataCriacao"
            };

            DateTime dataCriacaoCapturada = DateTime.MinValue;

            _avisoRepositoryMock
                .Setup(x => x.AdicionarAvisoAsync(It.IsAny<AvisoEntity>(), It.IsAny<CancellationToken>()))
                .Callback<AvisoEntity, CancellationToken>((aviso, _) => dataCriacaoCapturada = aviso.DataCriacao)
                .ReturnsAsync((AvisoEntity aviso, CancellationToken _) => aviso);

            var antesDaCriacao = DateTime.UtcNow;

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            var depoisDaCriacao = DateTime.UtcNow;

            // Assert
            dataCriacaoCapturada.Should().BeOnOrAfter(antesDaCriacao);
            dataCriacaoCapturada.Should().BeOnOrBefore(depoisDaCriacao);
        }

        [Fact]
        public async Task Handle_DeveDefinirAtivoComoTrue_QuandoAvisoForCriado()
        {
            // Arrange
            var request = new CreateAvisoRequest
            {
                Titulo = "Título Ativo",
                Mensagem = "Mensagem para verificar campo Ativo"
            };

            bool ativoCapturado = false;

            _avisoRepositoryMock
                .Setup(x => x.AdicionarAvisoAsync(It.IsAny<AvisoEntity>(), It.IsAny<CancellationToken>()))
                .Callback<AvisoEntity, CancellationToken>((aviso, _) => ativoCapturado = aviso.Ativo)
                .ReturnsAsync((AvisoEntity aviso, CancellationToken _) => aviso);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            ativoCapturado.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_DeveChamarRepositorioComDadosCorretos()
        {
            // Arrange
            var request = new CreateAvisoRequest
            {
                Titulo = "Título Específico",
                Mensagem = "Mensagem Específica"
            };

            AvisoEntity? avisoCapturado = null;

            _avisoRepositoryMock
                .Setup(x => x.AdicionarAvisoAsync(It.IsAny<AvisoEntity>(), It.IsAny<CancellationToken>()))
                .Callback<AvisoEntity, CancellationToken>((aviso, _) => avisoCapturado = aviso)
                .ReturnsAsync((AvisoEntity aviso, CancellationToken _) => aviso);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            avisoCapturado.Should().NotBeNull();
            avisoCapturado!.Titulo.Should().Be(request.Titulo);
            avisoCapturado.Mensagem.Should().Be(request.Mensagem);
        }
    }
}
