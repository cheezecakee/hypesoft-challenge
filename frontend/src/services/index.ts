import {
    Product,
    ProductDisplay,
    Category,
    CreateProductDto,
    UpdateProductDto,
    CreateCategoryDto,
    UpdateCategoryDto,
    DashboardStats,
    ProductsQueryParams,
    PaginatedResponse,
    CategoryStats,
} from '@/types';

type ApiFunction = <T = any>(config: any) => Promise<T>;

// Data transformation utilities
const transformProduct = (product: Product): ProductDisplay => ({
    ...product,
});

const transformProducts = (products: Product[]): ProductDisplay[] =>
    products.map(transformProduct);

const transformPaginatedProducts = (response: PaginatedResponse<Product>): PaginatedResponse<ProductDisplay> => ({
    ...response,
    products: transformProducts(response.products),
});

export const createProductsApi = (makeRequest: ApiFunction) => ({
    getProducts: async (params?: ProductsQueryParams): Promise<PaginatedResponse<ProductDisplay>> => {
        const response = await makeRequest<PaginatedResponse<Product>>({
            method: 'GET',
            url: '/Products',
            params: {
                ...params,
                page: params?.page || 1,
                pageSize: params?.pageSize || 10,
            },
        });
        return transformPaginatedProducts(response);
    },

    getProduct: async (id: string): Promise<ProductDisplay> => {
        const product = await makeRequest<Product>({
            method: 'GET',
            url: `/Products/${id}`,
        });
        return transformProduct(product);
    },

    createProduct: async (data: CreateProductDto): Promise<ProductDisplay> => {
        const product = await makeRequest<Product>({
            method: 'POST',
            url: '/Products',
            data,
        });
        return transformProduct(product);
    },

    updateProduct: async (id: string, data: UpdateProductDto): Promise<ProductDisplay> => {
        const product = await makeRequest<Product>({
            method: 'PUT',
            url: `/Products/${id}`,
            data,
        });
        return transformProduct(product);
    },

    deleteProduct: async (id: string): Promise<void> => {
        return makeRequest({
            method: 'DELETE',
            url: `/Products/${id}`,
        });
    },

    updateStock: async (id: string, stockQuantity: number): Promise<ProductDisplay> => {
        const product = await makeRequest<Product>({
            method: 'PATCH',
            url: `/Products/${id}/stock`,
            data: { id, stockQuantity },
        });
        return transformProduct(product);
    },

    searchProducts: async (query: string): Promise<ProductDisplay[]> => {
        const products = await makeRequest<Product[]>({
            method: 'GET',
            url: '/Products/search',
            params: { query },
        });
        return transformProducts(products);
    },

    getLowStockProducts: async (): Promise<ProductDisplay[]> => {
        const products = await makeRequest<Product[]>({
            method: 'GET',
            url: '/Products/low-stock',
        });
        return transformProducts(products);
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

    getProductsByCategory: async (): Promise<CategoryStats[]> => {
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
