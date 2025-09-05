import apiClient from '@/lib/api';
import {
    Product,
    Category,
    CreateProductDto,
    UpdateProductDto,
    CreateCategoryDto,
    UpdateCategoryDto,
    DashboardStats,
    ProductsQueryParams,
    PaginatedResponse,
    ApiResponse,
} from '@/types';

export const productsApi = {
    getProducts: async (params?: ProductsQueryParams): Promise<PaginatedResponse<Product>> => {
        const response = await apiClient.get('/products', { params });
        return response.data;
    },

    getProduct: async (id: string): Promise<Product> => {
        const response = await apiClient.get(`/products/${id}`);
        return response.data;
    },

    createProduct: async (data: CreateProductDto): Promise<Product> => {
        const response = await apiClient.post('/products', data);
        return response.data;
    },

    updateProduct: async (id: string, data: UpdateProductDto): Promise<Product> => {
        const response = await apiClient.put(`/products/${id}`, data);
        return response.data;
    },

    deleteProduct: async (id: string): Promise<void> => {
        await apiClient.delete(`/products/${id}`);
    },

    updateStock: async (id: string, stockQuantity: number): Promise<Product> => {
        const response = await apiClient.patch(`/products/${id}/stock`, { stockQuantity });
        return response.data;
    },

    searchProducts: async (query: string): Promise<Product[]> => {
        const response = await apiClient.get('/products/search', {
            params: { query }
        });
        return response.data;
    },

    getLowStockProducts: async (): Promise<Product[]> => {
        const response = await apiClient.get('/products/low-stock');
        return response.data;
    },
};

export const categoriesApi = {
    getCategories: async (): Promise<Category[]> => {
        const response = await apiClient.get('/categories');
        return response.data;
    },

    getCategory: async (id: string): Promise<Category> => {
        const response = await apiClient.get(`/categories/${id}`);
        return response.data;
    },

    createCategory: async (data: CreateCategoryDto): Promise<Category> => {
        const response = await apiClient.post('/categories', data);
        return response.data;
    },

    updateCategory: async (id: string, data: UpdateCategoryDto): Promise<Category> => {
        const response = await apiClient.put(`/categories/${id}`, data);
        return response.data;
    },

    deleteCategory: async (id: string): Promise<void> => {
        await apiClient.delete(`/categories/${id}`);
    },
};

export const dashboardApi = {
    getStats: async (): Promise<DashboardStats> => {
        const response = await apiClient.get('/dashboard/stats');
        return response.data;
    },

    getProductsByCategory: async (): Promise<{ categoryName: string; count: number }[]> => {
        const response = await apiClient.get('/dashboard/products-by-category');
        return response.data;
    },
};

export const healthApi = {
    checkHealth: async (): Promise<{ status: string; timestamp: string }> => {
        const response = await apiClient.get('/health');
        return response.data;
    },
};
