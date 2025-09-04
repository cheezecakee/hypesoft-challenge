# Hypesoft Challenge

This repository contains a full-stack take-home project for a candidate applying for a Full Stack Developer position.

## Overview

The application is a **ShopSense-style dashboard** with product and inventory management features. It demonstrates modern frontend and backend development practices, Clean Architecture, and DDD patterns.

## Features

### Product Management
- CRUD operations for products
- Product fields: name, description, price, category, stock quantity
- Basic search and filter by category

### Category System
- CRUD operations for categories
- Association with products

### Inventory Control
- Manage stock quantities
- Highlight low-stock products (<10 units)

### Dashboard
- Total products
- Total stock value
- Low-stock products list
- Products by category chart

### Authentication
- Keycloak integration (OAuth2/OpenID Connect)
- Route protection and role-based access

## Tech Stack

### Frontend
- React 18 + TypeScript
- Vite / Next.js 14
- TailwindCSS + Shadcn/ui
- React Query / TanStack Query
- React Hook Form + Zod
- Recharts / Chart.js

### Backend
- .NET 9 + C#
- Clean Architecture + DDD
- CQRS + MediatR
- Entity Framework Core + MongoDB
- FluentValidation, AutoMapper, Serilog

### Infrastructure
- MongoDB
- Keycloak
- Docker + Docker Compose
