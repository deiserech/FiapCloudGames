# Autenticação JWT - FiapCloudGames

Este documento descreve como usar o sistema de autenticação JWT implementado no projeto FiapCloudGames.

## Configuração

### Configurações JWT (appsettings.json)
```json
{
  "JwtSettings": {
    "SecretKey": "FiapCloudGames2024SuperSecretKeyForJWTAuthentication",
    "Issuer": "FiapCloudGames",
    "Audience": "FiapCloudGamesUsers",
    "ExpiryInMinutes": 60
  }
}
```

## Endpoints de Autenticação

### 1. Registro de Usuário
**POST** `/api/auth/register`

**Body:**
```json
{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "minhasenha123"
}
```

**Resposta de Sucesso (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "joao@email.com",
  "name": "João Silva",
  "userId": 1
}
```

**Resposta de Erro (400):**
```json
{
  "message": "Email já está em uso"
}
```

### 2. Login
**POST** `/api/auth/login`

**Body:**
```json
{
  "email": "joao@email.com",
  "password": "minhasenha123"
}
```

**Resposta de Sucesso (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "joao@email.com",
  "name": "João Silva",
  "userId": 1
}
```

**Resposta de Erro (401):**
```json
{
  "message": "Email ou senha inválidos"
}
```

## Endpoints Protegidos

### 1. Obter Perfil do Usuário Logado
**GET** `/api/user/profile`

**Headers:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Resposta de Sucesso (200):**
```json
{
  "id": 1,
  "name": "João Silva",
  "email": "joao@email.com"
}
```

### 2. Obter Usuário por ID
**GET** `/api/user/{id}`

**Headers:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Resposta de Sucesso (200):**
```json
{
  "id": 1,
  "name": "João Silva",
  "email": "joao@email.com"
}
```

## Como Usar o Token JWT

1. **Obter o Token**: Faça login ou registre-se para obter um token JWT
2. **Incluir o Token**: Para acessar endpoints protegidos, inclua o token no header Authorization:
   ```
   Authorization: Bearer SEU_TOKEN_AQUI
   ```
3. **Validade**: O token expira em 60 minutos (configurável)

## Estrutura do Token JWT

O token contém as seguintes informações (claims):
- `nameid`: ID do usuário
- `email`: Email do usuário
- `name`: Nome do usuário
- `jti`: ID único do token
- `exp`: Data de expiração

## Segurança

- As senhas são criptografadas usando BCrypt
- O token JWT é assinado com uma chave secreta
- Endpoints protegidos requerem autenticação válida
- Senhas nunca são retornadas nas respostas da API

## Testando com Swagger

1. Execute a aplicação
2. Acesse `/swagger`
3. Use o endpoint `/api/auth/register` ou `/api/auth/login` para obter um token
4. Clique no botão "Authorize" no Swagger
5. Digite: `Bearer SEU_TOKEN_AQUI`
6. Agora você pode testar os endpoints protegidos

## Exemplo de Uso com JavaScript/Fetch

```javascript
// Login
const loginResponse = await fetch('/api/auth/login', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    email: 'joao@email.com',
    password: 'minhasenha123'
  })
});

const loginData = await loginResponse.json();
const token = loginData.token;

// Usar o token para acessar endpoint protegido
const profileResponse = await fetch('/api/user/profile', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

const profileData = await profileResponse.json();
console.log(profileData);
