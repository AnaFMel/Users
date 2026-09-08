### Diagrama de Sequência: Autenticação, Cadastro e Notificação

```mermaid
sequenceDiagram
    autonumber
    actor User as Usuário / Client
    participant API as Users.API (Port 5000)
    participant US as UserService
    participant RMQ as RabbitMQ (MassTransit)
    participant Worker as Notifications.Worker
    participant Email as Serviço de E-mail

    title Fluxo de Autenticação, Cadastro de Usuário e Notificação Assíncrona

    %% Passo 1: Autenticação
    Note over User, API: 1. Autenticação do Usuário
    User->>API: POST /api/users/auth (Credenciais)
    activate API
    API->>API: Valida credenciais e gera JWT
    API-->>User: Retorna Token JWT (200 OK)
    deactivate API

    %% Passo 2: Cadastro de Usuário
    Note over User, API: 2. Cadastro de Novo Usuário
    User->>API: POST /api/users/create (Dados do Usuário + Bearer Token)
    activate API
    
    API->>US: Chame UserService.Add(user)
    activate US
    US->>US: Salva usuário no Banco de Dados
    
    %% Passo 3: Mensageria com MassTransit
    US->>RMQ: Publica UserCreatedEvent (via MassTransit)
    activate RMQ
    US-->>API: Retorna Sucesso (User adicionado)
    deactivate US
    
    API-->>User: Retorna Usuário Criado (201 Created)
    deactivate API

    %% Passo 4: Processamento Assíncrono pelo Worker
    Note over RMQ, Worker: 3. Processamento Assíncrono (Background)
    RMQ-)Worker: Consome UserCreatedEvent (MassTransit Consumer)
    deactivate RMQ
    activate Worker
    
    Worker->>Email: Envia e-mail de boas-vindas ao usuário
    activate Email
    Email-->>Worker: E-mail enviado com sucesso
    deactivate Email
    
    deactivate Worker