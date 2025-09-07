[Leia a versão em português](README-PT.md) | [Read the English version](README.md)

# Hypesoft Challenge - Complete Setup Guide

## Overview

This project is a full-stack product management system demonstrating modern architecture, clean code practices, and advanced frontend/backend integration.

## Prerequisites

- Docker and Docker Compose installed
- Git
- Node.js 18+ and npm (for local frontend development)
- .NET 9 SDK (for local backend development)
- curl or Postman for API testing

---

## Architecture

### Backend
- **Pattern**: Clean Architecture + DDD
- **Command/Query Handling**: CQRS with MediatR
- **Database**: MongoDB
- **Validation**: FluentValidation for DTOs & commands
- **Mapping**: AutoMapper for entity-DTO transformations
- **Logging**: Serilog
- **Endpoints**: Swagger documentation for API exploration
- **Security**: Role-based route protection, Keycloak OAuth2/OpenID Connect

### Frontend
- **Framework**: Next.js 14 (App Router) with React 18 + TypeScript
- **Styling**: TailwindCSS with shadcn/ui components
- **Forms & Validation**: React Hook Form + Zod
- **Data Fetching & Caching**: TanStack Query
- **Charts & Dashboards**: Recharts
- **Routing & Authentication**: Protected routes integrated with Keycloak
- **State Management**: Minimal local state + React Query cache

### Additional Notes
- Modular component structure for reusability
- Reusable hooks for API interactions

---

## Quick Start (Recommended)

### Full System (All Services)
```bash
git clone https://github.com/cheezecakee/hypesoft-challenge.git
cd hypesoft-challenge
docker compose --profile full up -d
```

### Infrastructure Only (No Backend/Frontend)
```bash
docker compose up -d
```

### Backend + Infrastructure
```bash
docker compose --profile backend up -d
```

### Frontend + Infrastructure
```bash
docker compose --profile frontend up -d
```

This will start:
- **Keycloak**: `http://localhost:8080` (pre-configured)
- **Backend API**: `http://localhost:5113` (if using backend profile)
- **MongoDB**: `localhost:27017`
- **Mongo Express**: `http://localhost:8081`

## Dev Mode with Seed Data

When running the backend in development mode, mock data for products and categories is automatically seeded into MongoDB. This allows you to:
- Quickly test frontend features
- Preview dashboards and product/category lists
- Work with realistic example data without manual creation

---

## Pre-configured Authentication

The system comes with a pre-configured Keycloak realm that includes:
- **Realm**: `hypesoft`
- **Client**: `hypesoft-api`
- **Test Users**:
  - **Admin**: `admin@hypesoft.com` / `admin123` (Admin role)
  - **Manager**: `manager@hypesoft.com` / `manager123` (Manager role)
  - **User**: `user@hypesoft.com` / `user123` (User role)

### Service URLs & Credentials

| Service | URL | Credentials |
|---------|-----|-------------|
| **Keycloak Admin** | http://localhost:8080 | admin / admin |
| **Backend API** | http://localhost:5113 | Bearer token required |
| **Swagger UI** | http://localhost:5113/swagger | Bearer token required |
| **Mongo Express** | http://localhost:8081 | admin / admin |
| **Frontend** | http://localhost:3000 | Keycloak SSO |

---

## Authentication & API Testing

### Getting Bearer Token

```bash
# For Admin User
curl -X POST "http://localhost:8080/realms/hypesoft/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=hypesoft-api" \
  -d "client_secret=dLGltIUV18VMwomohmQsfa3blaOehFR0" \
  -d "username=admin@hypesoft.com" \
  -d "password=admin123"

# For Manager User
curl -X POST "http://localhost:8080/realms/hypesoft/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=hypesoft-api" \
  -d "client_secret=dLGltIUV18VMwomohmQsfa3blaOehFR0" \
  -d "username=manager@hypesoft.com" \
  -d "password=manager123"

# For Regular User
curl -X POST "http://localhost:8080/realms/hypesoft/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=hypesoft-api" \
  -d "client_secret=dLGltIUV18VMwomohmQsfa3blaOehFR0" \
  -d "username=user@hypesoft.com" \
  -d "password=user123"
```
⚠️ **Important:** If running the backend in Docker, Keycloak is also in Docker. To get a token, you must run curl inside the same Docker network:
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

### Using the Token

