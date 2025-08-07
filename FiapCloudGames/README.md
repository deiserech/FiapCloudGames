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

## 📚 Endpoints da API

### 🔐 Autenticação
- `POST /api/auth/register` - Registrar novo usuário
- `POST /api/auth/login` - Fazer login

### 🎮 Jogos
- `GET /api/games` - Listar todos os jogos
- `GET /api/games/{id}` - Obter jogo por ID
- `POST /api/games` - Criar novo jogo
- `PUT /api/games/{id}` - Atualizar jogo
- `DELETE /api/games/{id}` - Remover jogo

### 📚 Biblioteca
- `GET /api/library` - Obter biblioteca do usuário
- `POST /api/library/purchase` - Comprar jogo

### 🏷️ Promoções
- `GET /api/promotions` - Listar promoções ativas
- `POST /api/promotions` - Criar nova promoção

### 👤 Usuários
- `GET /api/users/profile` - Obter perfil do usuário
- `PUT /api/users/profile` - Atualizar perfil

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

## 📊 Funcionalidades Principais

### 🎮 Gerenciamento de Jogos
- Cadastro de jogos com título, descrição e preço
- Consulta de catálogo com filtros
- Atualização de informações de jogos

### 👨‍💻 Sistema de Usuários
- Registro e autenticação com JWT
- Perfis de usuário personalizáveis
- Gerenciamento de sessões

### 📚 Biblioteca Pessoal
- Compra e adição de jogos à biblioteca
- Visualização da coleção pessoal
- Histórico de compras

### 🏷️ Sistema de Promoções
- Criação de ofertas especiais
- Aplicação automática de descontos
- Gestão de campanhas promocionais

## 🔒 Segurança

- **Autenticação JWT**: Tokens seguros para autenticação
- **Autorização baseada em roles**: Controle de acesso granular
- **Validação de dados**: Validações robustas em todas as camadas
- **Middleware de tratamento de erros**: Respostas padronizadas e seguras

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
