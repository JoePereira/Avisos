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
    /// Testes unitários para o handler de listagem de avisos.
    /// </summary>
    public class GetAvisosHandlerTests
    {
        private readonly Mock<IAvisoRepository> _avisoRepositoryMock;
        private readonly GetAvisosHandler _handler;

        public GetAvisosHandlerTests()
        {
            _avisoRepositoryMock = new Mock<IAvisoRepository>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_avisoRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            _handler = new GetAvisosHandler(serviceProvider);
        }

        [Fact]
        public async Task Handle_DeveRetornarListaDeAvisos_QuandoExistiremAvisos()
        {
            // Arrange
            var request = new GetAvisosRequest();

            var avisos = new List<AvisoEntity>
            {
                new AvisoEntity
                {
                    Titulo = "Aviso 1",
                    Mensagem = "Mensagem 1",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow.AddDays(-2)
                },
                new AvisoEntity
                {
                    Titulo = "Aviso 2",
                    Mensagem = "Mensagem 2",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow.AddDays(-1)
                },
                new AvisoEntity
                {
                    Titulo = "Aviso 3",
                    Mensagem = "Mensagem 3",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                }
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterTodosAvisosAsync(TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisos);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull("listagem com resultados deve retornar dados");
            result.Data.Should().HaveCount(3);
        }

        [Fact]
        public async Task Handle_DeveRetornarNoContent_QuandoNaoExistiremAvisos()
        {
            // Arrange
            var request = new GetAvisosRequest();

            _avisoRepositoryMock
                .Setup(x => x.ObterTodosAvisosAsync(TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AvisoEntity>());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            // NoContent retorna sem dados
            result.Data.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task Handle_DeveBuscarComNoTracking()
        {
            // Arrange
            var request = new GetAvisosRequest();

            _avisoRepositoryMock
                .Setup(x => x.ObterTodosAvisosAsync(TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AvisoEntity>());

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _avisoRepositoryMock.Verify(
                x => x.ObterTodosAvisosAsync(TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_DeveRetornarDadosCorretos_ParaCadaAviso()
        {
            // Arrange
            var request = new GetAvisosRequest();
            var dataCriacao = DateTime.UtcNow.AddDays(-1);
            var dataEdicao = DateTime.UtcNow;

            var avisos = new List<AvisoEntity>
            {
                new AvisoEntity
                {
                    Titulo = "Título Específico",
                    Mensagem = "Mensagem Específica",
                    Ativo = true,
                    DataCriacao = dataCriacao,
                    DataEdicao = dataEdicao
                }
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterTodosAvisosAsync(TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisos);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Data.Should().NotBeNull();
            var aviso = result.Data!.First();
            aviso.Titulo.Should().Be("Título Específico");
            aviso.Mensagem.Should().Be("Mensagem Específica");
            aviso.Ativo.Should().BeTrue();
            aviso.DataCriacao.Should().Be(dataCriacao);
            aviso.DataEdicao.Should().Be(dataEdicao);
        }

        [Fact]
        public async Task Handle_DeveRetornarApenasAvisosAtivos()
        {
            // Arrange
            var request = new GetAvisosRequest();

            // O repositório já filtra apenas ativos, então retornamos uma lista simulada
            var avisosAtivos = new List<AvisoEntity>
            {
                new AvisoEntity
                {
                    Titulo = "Aviso Ativo 1",
                    Mensagem = "Mensagem 1",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                },
                new AvisoEntity
                {
                    Titulo = "Aviso Ativo 2",
                    Mensagem = "Mensagem 2",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                }
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterTodosAvisosAsync(TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisosAtivos);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Data.Should().NotBeNull();
            result.Data!.All(a => a.Ativo).Should().BeTrue();
        }

        [Fact]
        public async Task Handle_DeveRetornarListaVazia_ComoNoContent()
        {
            // Arrange
            var request = new GetAvisosRequest();

            _avisoRepositoryMock
                .Setup(x => x.ObterTodosAvisosAsync(TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AvisoEntity>());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Data.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task Handle_DeveRetornarMultiplosAvisos_ComDiferentesDatasCriacao()
        {
            // Arrange
            var request = new GetAvisosRequest();

            var avisos = new List<AvisoEntity>
            {
                new AvisoEntity
                {
                    Titulo = "Aviso Antigo",
                    Mensagem = "Mensagem",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow.AddMonths(-1)
                },
                new AvisoEntity
                {
                    Titulo = "Aviso Recente",
                    Mensagem = "Mensagem",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                }
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterTodosAvisosAsync(TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisos);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Data.Should().HaveCount(2);
            result.Data!.Select(a => a.Titulo).Should().Contain("Aviso Antigo");
            result.Data!.Select(a => a.Titulo).Should().Contain("Aviso Recente");
        }

        [Fact]
        public async Task Handle_DeveRetornarUmAviso_QuandoExistirApenas1()
        {
            // Arrange
            var request = new GetAvisosRequest();

            var avisos = new List<AvisoEntity>
            {
                new AvisoEntity
                {
                    Titulo = "Único Aviso",
                    Mensagem = "Mensagem Única",
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                }
            };

            _avisoRepositoryMock
                .Setup(x => x.ObterTodosAvisosAsync(TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(avisos);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data!.First().Titulo.Should().Be("Único Aviso");
        }
    }
}
