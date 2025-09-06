export interface ApiResponse<T> {
    data: T;
    message?: string;
    success: boolean;
}

export interface PaginatedResponse<T> {
    data: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
}

export interface Money {
    amount: number;
    currency: string;
}

// Product Types
export interface Product {
    id: string;
    name: string;
    description: string;
    price: Money;
    categoryId: string;
    stockQuantity: number;
    category?: Category;
    createdAt: string;
    updatedAt: string;
}

// Frontend display product
export interface ProductDisplay {
    id: string;
    name: string;
    description: string;
    price: number;
    categoryId: string;
    stockQuantity: number;
    category?: Category;
    createdAt: string;
    updatedAt: string;
}

export interface CreateProductDto {
    name: string;
    description: string;
    price: number;
    categoryId: string;
    stockQuantity: number;
}

export interface UpdateProductDto {
    name?: string;
    description?: string;
    price?: number;
    categoryId?: string;
    stockQuantity?: number;
}

// Category Types
export interface Category {
    id: string;
    name: string;
    description?: string;
    createdAt: string;
    updatedAt: string;
}

export interface CreateCategoryDto {
    name: string;
    description?: string;
}

export interface UpdateCategoryDto {
    name?: string;
    description?: string;
}

// Dashboard Types
export interface DashboardStats {
    totalProducts: number;
    totalStockValue: number;
    lowStockProductCount: number;
    totalCategories: number;
}

export interface CategoryStats {
    categoryId: string;
    categoryName: string;
    productCount: number;
    totalValue: number;
}

// Query Parameters
export interface ProductsQueryParams {
    pageNumber?: number;
    pageSize?: number;
    search?: string;
    categoryId?: string;
    sortBy?: 'name' | 'price' | 'stock';
    sortOrder?: 'asc' | 'desc';
}

// Authentication Types (Keycloak)
export interface User {
    id: string;
    username: string;
    email: string;
    roles: string[];
}

export interface AuthTokens {
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
}
