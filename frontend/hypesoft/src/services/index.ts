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

// Type for the API function from useAuth
type ApiFunction = <T = any>(config: any) => Promise<T>;

// Data transformation utilities
const transformProduct = (product: Product): ProductDisplay => ({
    ...product,
    price: product.price?.amount || 0, // Transform Money object to number
});

const transformProducts = (products: Product[]): ProductDisplay[] =>
    products.map(transformProduct);

const transformPaginatedProducts = (response: PaginatedResponse<Product>): PaginatedResponse<ProductDisplay> => ({
    ...response,
    data: transformProducts(response.data),
});

// Transform backend DTO for API calls
const transformCreateProductDto = (data: CreateProductDto) => ({
    ...data,
    // Backend expects the price as a number, which gets converted to Money object
});

const transformUpdateProductDto = (data: UpdateProductDto) => ({
    ...data,
    // Backend expects the price as a number, which gets converted to Money object  
});

export const createProductsApi = (makeRequest: ApiFunction) => ({
    getProducts: async (params?: ProductsQueryParams): Promise<PaginatedResponse<ProductDisplay>> => {
        const response = await makeRequest<PaginatedResponse<Product>>({
            method: 'GET',
            url: '/Products',
            params: {
                ...params,
                pageNumber: params?.pageNumber || 1,
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
            data: transformCreateProductDto(data),
        });
        return transformProduct(product);
    },

    updateProduct: async (id: string, data: UpdateProductDto): Promise<ProductDisplay> => {
        const product = await makeRequest<Product>({
            method: 'PUT',
            url: `/Products/${id}`,
            data: transformUpdateProductDto(data),
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
            data: { stockQuantity },
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
