import { useQuery } from '@tanstack/react-query';
import { useAuth } from '@/hooks/useAuth';
import { createDashboardApi, createProductsApi } from '@/services';
import { queryKeys } from '@/lib/queryClient';

export const useDashboardStats = () => {
    const { makeRequest } = useAuth();
    const dashboardApi = createDashboardApi(makeRequest);

    return useQuery({
        queryKey: queryKeys.dashboardStats(),
        queryFn: () => dashboardApi.getStats(),
        staleTime: 1000 * 60 * 2, // 2 minutes
        refetchInterval: 1000 * 60 * 5, // Auto-refresh every 5 minutes
        enabled: !!makeRequest, // Only run when we have the API function
    });
};

export const useProductsByCategory = () => {
    const { makeRequest } = useAuth();
    const dashboardApi = createDashboardApi(makeRequest);

    return useQuery({
        queryKey: queryKeys.productsByCategory(),
        queryFn: () => dashboardApi.getProductsByCategory(),
        staleTime: 1000 * 60 * 5, // 5 minutes
        enabled: !!makeRequest, // Only run when we have the API function
    });
};

export const useLowStockProducts = () => {
    const { makeRequest } = useAuth();
    const productsApi = createProductsApi(makeRequest);

    return useQuery({
        queryKey: queryKeys.lowStockProducts(), // Fixed: use consistent query key
        queryFn: () => productsApi.getLowStockProducts(),
        staleTime: 1000 * 60 * 5, // 5 minutes
        enabled: !!makeRequest, // Only run when we have the API function
    });
};
