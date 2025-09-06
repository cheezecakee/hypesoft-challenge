[Leia a versão em português](README-PT.md) | [Read the English version](README.md)

# Hypesoft Product Management System

## Overview

This project is a full-stack product management system demonstrating modern architecture, clean code practices, and advanced frontend/backend integration.

- **Backend:** .NET 9, Clean Architecture, DDD, CQRS, MediatR, MongoDB, FluentValidation, AutoMapper, Serilog  
- **Frontend:** Next.js 14 (App Router), React 18, TypeScript, TailwindCSS + shadcn/ui, TanStack Query, React Hook Form + Zod, Recharts  
- **Authentication:** Keycloak (OAuth2 / OpenID Connect)

---

## Features Implemented

### Backend
- CRUD operations for Products and Categories
- Product entity includes **stock quantity** and `IsLowStock` flag
- Role-based route protection (Admin, Manager, User)
- Swagger API documentation
- Health check endpoints
- Validation via FluentValidation
- Logging via Serilog
- CQRS + MediatR pattern for commands and queries
- MongoDB repository implementation

### Frontend
- Login/logout pages integrated with Keycloak
- Protected routes for role-based access
- Dashboard with stats cards and charts
- Product and category list, create, edit, delete pages
- Filters and search for products
- Reusable form components with React Hook Form + Zod
- Client-side validation + server-side validation handling
- Loading states and toast notifications
- Responsive design (desktop + mobile)

**⚠️ Note:** Currently, **Product stock update is not available**. All other functionality is operational.

---

## Getting Started

### Prerequisites
- Docker Desktop 4.0+  
- Node.js 18+  
- .NET 9 SDK  
- Git  

### Environment Variables

Frontend and backend require environment variables for API URLs and Keycloak integration:

### Frontend
```bash
NEXT_PUBLIC_API_URL=http://localhost:5000/api
NEXT_PUBLIC_KEYCLOAK_URL=http://localhost:8080
NEXT_PUBLIC_KEYCLOAK_REALM=Hypesoft
NEXT_PUBLIC_KEYCLOAK_CLIENT_ID=frontend
```

--- 

## Installation

### Clone the Repository
```bash
git clone https://github.com/cheezecakee/hypesoft-challenge.git
cd hypesoft-challenge
```

--- 

### Using Docker

The application can be run locally with Docker Compose, which includes Keycloak, MongoDB, and MongoExpress with preconfigured logging:
```bash
docker-compose up -d```

Access services:
Keycloak Admin Console: `http://localhost:8080`
- Username: admin
- Password: admin123
MongoExpress UI: `http://localhost:8081`
- Username: admin
- Password: admin

Docker logs for each service can be viewed with:
```
docker logs -f hypesoft-keycloak
docker logs -f hypesoft-mongo
docker logs -f mongo-express
```
--- 

### Running Locally (Without Docker)

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

## Architecture

### Backend
- Pattern: Clean Architecture + DDD
- Command/Query Handling: CQRS with MediatR
- Database: MongoDB (Repository pattern, Unit-of-Work if applicable)
- Validation: FluentValidation for DTOs & commands
- Mapping: AutoMapper for entity-DTO transformations
- Logging: Serilog
- Endpoints: Swagger documentation for API exploration
- Security: Role-based route protection, Keycloak OAuth2/OpenID Connect

### Frontend
- Framework: Next.js 14 (App Router) with React 18 + TypeScript
- Styling: TailwindCSS with shadcn/ui components
- Forms & Validation: React Hook Form + Zod
- Data Fetching & Caching: TanStack Query
- Charts & Dashboards: Recharts
- Routing & Authentication: Protected routes integrated with Keycloak
- State Management: Minimal local state + React Query cache

### Additional Notes
- Modular component structure for reusability
- Reusable hooks for API interactions
 
--- 

 ## Dev Mode with Seed Data
 
 When running the backend in development mode, mock data for products and categories is automatically seeded into MongoDB. This allows you to:
 Quickly test frontend features
Preview dashboards and product/category lists
Work with realistic example data without manual creation
