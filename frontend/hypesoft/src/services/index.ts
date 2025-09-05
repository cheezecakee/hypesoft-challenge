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
} from '@/types';

// Type for the API function from useAuth
type ApiFunction = <T = any>(config: any) => Promise<T>;

export const createProductsApi = (makeRequest: ApiFunction) => ({
    getProducts: async (params?: ProductsQueryParams): Promise<PaginatedResponse<Product>> => {
        return makeRequest({
            method: 'GET',
            url: '/Products',
            params,
        });
    },

    getProduct: async (id: string): Promise<Product> => {
        return makeRequest({
            method: 'GET',
            url: `/Products/${id}`,
        });
    },

    createProduct: async (data: CreateProductDto): Promise<Product> => {
        return makeRequest({
            method: 'POST',
            url: '/Products',
            data,
        });
    },

    updateProduct: async (id: string, data: UpdateProductDto): Promise<Product> => {
        return makeRequest({
            method: 'PUT',
            url: `/Products/${id}`,
            data,
        });
    },

    deleteProduct: async (id: string): Promise<void> => {
        return makeRequest({
            method: 'DELETE',
            url: `/Products/${id}`,
        });
    },

    updateStock: async (id: string, stockQuantity: number): Promise<Product> => {
        return makeRequest({
            method: 'PATCH',
            url: `/Products/${id}/stock`,
            data: { stockQuantity },
        });
    },

    searchProducts: async (query: string): Promise<Product[]> => {
        return makeRequest({
            method: 'GET',
            url: '/Products/search',
            params: { query },
        });
    },

    getLowStockProducts: async (): Promise<Product[]> => {
        return makeRequest({
            method: 'GET',
            url: '/Products/low-stock',
        });
    },
});

export const createCategoriesApi = (makeRequest: ApiFunction) => ({
    getCategories: async (): Promise<Category[]> => {
        return makeRequest({
            method: 'GET',
            url: '/Categories',
        });
    },

    getCategory: async (id: string): Promise<Category> => {
        return makeRequest({
            method: 'GET',
            url: `/Categories/${id}`,
        });
    },

    createCategory: async (data: CreateCategoryDto): Promise<Category> => {
        return makeRequest({
            method: 'POST',
            url: '/Categories',
            data,
        });
    },

    updateCategory: async (id: string, data: UpdateCategoryDto): Promise<Category> => {
        return makeRequest({
            method: 'PUT',
            url: `/Categories/${id}`,
            data,
        });
    },

    deleteCategory: async (id: string): Promise<void> => {
        return makeRequest({
            method: 'DELETE',
            url: `/Categories/${id}`,
        });
    },
});

export const createDashboardApi = (makeRequest: ApiFunction) => ({
    getStats: async (): Promise<DashboardStats> => {
        return makeRequest({
            method: 'GET',
            url: '/Dashboard/stats',
        });
    },

    getProductsByCategory: async (): Promise<{ categoryName: string; count: number }[]> => {
        return makeRequest({
            method: 'GET',
            url: '/Dashboard/products-by-category',
        });
    },
});

export const createHealthApi = (makeRequest: ApiFunction) => ({
    checkHealth: async (): Promise<{ status: string; timestamp: string }> => {
        return makeRequest({
            method: 'GET',
            url: '/Health',
        });
    },
});
