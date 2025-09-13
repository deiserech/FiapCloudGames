# 🎮 FIAP Cloud Games

## 📝 Sobre o Projeto

O **FIAP Cloud Games** é uma plataforma de jogos digitais desenvolvida como parte do TchChallenge (TC) da FIAP. A aplicação oferece um sistema completo para gerenciamento de jogos, biblioteca pessoal de usuários, promoções e autenticação segura.

### 🎯 Objetivos

- **Gerenciamento de Jogos**: Cadastro, consulta e gerenciamento de catálogo de jogos
- **Biblioteca Pessoal**: Sistema para usuários organizarem sua coleção de jogos
- **Sistema de Promoções**: Gestão de ofertas e descontos especiais
- **Autenticação Segura**: Sistema de login/registro com JWT
- **API RESTful**: Interface moderna e escalável para integração

## 🏗️ Arquitetura

O projeto está organizado em camadas:

```
📁 FiapCloudGames/
├── 🌐 FiapCloudGames.Api/          # Camada de apresentação (Controllers, Middlewares)
├── ⚙️ FiapCloudGames.Application/   # Camada de aplicação (Services, Use Cases)
├── 🏛️ FiapCloudGames.Domain/        # Camada de domínio (Entities, DTOs, Interfaces)
├── 🔧 FiapCloudGames.Infrastructure/ # Camada de infraestrutura (Repositories, Data)
└── 🧪 FiapCloudGames.Tests/        # Testes unitários e de integração
```

### 🛠️ Tecnologias Utilizadas

- **.NET 8.0**: Framework principal
- **ASP.NET Core Web API**: Para criação da API REST
- **Entity Framework Core**: ORM para acesso a dados
- **SQL Server**: Banco de dados relacional
- **JWT (JSON Web Tokens)**: Autenticação e autorização
- **Swagger/OpenAPI**: Documentação da API
- **xUnit**: Framework de testes

## 🚀 Como Executar

### 📋 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) ou [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)

### ⚡ Instalação e Configuração

1. **Clone o repositório**
   ```bash
   git clone https://github.com/deiserech/FiapCloudGames.git
   cd FiapCloudGames
   ```

2. **Restaure as dependências**
   ```bash
   dotnet restore
   ```

3. **Configure a string de conexão**
   
   Edite o arquivo `src/FiapCloudGames.Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=FiapCloudGamesDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

4. **Execute as migrações do banco de dados**
   ```bash
   cd src/FiapCloudGames.Api
   dotnet ef database update
   ```

5. **Execute a aplicação**
   ```bash
   dotnet run
   ```

6. **Acesse a documentação da API**
   
   Abra seu navegador e vá para: `https://localhost:5001/swagger`

## 🔑 Autenticação

A API utiliza autenticação Bearer Token (JWT). Para acessar endpoints protegidos:

1. **Registre um usuário** via `POST /api/Auth/register` ou faça login via `POST /api/Auth/login`
2. **Obtenha o token JWT** na resposta
3. **Inclua o token** no header `Authorization: Bearer {seu_token}` nas requisições

