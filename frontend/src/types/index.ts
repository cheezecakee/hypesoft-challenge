export interface ApiResponse<T> {
    data: T;
    message?: string;
    success: boolean;
}

export interface PaginatedResponse<T> {
    products: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

export interface Money {
    amount: number;
    currency: string;
}

export interface Product {
    id: string;
    name: string;
    description: string;
    price: number;
    currency: string;
    categoryId: string;
    categoryName: string;
    stockQuantity: number;
    isLowStock: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface ProductDisplay {
    id: string;
    name: string;
    description: string;
    price: number;
    currency: string;
    categoryId: string;
    categoryName: string;
    stockQuantity: number;
    isLowStock: boolean;
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
    currency?: string;
    categoryId?: string;
    stockQuantity?: number;
}

export interface Category {
    id: string;
    name: string;
    description?: string;
    productCount: number;
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

export interface ProductsQueryParams {
    page?: number;
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
