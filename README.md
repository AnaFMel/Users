# Projeto Tech Challenge FIAP - UsersAPI

Este projeto tem como objetivo autenticar o usuário, criar novos usuários e obter dados dos usuários cadastrados.

## Estrutura do Projeto

- **Users.API (Deployment)**: Aplicação ASP.NET Core Minimal API responsável pela autenticação, criação e obtenção de dados dos usuários. 
- **SQL Server**: Banco de dados relacional responsável pela persistência dos dados dos usuários.
- **Users.Tests**: Testes de integração e testes unitários dos principais serviços da aplicação.

## Tecnologias Utilizadas

- **.NET 10**: Framework principal
- **ASP.NET Core Minimal API**: API de Usuários
- **Microsoft.EntityFrameworkCore.SqlServer 10.0.7**: Persistência de usuários
- **Azure.Extensions.AspNetCore.Configuration.Secrets 1.5.2**: Biblioteca para obtenção de segredos do chave de cofres na Azure Key Vault
- **Azure.Identity 1.21.0**: Biblioteca para autenticar nos serviços da Azure usando conta de Serviço (Microsoft Entrada ID)
- **MassTransit.Azure.ServiceBus.Core 8.3.4**: Biblioteca para abstração de comunicação com Azure Bus Service
