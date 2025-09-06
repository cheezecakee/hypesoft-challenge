
[Leia a versão em português](README-PT.md) | [Read the English version](README.md)

# Sistema de Gerenciamento de Produtos Hypesoft

## Visão Geral

Este projeto é um sistema de gerenciamento de produtos full-stack que demonstra arquitetura moderna, práticas de código limpo e integração avançada entre frontend e backend.

- **Backend:** .NET 9, Clean Architecture, DDD, CQRS, MediatR, MongoDB, FluentValidation, AutoMapper, Serilog  
- **Frontend:** Next.js 14 (App Router), React 18, TypeScript, TailwindCSS + shadcn/ui, TanStack Query, React Hook Form + Zod, Recharts  
- **Autenticação:** Keycloak (OAuth2 / OpenID Connect)

---

## Funcionalidades Implementadas

### Backend
- Operações CRUD para Produtos e Categorias
- Entidade Produto inclui **quantidade de estoque** e flag `IsLowStock`
- Proteção de rotas baseada em papéis (Admin, Gerente, Usuário)
- Documentação da API com Swagger
- Endpoints de verificação de saúde
- Validação via FluentValidation
- Logging via Serilog
- Padrão CQRS + MediatR para comandos e consultas
- Implementação de repositório MongoDB

### Frontend
- Páginas de login/logout integradas com Keycloak
- Rotas protegidas para acesso baseado em papéis
- Dashboard com cartões de estatísticas e gráficos
- Listagem, criação, edição e exclusão de produtos e categorias
- Filtros e busca para produtos
- Componentes de formulário reutilizáveis com React Hook Form + Zod
- Validação no lado do cliente e tratamento de validação no servidor
- Estados de carregamento e notificações toast
- Design responsivo (desktop + mobile)

---

## Primeiros Passos

### Pré-requisitos
- Docker Desktop 4.0+  
- Node.js 18+  
- .NET 9 SDK  
- Git  

### Variáveis de Ambiente

Frontend e backend requerem variáveis de ambiente para URLs da API e integração com Keycloak:

### Frontend
```bash
NEXT_PUBLIC_API_URL=http://localhost:5000/api
NEXT_PUBLIC_KEYCLOAK_URL=http://localhost:8080
NEXT_PUBLIC_KEYCLOAK_REALM=Hypesoft
NEXT_PUBLIC_KEYCLOAK_CLIENT_ID=frontend
```

--- 

## Instalação

### Clonar o Repositório
```bash
git clone https://github.com/cheezecakee/hypesoft-challenge.git
cd hypesoft-challenge
```

--- 

### Usando Docker

A aplicação pode ser executada localmente com Docker Compose, que inclui Keycloak, MongoDB e MongoExpress com logging pré-configurado:
```bash
docker-compose up -d```

Acesse os serviços:
Console Admin do Keycloak: `http://localhost:8080`
- Usuário: admin
- Senha: admin123
Interface MongoExpress: `http://localhost:8081`
- Usuário: admin
- Senha: admin

Logs do Docker para cada serviço podem ser visualizados com:
```
docker logs -f hypesoft-keycloak
docker logs -f hypesoft-mongo
docker logs -f mongo-express
```
--- 

### Executando Localmente (Sem Docker)

#### Frontend
```
cd frontend/hypesoft
npm install
npm run dev
```
#### Backend
```
cd backend/Hypesoft.API
dotnet restore
dotnet run
```

Swagger UI: `http://localhost:5000/swagger`

---

## Arquitetura

### Backend
- Padrão: Clean Architecture + DDD
- Manipulação de Comandos/Consultas: CQRS com MediatR
- Banco de Dados: MongoDB (padrão Repository, Unit-of-Work se aplicável)
- Validação: FluentValidation para DTOs e comandos
- Mapeamento: AutoMapper para transformações entidade-DTO
- Logging: Serilog
- Endpoints: Documentação Swagger para exploração da API
- Segurança: Proteção de rotas baseada em papéis, Keycloak OAuth2/OpenID Connect

### Frontend
- Framework: Next.js 14 (App Router) com React 18 + TypeScript
- Estilização: TailwindCSS com componentes shadcn/ui
- Formulários e Validação: React Hook Form + Zod
- Busca e Cache de Dados: TanStack Query
- Gráficos e Dashboards: Recharts
- Roteamento e Autenticação: Rotas protegidas integradas com Keycloak
- Gerenciamento de Estado: Estado local mínimo + cache do React Query

### Notas Adicionais
- Estrutura modular de componentes para reutilização
- Hooks reutilizáveis para interações com a API

--- 

## Modo de Desenvolvimento com Dados de Teste

Ao executar o backend em modo de desenvolvimento, dados fictícios para produtos e categorias são automaticamente inseridos no MongoDB. Isso permite:
Testar rapidamente funcionalidades do frontend
Visualizar dashboards e listas de produtos/categorias
Trabalhar com dados de exemplo realistas sem criação manual
