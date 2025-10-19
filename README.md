# Eurofirms Test API

![.NET](https://img.shields.io/badge/.NET-8.0-blue) ![SQLite](https://img.shields.io/badge/Database-SQLite-orange) ![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-green)

Esta é a **Eurofirms Test API**, uma API de demonstração que permite consultar e gerir informações de personagens e episódios do universo Rick and Morty.

Foi desenvolvida em **.NET 8** com **ASP.NET Core**, utilizando **Entity Framework Core** para persistência de dados, **Identity Framework** para gestão de utilizadores e **JWT** para autenticação simples.

## Estrutura do Projeto

- `PruebaEurofirms.Api`: Projeto da API que expõe os endpoints REST.
- `PruebaEurofirms.Domain`: Contém as entidades e o `DbContext`.
- `PruebaEurofirms.Infrastructure`: Implementações de repositórios, handlers e clientes HTTP.
- `PruebaEurofirms.Tests`: Testes unitários dos handlers e repositórios.

## Endpoints

### Autenticação

#### POST /api/auth/login
- Fornece um token jwt (Utilizar username: admin e password: admin)

*Observações*: O token deve ser incluído nos requests subsequentes com o prefixo bearer no header Authorization. Exemplo: Authorization: bearer <token>

### RickAndMorty

#### GET /api/RickAndMorty/import
- Importa todas as characters da Rick and Morty API e adiciona as não existentes.
- retorna:
  - 200, Retorna contagem de personagens inseridas
  - 500, Mensagem de erro caso algo tenha acontecido (500)

#### GET /api/RickAndMorty/{status}
- Request: Autenticação requerida (JWT)
- Passar Status (Alive, Dead, ou Unknown) para filtrar pelos status. Case insensitive.
- retorna:
  - 200, Lista de characters do status requisitado
  - 400, caso o status dado seja inválido
  - 500, caso aconteça um erro inesperado

#### DELETE /api/RickAndMorty/{apiId}
- Request: Autenticação requerida (JWT)
- Passar o ApiId da character que queremos apagar
- retorna:
  - 200, caso a character tenha sido apagada com sucesso.
  - 404, caso a character não tenha sido encontrada.
  - 500, caso tenha acontecido um erro inesperado
O projeto PruebaEurofirms.Tests contém testes unitários que cobrem Handlers de comandos e queries (MediatR), e testes de validação de mapeamentos DTO. Estes testes garantem que a lógica de negócio funciona corretamente de forma isolada, sem necessidade de aceder à base de dados real.

## Base de Dados e Seeding

- A base de dados inicializa com um utilizador admin:
  - Username: admin
  - Password: admin
- Este utilizador deve ser utilizado para efetuar login no endpoint /api/auth/login. O token gerado deve ser incluído com o prefixo bearer nos requests subsequentes para permitir acesso aos endpoints protegidos.  
- **Importante: Esta autenticação JWT foi criada apenas para simular um fluxo de token simples e não representa segurança real. Não deve ser utilizada em ambientes de produção.**

## Tecnologias Utilizadas

- .NET 8
- ASP.NET Core
- Entity Framework Core (SQLite)
- ASP.NET Identity
- JWT Bearer Authentication
- Swagger / OpenAPI
- MediatR
- AutoMapper
