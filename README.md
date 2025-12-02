# Bernhoeft GRT - API de Avisos

## Desafio Técnico: Modificações na API de Avisos

Esta documentação descreve as implementações realizadas para o desafio técnico Bernhoeft GRT.

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

---

## Padrão Arquitetural

A implementação seguiu rigorosamente o padrão existente:

- **Clean Architecture** com separação em camadas
- **CQRS** com MediatR para separar Commands e Queries
- **Repository Pattern** para acesso a dados
- **FluentValidation** para validações na camada de apresentação

---

## Execução

```bash
# Build
dotnet build

# Run
dotnet run --project 1-Presentation/Bernhoeft.GRT.Teste.Api

# Acessar Swagger
# http://localhost:{porta}/
```