#### In Swagger UI:
1. Access Swagger: `http://localhost:5113/swagger`
2. Click "Authorize" button
3. Enter: `Bearer {your_access_token}`
4. Click "Authorize"

#### In curl:
```bash
curl -H "Authorization: Bearer {your_access_token}" \
     http://localhost:5113/api/Dashboard/stats
```

---

## API Documentation

### Available Endpoints

#### Public Endpoints (No Authentication Required)
- `GET /api/Health` - Health check endpoint
- Swagger documentation at `/swagger`

#### Dashboard Endpoints (Authenticated Users Only)
- `GET /api/Dashboard/stats` - Dashboard statistics
- `GET /api/Dashboard/products-by-category` - Products grouped by category

#### Product Management (Admin/Manager Only)
- `GET /api/Products` - List products with pagination
- `POST /api/Products` - Create new product
- `PUT /api/Products/{id}` - Update product
- `DELETE /api/Products/{id}` - Delete product
- `PATCH /api/Products/{id}/stock` - Update product stock level
- `GET /api/Products/low-stock` - Get products with low stock

#### Category Management (Admin/Manager Only)
- `GET /api/Categories` - List categories
- `POST /api/Categories` - Create category
- `PUT /api/Categories/{id}` - Update category
- `DELETE /api/Categories/{id}` - Delete category

### Detailed Authorization Requirements

#### **Public Access**
- Health check endpoints
- Swagger documentation

#### **Authenticated Users** (Any valid token)
- Dashboard viewing (`/api/Dashboard/*`)
- Read-only access to view statistics and charts
- Regular users can only view dashboard data

#### **Admin/Manager Only** (Requires Admin or Manager role)
- **All CRUD operations** on Products and Categories
- **Stock management** operations
- **Product creation, modification, and deletion**
- **Category management**
- **Inventory control features**

**Note**: Regular authenticated users are restricted to dashboard viewing only. Administrative functions require elevated privileges (Admin or Manager roles).

---

## Running Services Individually

### Backend Only
```bash
cd backend/Hypesoft.API
dotnet restore
dotnet run
```
- Runs on: `http://localhost:5113`
- Requires: MongoDB and Keycloak running

### Frontend Only
```bash
cd frontend/
npm install
npm run dev
```
- Runs on: `http://localhost:3000`
- Requires: Backend API running

### Infrastructure Services Only
```bash
# Start MongoDB, Keycloak, and Mongo Express
docker compose up -d mongodb keycloak mongo-express
```

---

## Troubleshooting

### Common Issues

#### 1. Services Not Starting
**Solution**: Check if all required ports are available and start services step by step:
```bash
docker compose ps
docker compose logs [service-name]
```

#### 2. Authentication Issues
**Problem**: Token not working or expired.

**Solution**: Get a fresh token using the curl commands above. Tokens expire after 5 minutes by default.

#### 3. Database Connection Issues
**Problem**: Backend can't connect to MongoDB.

**Solution**: Ensure MongoDB is running and accessible:
```bash
docker compose logs mongodb
docker compose restart backend
```

#### 4. Port Conflicts
**Problem**: Ports already in use.

**Solution**: Stop conflicting services or modify ports in docker-compose.yml

### Viewing Logs
```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f backend
docker compose logs -f keycloak
docker compose logs -f mongodb
```

### Resetting Everything
```bash
# Stop and remove all containers and volumes
docker compose down -v

# Start fresh
docker compose --profile full up -d
```

---

## Development Workflow

### 1. Start Infrastructure Only
```bash
docker compose up -d
```

### 2. Run Backend Locally (Optional)
```bash
cd backend/Hypesoft.API
dotnet run
```

### 3. Run Frontend Locally (Optional)
```bash
cd frontend/
npm run dev
```

### 4. Make Changes
- Backend changes require restart
- Frontend has hot reload
- Database changes persist in Docker volumes

---

## Support

### Getting Help
1. Check the logs: `docker compose logs [service-name]`
2. Verify service status: `docker compose ps`
3. Test connectivity: `curl http://localhost:5113/api/Health`
4. Verify authentication with test users
5. Check database in Mongo Express

### Known Limitations
- Development setup uses HTTP (not HTTPS)
- Client secrets are exposed in docker-compose (development only)
- Frontend service is commented out pending Dockerfile creation

