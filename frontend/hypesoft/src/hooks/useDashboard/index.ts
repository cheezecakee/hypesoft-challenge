import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '@/services';
import { queryKeys } from '@/lib/queryClient';

export const useDashboardStats = () => {
    return useQuery({
        queryKey: queryKeys.dashboardStats(),
        queryFn: () => dashboardApi.getStats(),
        staleTime: 1000 * 60 * 2, // 2 minutes
        refetchInterval: 1000 * 60 * 5, // Auto-refresh every 5 minutes
    });
};

export const useProductsByCategory = () => {
    return useQuery({
        queryKey: queryKeys.productsByCategory(),
        queryFn: () => dashboardApi.getProductsByCategory(),
        staleTime: 1000 * 60 * 5, // 5 minutes
    });
};
