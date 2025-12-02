# Bernhoeft GRT - API de Avisos

## Desafio Técnico: Modificações na API de Avisos

Esta documentação descreve as implementações realizadas para o desafio técnico Bernhoeft GRT.

---

## Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- **.NET 9.0 SDK** (ou superior)
  - Verifique sua versão: `dotnet --version`
  - Download: https://dotnet.microsoft.com/download/dotnet/9.0

---

## Início Rápido

### 1. Clonar o Repositório

```bash
git clone <url-do-repositorio>
cd Avisos
```

### 2. Restaurar Dependências

```bash
dotnet restore
```

### 3. Compilar o Projeto

```bash
dotnet build
```

### 4. Executar a API

```bash
dotnet run --project 1-Presentation/Bernhoeft.GRT.Teste.Api
```

### 5. Acessar a Aplicação

| Protocolo | URL |
|-----------|-----|
| HTTP | http://localhost:5000 |
| HTTPS | https://localhost:5001 |

> **Swagger UI** disponível na raiz: https://localhost:5001/

---

## Executar os Testes

### Executar todos os testes

```bash
dotnet test
```

### Executar com detalhes

```bash
dotnet test --verbosity normal
```

### Executar testes específicos

```bash
# Testes de criação
dotnet test --filter "FullyQualifiedName~CreateAvisoHandler"

# Testes de leitura
dotnet test --filter "FullyQualifiedName~GetAvisoHandler"

# Testes de atualização
dotnet test --filter "FullyQualifiedName~UpdateAvisoHandler"

# Testes de exclusão
dotnet test --filter "FullyQualifiedName~DeleteAvisoHandler"
```

### Gerar relatório de cobertura (opcional)

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## Solução de Problemas

### Erro: "Duplicar atributo" durante o build

**Causa**: Arquivos de build antigos conflitando com os novos.

**Solução**:

```powershell
# Windows PowerShell
Get-ChildItem -Path . -Include Build,obj,bin -Recurse -Directory | Remove-Item -Recurse -Force
dotnet build
```

```bash
# Linux/Mac
find . -type d \( -name "Build" -o -name "obj" -o -name "bin" \) -exec rm -rf {} +
dotnet build
```

### Erro: "O tipo 'IRepository<>' está definido em um assembly que não é referenciado"

**Causa**: Referências às DLLs do Bernhoeft.GRT.Core não configuradas.

**Solução**: Verifique se os arquivos na pasta `Libs/` estão presentes:
- `Bernhoeft.GRT.Core.dll`
- `Bernhoeft.GRT.Core.EntityFramework.dll`
- `Bernhoeft.GRT.Core.Rest.dll`

### Erro de certificado HTTPS

**Solução**:

```bash
dotnet dev-certs https --trust
```

---

## Estrutura do Projeto

```
Avisos/
├── 0-Tests/                          # Testes
│   └── Bernhoeft.GRT.Teste.IntegrationTests/
│       └── Handlers/
│           ├── Commands/             # Testes de Create, Update, Delete
│           └── Queries/              # Testes de GetAviso, GetAvisos
├── 1-Presentation/                   # Camada de Apresentação
│   └── Bernhoeft.GRT.Teste.Api/
│       ├── Controllers/              # Controllers REST
│       └── Program.cs                # Configuração da API
├── 2-Application/                    # Camada de Aplicação
│   └── Bernhoeft.GRT.Teste.Application/
│       ├── Handlers/                 # Handlers MediatR (CQRS)
│       ├── Requests/                 # DTOs de entrada
│       └── Responses/                # DTOs de saída
├── 3-Domain/                         # Camada de Domínio
│   └── Bernhoeft.GRT.Teste.Domain/
│       ├── Entities/                 # Entidades de domínio
│       └── Interfaces/               # Interfaces de repositórios
├── 4-Infra/                          # Camada de Infraestrutura
│   └── Bernhoeft.GRT.Teste.Infra.Persistence.InMemory/
│       └── Repositories/             # Implementação dos repositórios
├── Libs/                             # DLLs do Bernhoeft.GRT.Core
└── Bernhoeft.GRT.Teste.sln           # Solution file
```

---

## Endpoints Implementados

### GET /api/v1/avisos
Retorna todos os avisos **ativos** cadastrados.

- **Response 200**: Lista de avisos
- **Response 204**: Sem avisos cadastrados

### GET /api/v1/avisos/{id}
Retorna um aviso específico por ID.

- **Response 200**: Aviso encontrado
- **Response 400**: ID inválido (≤ 0)
- **Response 404**: Aviso não encontrado

### POST /api/v1/avisos
Cria um novo aviso.

**Body:**
```json
{
  "Titulo": "string (obrigatório, max 50 caracteres)",
  "Mensagem": "string (obrigatório)"
}
```

- **Response 200**: Aviso criado com sucesso
- **Response 400**: Validação falhou (título ou mensagem vazios)

### PUT /api/v1/avisos/{id}
Atualiza a **mensagem** de um aviso existente.

**Body:**
```json
{
  "Mensagem": "string (obrigatório)"
}
```

- **Response 200**: Aviso atualizado
- **Response 400**: ID inválido ou mensagem vazia
- **Response 404**: Aviso não encontrado

### DELETE /api/v1/avisos/{id}
Remove um aviso (soft delete).

- **Response 204**: Aviso removido com sucesso
- **Response 400**: ID inválido (≤ 0)
- **Response 404**: Aviso não encontrado

---

## Testes Unitários

O projeto inclui **34 testes unitários** cobrindo todos os cenários do CRUD:

