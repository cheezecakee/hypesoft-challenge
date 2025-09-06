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

**⚠️ Note:** Product **stock quantity field** is implemented in the backend but **not yet fully wired on the frontend**. Low-stock indicators and stock update forms still need to be integrated.

---

## Getting Started

### Prerequisites
- Docker Desktop 4.0+  
- Node.js 18+  
- .NET 9 SDK  
- Git  

### Environment Variables

Frontend and backend require environment variables for API URLs and Keycloak integration:

```bash
# Frontend
NEXT_PUBLIC_API_URL=http://localhost:5000/api
NEXT_PUBLIC_KEYCLOAK_URL=http://localhost:8080
NEXT_PUBLIC_KEYCLOAK_REALM=Hypesoft
NEXT_PUBLIC_KEYCLOAK_CLIENT_ID=frontend

--- 

## Installation 
### **Clone the Repository**
```bash
git clone https://github.com/cheezecakee/hypesoft-challenge.git
cd hypesoft-challenge```

## Using Docker
The application can be run locally with Docker Compose:

```bash
docker-compose up -d```


## Running Locally (Without Docker)
Frontend
```
cd frontend/hypesoft
npm install
npm run dev
```

```
cd backend/Hypesoft.API
dotnet restore
dotnet run
```

Swagger UI: `http://localhost:5000/swagger`

---

## Architecture

Backend: Clean Architecture + DDD
Frontend: Modular structure with reusable components and hooks