### Exemplo de Uso:
```bash
# 1. Registrar usuário
curl -X POST "https://localhost:5001/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao@email.com", 
    "password": "MinhaSenh@123",
    "role": "User"
  }'

# 2. Usar o token retornado em outras requisições
curl -X GET "https://localhost:5001/api/User/profile" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

## 📚 Endpoints da API

### 🔐 Autenticação
- `POST /api/Auth/register` - Registra um novo usuário no sistema
- `POST /api/Auth/login` - Realiza o login de um usuário no sistema

### 🎮 Jogos
- `GET /api/Game` - Lista todos os jogos cadastrados
- `GET /api/Game/{id}` - Obtém um jogo pelo ID
- `POST /api/Game` - Cadastra um novo jogo (apenas administradores)

### 📚 Biblioteca
- `GET /api/Library/user/{userId}` - Obtém a biblioteca completa de um usuário
- `GET /api/Library/{id}` - Obtém uma entrada específica da biblioteca pelo ID
- `POST /api/Library/purchase` - Realiza a compra de um jogo para um usuário

### 🏷️ Promoções
- `GET /api/Promotion/active` - Obtém todas as promoções ativas
- `GET /api/Promotion/{id}` - Obtém uma promoção específica pelo ID
- `POST /api/Promotion` - Cria uma nova promoção
- `PUT /api/Promotion/{id}` - Atualiza uma promoção existente
- `DELETE /api/Promotion/{id}` - Remove uma promoção
- `GET /api/Promotion/game/{gameId}/active` - Obtém todas as promoções ativas de um jogo específico
- `GET /api/Promotion/game/{gameId}/discounted-price` - Calcula o preço com desconto para um jogo específico

### 👤 Usuários
- `GET /api/User/{id}` - Obtém um usuário pelo ID
- `GET /api/User/profile` - Obtém o perfil do usuário autenticado
- `POST /api/User` - Cria um novo usuário

## 🧪 Executando os Testes

### Testes Unitários
```bash
dotnet test
```

### Relatório de Cobertura
```bash
# Execute o script PowerShell para gerar relatório de cobertura
./others/coverage.ps1
```

O relatório será gerado em `tests/coverage-report/index.html`

## 🔧 Configurações Avançadas

### JWT Settings
```json
{
  "JwtSettings": {
    "Issuer": "FiapCloudGames",
    "Audience": "FiapCloudGamesUsers",
    "ExpiryInMinutes": 60
  }
}
```

### Configurações de Ambiente

- **Development**: `appsettings.Development.json`
- **Production**: `appsettings.Production.json`

## 📊 Modelos de Dados

### 🎮 Game (Jogo)
```json
{
  "id": 1,
  "title": "Cyberpunk 2077",
  "description": "RPG futurístico em mundo aberto",
  "price": 199.99
}
```

### 👤 User Roles (Perfis de Usuário)
- **User**: Usuário comum (pode comprar jogos, ver biblioteca)
- **Admin**: Administrador (pode gerenciar jogos e promoções)

### 📚 Library (Biblioteca)
```json
{
  "id": 1,
  "userId": 123,
  "gameId": 456
}
```

### 🏷️ Promotion (Promoção)
```json
{
  "id": 1,
  "title": "Oferta de Verão",
  "description": "Grandes descontos para o verão",
  "discountPercentage": 30.0,
  "discountAmount": null,
  "startDate": "2024-12-01T00:00:00",
  "endDate": "2024-12-31T23:59:59",
  "isActive": true,
  "gameId": 456
}
```

### 🛒 Purchase Request (Compra)
```json
{
  "userId": 123,
  "gameId": 456, 
  "purchasePrice": 139.99,
  "isGift": false,
  "giftMessage": null
}
```

## 📊 Funcionalidades Principais

### 🎮 Gerenciamento de Jogos
- Cadastro de jogos com título, descrição e preço (apenas administradores)
- Consulta de catálogo público de jogos
- Busca de jogos específicos por ID

### 👨‍💻 Sistema de Usuários
- Registro de novos usuários com validação de email
- Autenticação segura com JWT
- Perfis diferenciados (User/Admin)
- Consulta de perfil do usuário autenticado

### 📚 Biblioteca Pessoal
- Compra de jogos com preço personalizado
- Suporte a presentes com mensagens personalizadas
- Visualização da biblioteca completa do usuário
- Controle de jogos já adquiridos (evita duplicatas)

### 🏷️ Sistema de Promoções Avançado
- Criação de promoções com desconto percentual ou valor fixo
- Controle de período de validade das promoções
- Promoções específicas por jogo
- Cálculo automático de preço com desconto
- Listagem de promoções ativas

## 🔒 Segurança

- **Autenticação JWT**: Tokens seguros para autenticação com Bearer scheme
- **Autorização baseada em roles**: Controle de acesso granular (User/Admin)
- **Validação rigorosa de dados**: 
  - Emails válidos obrigatórios
  - Senhas com mínimo de 8 caracteres
  - Validação de preços e descontos
  - Limitação de tamanho de campos
- **Middleware de tratamento de erros**: Respostas padronizadas e seguras
- **Controle de duplicatas**: Evita compras duplicadas de jogos
- **Validação de business rules**: Verificação de regras de negócio antes das operações

## 📁 Estrutura de Pastas Detalhada

```
FiapCloudGames/
├── src/
│   ├── FiapCloudGames.Api/
│   │   ├── Controllers/         # Controladores da API
│   │   ├── Middlewares/         # Middlewares customizados
│   │   ├── Properties/          # Configurações de launch
│   │   └── Request/             # DTOs de requisição
│   ├── FiapCloudGames.Application/
│   │   └── Services/            # Serviços de aplicação
│   ├── FiapCloudGames.Domain/
│   │   ├── DTOs/                # Data Transfer Objects
│   │   ├── Entities/            # Entidades do domínio
│   │   ├── Enums/               # Enumerações
│   │   ├── Interfaces/          # Contratos e interfaces
│   │   └── Utils/               # Utilitários do domínio
│   └── FiapCloudGames.Infrastructure/
│       ├── Data/                # Contexto do Entity Framework
│       ├── Migrations/          # Migrações do banco
│       └── Repositories/        # Implementações dos repositórios
├── tests/
│   └── FiapCloudGames.Tests/    # Testes unitários
└── others/
    └── coverage.ps1            # Script de cobertura de testes
```


## 👥 Equipe

- **Desenvolvedor Principal**: [@deiserech](https://github.com/deiserech)
- **Instituição**: FIAP - Faculdade de Informática e Administração Paulista

## 📞 Contato

- **Email**: rech.deise@gmail.com
- **GitHub**: [FiapCloudGames](https://github.com/deiserech/FiapCloudGames)

---
