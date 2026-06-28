# Projeto Tech Challenge FIAP - UsersAPI

Este projeto tem como objetivo autenticar o usuário, criar novos usuários e obter dados dos usuários cadastrados.

## Estrutura do Projeto

- **Users.API**: Aplicação ASP.NET Core Minimal API responsável pela autenticação, criação e obtenção de dados dos usuários.
- **docker-compose.yml**: Configuração do RabbitMQ, MySQL e NotificationsWorker.

## Pré-requisitos

- .NET 10 SDK
- Docker e Docker Compose

## Como executar

### 1. Iniciar todos os serviços (RabbitMQ, MySQL e NotificationsWorker)
```bash
# Docker-compose
docker-compose up -d
```

### 2. Executar o Users.API
```bash
cd Users.API
dotnet run
```

### 4. Acesso à aplicação
1. Abra o navegador em: http://localhost:5099 (ou a porta indicada)
2. Execute os procedimentos informados no arquivo Users.API.http da solução.

### Acesso ao RabbitMQ Management
- URL: http://localhost:15672
- Usuário: guest
- Senha: guest

### Acesso ao Banco de dados MySQL
- Porta: 3306
- Usuário: root
- Senha: SenhaAdmin123!
- Base: users_db

## Tecnologias Utilizadas

- **.NET 10**: Framework principal
- **ASP.NET Core Minimal API**: API de Usuários
- **MassTransit.RabbitMQ 8.3.4**: Biblioteca para comunicação com RabbitMQ
- **MySql.EntityFrameworkCore 10.0.7**: Persistência de usuários