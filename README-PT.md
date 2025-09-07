[Leia a versão em português](README-PT.md) | [Read the English version](README.md)

# Hypesoft Challenge - Guia Completo de Configuração

## Visão Geral

Este projeto é um sistema de gerenciamento de produtos full-stack que demonstra arquitetura moderna, práticas de clean code e integração avançada entre frontend e backend.

## Pré-requisitos

- Docker e Docker Compose instalados
- Git
- Node.js 18+ e npm (para desenvolvimento local do frontend)
- .NET 9 SDK (para desenvolvimento local do backend)
- curl ou Postman para testes da API

---

## Arquitetura

### Backend
- **Padrão**: Clean Architecture + DDD
- **Manipulação de Command/Query**: CQRS com MediatR
- **Database**: MongoDB
- **Validação**: FluentValidation para DTOs e commands
- **Mapeamento**: AutoMapper para transformações entity-DTO
- **Logging**: Serilog
- **Endpoints**: Documentação Swagger para exploração da API
- **Segurança**: Proteção de rotas baseada em roles, Keycloak OAuth2/OpenID Connect

### Frontend
- **Framework**: Next.js 14 (App Router) com React 18 + TypeScript
- **Estilização**: TailwindCSS com componentes shadcn/ui
- **Formulários e Validação**: React Hook Form + Zod
- **Data Fetching e Caching**: TanStack Query
- **Gráficos e Dashboards**: Recharts
- **Roteamento e Autenticação**: Rotas protegidas integradas com Keycloak
- **Gerenciamento de Estado**: Estado local mínimo + cache do React Query

### Notas Adicionais
- Estrutura modular de componentes para reutilização
- Hooks reutilizáveis para interações com API

---

## Início Rápido (Recomendado)

### Sistema Completo (Todos os Serviços)
```bash
git clone https://github.com/cheezecakee/hypesoft-challenge.git
cd hypesoft-challenge
docker compose --profile full up -d
```

### Apenas Infraestrutura (Sem Backend/Frontend)
```bash
docker compose up -d
```

### Backend + Infraestrutura
```bash
docker compose --profile backend up -d
```

### Frontend + Infraestrutura
```bash
docker compose --profile frontend up -d
```

Isso iniciará:
- **Keycloak**: `http://localhost:8080` (pré-configurado)
- **Backend API**: `http://localhost:5113` (se usando profile backend)
- **MongoDB**: `localhost:27017`
- **Mongo Express**: `http://localhost:8081`

## Modo Dev com Dados de Seed

Ao executar o backend em modo de desenvolvimento, dados mock para produtos e categorias são automaticamente inseridos no MongoDB. Isso permite:
- Testar rapidamente funcionalidades do frontend
- Visualizar dashboards e listas de produtos/categorias
- Trabalhar com dados de exemplo realistas sem criação manual

---

## Autenticação Pré-configurada

O sistema vem com um realm Keycloak pré-configurado que inclui:
- **Realm**: `hypesoft`
- **Client**: `hypesoft-api`
- **Usuários de Teste**:
  - **Admin**: `admin@hypesoft.com` / `admin123` (role Admin)
  - **Manager**: `manager@hypesoft.com` / `manager123` (role Manager)
  - **User**: `user@hypesoft.com` / `user123` (role User)

### URLs dos Serviços e Credenciais

| Serviço | URL | Credenciais |
|---------|-----|-------------|
| **Keycloak Admin** | http://localhost:8080 | admin / admin |
| **Backend API** | http://localhost:5113 | Bearer token obrigatório |
| **Swagger UI** | http://localhost:5113/swagger | Bearer token obrigatório |
| **Mongo Express** | http://localhost:8081 | admin / admin |
| **Frontend** | http://localhost:3000 | Keycloak SSO |

---

## Autenticação e Testes da API

### Obtendo Bearer Token

```bash
# Para Usuário Admin
curl -X POST "http://localhost:8080/realms/hypesoft/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=hypesoft-api" \
  -d "client_secret=dLGltIUV18VMwomohmQsfa3blaOehFR0" \
  -d "username=admin@hypesoft.com" \
  -d "password=admin123"

# Para Usuário Manager
curl -X POST "http://localhost:8080/realms/hypesoft/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=hypesoft-api" \
  -d "client_secret=dLGltIUV18VMwomohmQsfa3blaOehFR0" \
  -d "username=manager@hypesoft.com" \
  -d "password=manager123"

# Para Usuário Regular
curl -X POST "http://localhost:8080/realms/hypesoft/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=hypesoft-api" \
  -d "client_secret=dLGltIUV18VMwomohmQsfa3blaOehFR0" \
  -d "username=user@hypesoft.com" \
  -d "password=user123"
```

> **⚠️ Nota de Segurança:** Essas credenciais são apenas para desenvolvimento/teste. Nunca use em ambientes de produção.

⚠️ **Importante:** Se executando o backend no Docker, o Keycloak também está no Docker. Para obter um token, você deve executar curl dentro da mesma rede Docker:
```bash
docker run --rm --network hypesoft-challenge_default curlimages/curl:latest \
  -X POST "http://hypesoft-keycloak:8080/realms/hypesoft/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=hypesoft-api" \
  -d "client_secret=..." \
  -d "username=admin@hypesoft.com" \
  -d "password=admin123"
```

### Formato da Resposta Esperada
Autenticação bem-sucedida retorna:
```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsInR5cC...",
  "token_type": "Bearer",
  "expires_in": 300,
  "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cC..."
}
```

### Usando o Token

