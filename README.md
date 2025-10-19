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
- Request Body:
{
  "username": "admin",
  "password": "admin"
}
- Response 200 OK:
{
  "token": "<JWT token>"
}
- Response 401 Unauthorized:
{
  "message": "Invalid username or password"
}

*Observações*: O token deve ser incluído nos requests subsequentes com o prefixo bearer no header Authorization. Exemplo: Authorization: bearer <token>

### Personagens

#### GET /api/characters
Request: Autenticação requerida (JWT). Query Parameters (opcional): status (Alive, Dead, Unknown)
Response 200 OK:
[
  {
    "id": 1,
    "name": "Rick Sanchez",
    "status": "Alive",
    "species": "Human",
    "gender": "Male"
  }
]

#### GET /api/characters/{id}
Request: Autenticação requerida (JWT)
Response 200 OK:
{
  "id": 1,
  "name": "Rick Sanchez",
  "status": "Alive",
  "species": "Human",
  "gender": "Male"
}
Response 404 Not Found:
{
  "message": "Character not found"
}

### Episódios

#### GET /api/episodes
Request: Autenticação requerida (JWT)
Response 200 OK:
[
  {
    "id": 1,
    "name": "Pilot",
    "air_date": "December 2, 2013",
    "episode": "S01E01"
  }
]

#### GET /api/episodes/{id}
Request: Autenticação requerida (JWT)
Response 200 OK:
{
  "id": 1,
  "name": "Pilot",
  "air_date": "December 2, 2013",
  "episode": "S01E01"
}
Response 404 Not Found:
{
  "message": "Episode not found"
}

## Fluxo da API

O utilizador realiza login através do endpoint /api/auth/login. Recebe um JWT token, que deve ser incluído no header Authorization com o prefixo bearer. Com o token válido, o utilizador pode aceder aos endpoints de characters e episodes. Todos os endpoints protegidos verificam o JWT antes de processar o request.

## Unit Tests

O projeto PruebaEurofirms.Tests contém testes unitários que cobrem Handlers de comandos e queries (MediatR), Repositórios (CharacterRepository, EpisodeRepository) e testes de validação de mapeamentos DTO. Estes testes garantem que a lógica de negócio funciona corretamente de forma isolada, sem necessidade de aceder à base de dados real.

## Base de Dados e Seeding

A base de dados inicializa com um utilizador admin:
Username: admin
Password: admin
Este utilizador deve ser utilizado para efetuar login no endpoint /api/auth/login. O token gerado deve ser incluído com o prefixo bearer nos requests subsequentes para permitir acesso aos endpoints protegidos.  
Importante: Esta autenticação JWT foi criada apenas para simular um fluxo de token simples e não representa segurança real. Não deve ser utilizada em ambientes de produção.

## Tecnologias Utilizadas

- .NET 8
- ASP.NET Core
- Entity Framework Core (SQLite)
- ASP.NET Identity
- JWT Bearer Authentication
- Swagger / OpenAPI
- MediatR
- AutoMapper
