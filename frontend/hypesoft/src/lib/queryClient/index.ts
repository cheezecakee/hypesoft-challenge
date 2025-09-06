import { QueryClient } from '@tanstack/react-query';

export const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            staleTime: 1000 * 60 * 5, // 5 minutes
            gcTime: 1000 * 60 * 10, // 10 minutes
            retry: (failureCount, error: any) => {
                if (error?.response?.status >= 400 && error?.response?.status < 500) {
                    if ([408, 429].includes(error.response.status)) {
                        return failureCount < 2;
                    }
                    return false;
                }
                return failureCount < 3;
            },
            refetchOnWindowFocus: false,
        },
        mutations: {
            retry: false,
        },
    },
});

export const queryKeys = {
    // Products
    products: ['products'] as const,
    productsList: (params?: any) => [...queryKeys.products, 'list', params] as const,
    product: (id: string) => [...queryKeys.products, 'detail', id] as const,
    productsSearch: (query: string) => [...queryKeys.products, 'search', query] as const,
    lowStockProducts: () => [...queryKeys.products, 'low-stock'] as const,

    // Categories  
    categories: ['categories'] as const,
    categoriesList: () => [...queryKeys.categories, 'list'] as const,
    category: (id: string) => [...queryKeys.categories, 'detail', id] as const,

    // Dashboard
    dashboard: ['dashboard'] as const,
    dashboardStats: () => [...queryKeys.dashboard, 'stats'] as const,
    productsByCategory: () => [...queryKeys.dashboard, 'products-by-category'] as const,

    // Health
    health: ['health'] as const,
    healthCheck: () => [...queryKeys.health, 'check'] as const,
};