| Handler | Cenários Testados |
|---------|-------------------|
| **CreateAvisoHandler** | Criação bem-sucedida, DataCriacao definida, Ativo=true, dados corretos |
| **GetAvisoHandler** | Busca existente, NotFound, NoTracking, DataEdicao, múltiplos IDs |
| **GetAvisosHandler** | Lista avisos, NoContent, filtro ativos, dados corretos, lista única |
| **UpdateAvisoHandler** | Atualização sucesso, NotFound, apenas mensagem, chamadas ao repositório |
| **DeleteAvisoHandler** | Deleção sucesso, NotFound, ID correto, mensagem confirmação |

### Tecnologias de Teste
- **xUnit** - Framework de testes
- **Moq** - Mocking de dependências
- **FluentAssertions** - Asserções legíveis

---

## Decisões de Design e Implementação

### 1. Campos de Auditoria (DataCriacao e DataEdicao)

A entidade `AvisoEntity` foi expandida com dois novos campos:

- **DataCriacao**: `DateTime` - Definida automaticamente no momento da criação (UTC)
- **DataEdicao**: `DateTime?` - Atualizada automaticamente a cada modificação (UTC)

**Justificativa**: Permite rastrear quando os avisos foram criados e modificados, atendendo à necessidade da área de negócio.

### 2. Soft Delete

Ao invés de exclusão física, o sistema marca o aviso como `Ativo = false`.

**Implementação**:
- Campo `Ativo` na entidade (já existente)
- Método `DeletarAvisoAsync` define `Ativo = false` e atualiza `DataEdicao`
- Todas as consultas filtram apenas registros com `Ativo = true`

**Justificativa**: Permite manter histórico de dados e possibilita recuperação futura se necessário.

### 3. Validações com FluentValidation

Foram criados validadores para garantir que requisições inválidas não avancem para as camadas internas:

| Request | Validações |
|---------|------------|
| `GetAvisoRequest` | ID > 0 |
| `CreateAvisoRequest` | Título não nulo/vazio (max 50 chars), Mensagem não nula/vazia |
| `UpdateAvisoRequest` | ID > 0, Mensagem não nula/vazia |
| `DeleteAvisoRequest` | ID > 0 |

**Justificativa**: Barrar requisições inválidas na camada de apresentação, antes de chegarem à aplicação/domínio.

### 4. Edição Apenas da Mensagem

Conforme regra de negócio, o endpoint PUT permite editar **apenas o campo Mensagem**.

**Justificativa**: O título é considerado imutável após a criação, garantindo integridade dos dados.

### 5. Filtro de Avisos Ativos

Todos os métodos de busca no repositório (`ObterTodosAvisosAsync`, `ObterAvisoPorIdAsync`) filtram automaticamente apenas registros com `Ativo = true`.

**Justificativa**: Garante que avisos "deletados" (soft delete) não apareçam nas listagens.

---

## Padrão Arquitetural

A implementação seguiu rigorosamente o padrão existente:

- **Clean Architecture** com separação em camadas
- **CQRS** com MediatR para separar Commands e Queries
- **Repository Pattern** para acesso a dados
- **FluentValidation** para validações na camada de apresentação

---

## Banco de Dados

O projeto utiliza **Entity Framework Core InMemory**, ou seja:
- Não precisa de banco de dados externo
- Os dados ficam apenas em memória
- **Dados são perdidos ao reiniciar a aplicação**

---

## Comandos Úteis

```bash
# Restaurar dependências
dotnet restore

# Compilar
dotnet build

# Limpar build
dotnet clean

# Executar API
dotnet run --project 1-Presentation/Bernhoeft.GRT.Teste.Api

# Executar testes
dotnet test

# Executar testes com detalhes
dotnet test --verbosity normal

# Publicar para produção
dotnet publish -c Release
```

---

## Estrutura de Arquivos Criados/Modificados

### Entidade (Domain)
- `AvisoEntity.cs` - Adicionados campos `DataCriacao` e `DataEdicao`

### Repositório (Domain/Infra)
- `IAvisoRepository.cs` - Interface com novos métodos CRUD
- `AvisoRepository.cs` - Implementação com soft delete e filtro de ativos

### Mapeamento (Infra)
- `AvisoMap.cs` - Mapeamento EF para novos campos

### Requests (Application)
- `GetAvisoRequest.cs` - Request para busca por ID
- `CreateAvisoRequest.cs` - Request para criação
- `UpdateAvisoRequest.cs` - Request para edição
- `DeleteAvisoRequest.cs` - Request para exclusão

### Validators (Application)
- `GetAvisoRequestValidator.cs`
- `CreateAvisoRequestValidator.cs`
- `UpdateAvisoRequestValidator.cs`
- `DeleteAvisoRequestValidator.cs`

### Responses (Application)
- `GetAvisoResponse.cs` - Response para aviso único
- `GetAvisosResponse.cs` - Response para lista (atualizado)
- `CreateAvisoResponse.cs` - Response para criação
- `UpdateAvisoResponse.cs` - Response para edição

### Handlers (Application)
- `GetAvisoHandler.cs` - Handler para busca por ID
- `GetAvisosHandler.cs` - Handler para listagem (atualizado)
- `CreateAvisoHandler.cs` - Handler para criação
- `UpdateAvisoHandler.cs` - Handler para edição
- `DeleteAvisoHandler.cs` - Handler para exclusão

### Controller (Presentation)
- `AvisosController.cs` - Endpoints REST completos

### Testes (Tests)
- `CreateAvisoHandlerTests.cs` - Testes de criação
- `UpdateAvisoHandlerTests.cs` - Testes de atualização
- `DeleteAvisoHandlerTests.cs` - Testes de exclusão
- `GetAvisoHandlerTests.cs` - Testes de busca por ID
- `GetAvisosHandlerTests.cs` - Testes de listagem
