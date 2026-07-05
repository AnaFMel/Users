# Projeto Tech Challenge FIAP - UsersAPI

Este projeto tem como objetivo autenticar o usuário, criar novos usuários e obter dados dos usuários cadastrados.

## Estrutura do Projeto

- **Users.API (Deployment)**: Aplicação ASP.NET Core Minimal API responsável pela autenticação, criação e obtenção de dados dos usuários. 
- **MySQL (Deployment)**: Banco de dados relacional responsável pela persistência dos dados dos usuários.
- **Configmap e Secrets**: configurações relacionadas ao banco de dados.

## Tecnologias Utilizadas

- **.NET 10**: Framework principal
- **ASP.NET Core Minimal API**: API de Usuários
- **MySql.EntityFrameworkCore 10.0.7**: Persistência de usuários
- **MassTransit.RabbitMQ 8.3.4**: Biblioteca para comunicação com RabbitMQ