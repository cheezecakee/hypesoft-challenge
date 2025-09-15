[Leia a versão em português](README-PT.md) | [Read the English version](README.md)

# Hypesoft Challenge - Guia Completo de Configuração

## Visão Geral

Este projeto é um sistema de gerenciamento de produtos full-stack que demonstra arquitetura moderna, práticas de clean code e integração avançada entre frontend e backend.

## Demo 
[Link](https://youtu.be/l-DpusnmkZQ)

## Pré-requisitos

- Docker Desktop 4.0+
- Node.js 18+
- .NET 9 SDK
- Git

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

## Instalação e Execução

### Clone o repositório
```bash
git clone https://github.com/cheezecakee/hypesoft-challenge.git
cd hypesoft-challenge
```

### Copie as variáveis de ambiente
```bash
cp .env.example .env ## Incluído para esta demonstração
```

### Execute toda a aplicação com Docker Compose
```bash
docker-compose up -d
```

### Aguarde alguns segundos para os serviços iniciarem
```bash
# Verifique se todos os containers estão rodando
docker-compose ps
```

## URLs de Acesso

| Serviço | URL | Credenciais |
|---------|-----|-------------|
| **Frontend** | http://localhost:3000 | Keycloak SSO |
| **API** | http://localhost:5000 | Bearer token obrigatório |
| **Swagger** | http://localhost:5000/swagger | Bearer token obrigatório |
| **MongoDB Express** | http://localhost:8081 | admin / admin |
| **Keycloak** | http://localhost:8080 | admin / admin |

## Desenvolvimento Local

### Desenvolvimento do frontend
```bash
cd frontend
npm install
npm run dev
```

### Desenvolvimento do backend
```bash
dotnet restore
dotnet run --project backend/Hypesoft.API
```

### Executar testes
```bash
# Testes do backend
dotnet test

# Testes do frontend
cd frontend
npm test
```

---

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

### Usando o Token

#### No Swagger UI:
1. Acesse o Swagger: `http://localhost:5000/swagger`
2. Clique no botão "Authorize"
3. Digite: `Bearer {seu_access_token}`
4. Clique "Authorize"

#### No curl:
```bash
curl -H "Authorization: Bearer {seu_access_token}" \
     http://localhost:5000/api/Dashboard/stats
```

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

## Opções de Desenvolvimento

### Apenas Infraestrutura (MongoDB, Keycloak, Mongo Express)
```bash
docker compose --profile infrastructure up -d
```

Em seguida execute backend e frontend localmente:
```bash
# Backend
dotnet run --project backend/Hypesoft.API

# Frontend (em outro terminal)
cd frontend
npm install
npm run dev
```

### Aplicação Completa (Todos os serviços no Docker)
```bash
docker compose up -d
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
docker compose up -d
```

---

## Suporte

### Obtendo Ajuda
1. Verifique os logs: `docker compose logs [nome-do-serviço]`
2. Verifique o status dos serviços: `docker compose ps`
3. Teste conectividade: `curl http://localhost:5000/api/Health`
4. Verifique autenticação com usuários de teste
5. Verifique database no Mongo Express

### Limitações Conhecidas
- Setup de desenvolvimento usa HTTP (não HTTPS)
- Client secrets estão expostos no docker-compose (apenas desenvolvimento)
