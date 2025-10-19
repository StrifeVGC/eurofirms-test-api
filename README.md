# Eurofirms Test API

![.NET](https://img.shields.io/badge/.NET-8.0-blue) ![SQLite](https://img.shields.io/badge/Database-SQLite-orange) ![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-green)

Esta � a **Eurofirms Test API**, uma API de demonstra��o que permite consultar e gerir informa��es de personagens e epis�dios do universo Rick and Morty.

Foi desenvolvida em **.NET 8** com **ASP.NET Core**, utilizando **Entity Framework Core** para persist�ncia de dados, **Identity Framework** para gest�o de utilizadores e **JWT** para autentica��o simples.

## Estrutura do Projeto

- `PruebaEurofirms.Api`: Projeto da API que exp�e os endpoints REST.
- `PruebaEurofirms.Domain`: Cont�m as entidades e o `DbContext`.
- `PruebaEurofirms.Infrastructure`: Implementa��es de reposit�rios, handlers e clientes HTTP.
- `PruebaEurofirms.Tests`: Testes unit�rios dos handlers e reposit�rios.

## Endpoints

### Autentica��o

#### POST /api/auth/login
Request Body:
{
  "username": "admin",
  "password": "admin"
}
Response 200 OK:
{
  "token": "<JWT token>"
}
Response 401 Unauthorized:
{
  "message": "Invalid username or password"
}
Observa��es: O token deve ser inclu�do nos requests subsequentes com o prefixo bearer no header Authorization. Exemplo: Authorization: bearer <token>

### Personagens

#### GET /api/characters
Request: Autentica��o requerida (JWT). Query Parameters (opcional): status (Alive, Dead, Unknown)
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
Request: Autentica��o requerida (JWT)
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

### Epis�dios

#### GET /api/episodes
Request: Autentica��o requerida (JWT)
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
Request: Autentica��o requerida (JWT)
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

O utilizador realiza login atrav�s do endpoint /api/auth/login. Recebe um JWT token, que deve ser inclu�do no header Authorization com o prefixo bearer. Com o token v�lido, o utilizador pode aceder aos endpoints de characters e episodes. Todos os endpoints protegidos verificam o JWT antes de processar o request.

## Unit Tests

O projeto PruebaEurofirms.Tests cont�m testes unit�rios que cobrem Handlers de comandos e queries (MediatR), Reposit�rios (CharacterRepository, EpisodeRepository) e testes de valida��o de mapeamentos DTO. Estes testes garantem que a l�gica de neg�cio funciona corretamente de forma isolada, sem necessidade de aceder � base de dados real.

## Base de Dados e Seeding

A base de dados inicializa com um utilizador admin:
Username: admin
Password: admin
Este utilizador deve ser utilizado para efetuar login no endpoint /api/auth/login. O token gerado deve ser inclu�do com o prefixo bearer nos requests subsequentes para permitir acesso aos endpoints protegidos.  
Importante: Esta autentica��o JWT foi criada apenas para simular um fluxo de token simples e n�o representa seguran�a real. N�o deve ser utilizada em ambientes de produ��o.

## Tecnologias Utilizadas

- .NET 8
- ASP.NET Core
- Entity Framework Core (SQLite)
- ASP.NET Identity
- JWT Bearer Authentication
- Swagger / OpenAPI
- MediatR
- AutoMapper