#### No Swagger UI:
1. Acesse o Swagger: `http://localhost:5113/swagger`
2. Clique no botão "Authorize"
3. Digite: `Bearer {seu_access_token}`
4. Clique "Authorize"

#### No curl:
```bash
# Extraia o token da resposta e use nas chamadas da API
TOKEN="seu_access_token_aqui"
curl -X GET "http://localhost:5113/api/protected-endpoint" \
  -H "Authorization: Bearer $TOKEN"
```

### Solução de Problemas
- **401 Unauthorized**: Verifique a combinação usuário/senha
- **Connection refused**: Certifique-se de que o Keycloak está rodando na porta 8080
- **Problemas de rede Docker**: Verifique o nome da rede com `docker network ls`

---

## Documentação da API

### Endpoints Disponíveis

#### Endpoints Públicos (Sem Autenticação Necessária)
- `GET /api/Health` - Endpoint de verificação de saúde
- Documentação Swagger em `/swagger`

#### Endpoints do Dashboard (Apenas Usuários Autenticados)
- `GET /api/Dashboard/stats` - Estatísticas do dashboard
- `GET /api/Dashboard/products-by-category` - Produtos agrupados por categoria

#### Gerenciamento de Produtos (Apenas Admin/Manager)
- `GET /api/Products` - Listar produtos com paginação
- `POST /api/Products` - Criar novo produto
- `PUT /api/Products/{id}` - Atualizar produto
- `DELETE /api/Products/{id}` - Deletar produto
- `PATCH /api/Products/{id}/stock` - Atualizar nível de estoque do produto
- `GET /api/Products/low-stock` - Obter produtos com estoque baixo

#### Gerenciamento de Categorias (Apenas Admin/Manager)
- `GET /api/Categories` - Listar categorias
- `POST /api/Categories` - Criar categoria
- `PUT /api/Categories/{id}` - Atualizar categoria
- `DELETE /api/Categories/{id}` - Deletar categoria

### Requisitos Detalhados de Autorização

#### **Acesso Público**
- Endpoints de health check
- Documentação Swagger

#### **Usuários Autenticados** (Qualquer token válido)
- Visualização do Dashboard (`/api/Dashboard/*`)
- Acesso somente leitura para visualizar estatísticas e gráficos
- Usuários regulares podem apenas visualizar dados do dashboard

#### **Apenas Admin/Manager** (Requer role Admin ou Manager)
- **Todas as operações CRUD** em Produtos e Categorias
- **Operações de gerenciamento de estoque**
- **Criação, modificação e exclusão de produtos**
- **Gerenciamento de categorias**
- **Funcionalidades de controle de inventário**

**Nota**: Usuários autenticados regulares são restritos apenas à visualização do dashboard. Funções administrativas requerem privilégios elevados (roles Admin ou Manager).

---

## Executando Serviços Individualmente

### Apenas Backend
```bash
cd backend/Hypesoft.API
dotnet restore
dotnet run
```
- Executa em: `http://localhost:5113`
- Requer: MongoDB e Keycloak em execução

### Apenas Frontend
```bash
cd frontend/
npm install
npm run dev
```
- Executa em: `http://localhost:3000`
- Requer: Backend API em execução

### Apenas Serviços de Infraestrutura
```bash
# Iniciar MongoDB, Keycloak e Mongo Express
docker compose up -d mongodb keycloak mongo-express
```

---

## Solução de Problemas

### Problemas Comuns

#### 1. Serviços Não Inicializando
**Solução**: Verifique se todas as portas necessárias estão disponíveis e inicie os serviços passo a passo:
```bash
docker compose ps
docker compose logs [nome-do-serviço]
```

#### 2. Problemas de Autenticação
**Problema**: Token não funcionando ou expirado.

**Solução**: Obtenha um token novo usando os comandos curl acima. Tokens expiram após 5 minutos por padrão.

#### 3. Problemas de Conexão com Database
**Problema**: Backend não consegue conectar ao MongoDB.

**Solução**: Certifique-se de que o MongoDB está rodando e acessível:
```bash
docker compose logs mongodb
docker compose restart backend
```

#### 4. Conflitos de Porta
**Problema**: Portas já em uso.

**Solução**: Pare os serviços conflitantes ou modifique as portas no docker-compose.yml

### Visualizando Logs
```bash
# Todos os serviços
docker compose logs -f

# Serviço específico
docker compose logs -f backend
docker compose logs -f keycloak
docker compose logs -f mongodb
```

### Resetando Tudo
```bash
# Parar e remover todos os containers e volumes
docker compose down -v

# Começar do zero
docker compose --profile full up -d
```

---

## Fluxo de Desenvolvimento

### 1. Iniciar Apenas a Infraestrutura
```bash
docker compose up -d
```

### 2. Executar Backend Localmente (Opcional)
```bash
cd backend/Hypesoft.API
dotnet run
```

### 3. Executar Frontend Localmente (Opcional)
```bash
cd frontend/
npm run dev
```

### 4. Fazer Alterações
- Mudanças no backend requerem restart
- Frontend tem hot reload
- Mudanças no database persistem nos volumes Docker

---

## Suporte

### Obtendo Ajuda
1. Verifique os logs: `docker compose logs [nome-do-serviço]`
2. Verifique o status dos serviços: `docker compose ps`
3. Teste conectividade: `curl http://localhost:5113/api/Health`
4. Verifique autenticação com usuários de teste
5. Verifique database no Mongo Express

### Limitações Conhecidas
- Setup de desenvolvimento usa HTTP (não HTTPS)
- Client secrets estão expostos no docker-compose (apenas desenvolvimento)
- Serviço do frontend está comentado aguardando criação do Dockerfile
