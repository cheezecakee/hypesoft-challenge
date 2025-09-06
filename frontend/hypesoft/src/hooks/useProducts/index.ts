import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { useAuth } from '@/hooks/useAuth';
import { createProductsApi } from '@/services';
import { queryKeys } from '@/lib/queryClient';
import type {
    CreateProductDto,
    UpdateProductDto,
    ProductsQueryParams
} from '@/types';

export const useProducts = (params?: ProductsQueryParams) => {
    const { makeRequest } = useAuth();
    const productsApi = createProductsApi(makeRequest);

    return useQuery({
        queryKey: queryKeys.productsList(params),
        queryFn: () => productsApi.getProducts(params),
        staleTime: 1000 * 60 * 2, // 2 minutes
        enabled: !!makeRequest, // Only run when we have the API function
    });
};

export const useProduct = (id: string) => {
    const { makeRequest } = useAuth();
    const productsApi = createProductsApi(makeRequest);

    return useQuery({
        queryKey: queryKeys.product(id),
        queryFn: () => productsApi.getProduct(id),
        enabled: !!id && !!makeRequest,
    });
};

export const useProductSearch = (query: string) => {
    const { makeRequest } = useAuth();
    const productsApi = createProductsApi(makeRequest);

    return useQuery({
        queryKey: queryKeys.productsSearch(query),
        queryFn: () => productsApi.searchProducts(query),
        enabled: query.length > 2 && !!makeRequest, // Only search with 3+ characters and when authenticated
        staleTime: 1000 * 30, // 30 seconds
    });
};

export const useLowStockProducts = () => {
    const { makeRequest } = useAuth();
    const productsApi = createProductsApi(makeRequest);

    return useQuery({
        queryKey: queryKeys.lowStockProducts(),
        queryFn: () => productsApi.getLowStockProducts(),
        enabled: !!makeRequest, // Only run when we have the API function
    });
};

export const useCreateProduct = () => {
    const queryClient = useQueryClient();
    const { makeRequest } = useAuth();
    const productsApi = createProductsApi(makeRequest);

    return useMutation({
        mutationFn: (data: CreateProductDto) => productsApi.createProduct(data),
        onSuccess: (_newProduct) => {
            queryClient.invalidateQueries({ queryKey: queryKeys.products });
            queryClient.invalidateQueries({ queryKey: queryKeys.dashboard });

            toast.success('Product created successfully!');
        },
        onError: (error: any) => {
            const message = error?.response?.data?.message || 'Failed to create product';
            toast.error(message);
        },
    });
};

export const useUpdateProduct = () => {
    const queryClient = useQueryClient();
    const { makeRequest } = useAuth();
    const productsApi = createProductsApi(makeRequest);

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateProductDto }) =>
            productsApi.updateProduct(id, data),
        onSuccess: (updatedProduct, { id }) => {
            // Update the product in cache
            queryClient.setQueryData(queryKeys.product(id), updatedProduct);
            // Invalidate products list
            queryClient.invalidateQueries({ queryKey: queryKeys.products });
            // Invalidate dashboard stats
            queryClient.invalidateQueries({ queryKey: queryKeys.dashboard });

            toast.success('Product updated successfully!');
        },
        onError: (error: any) => {
            const message = error?.response?.data?.message || 'Failed to update product';
            toast.error(message);
        },
    });
};

export const useDeleteProduct = () => {
    const queryClient = useQueryClient();
    const { makeRequest } = useAuth();
    const productsApi = createProductsApi(makeRequest);

    return useMutation({
        mutationFn: (id: string) => productsApi.deleteProduct(id),
        onSuccess: (_, id) => {
            // Remove from cache
            queryClient.removeQueries({ queryKey: queryKeys.product(id) });
            // Invalidate products list
            queryClient.invalidateQueries({ queryKey: queryKeys.products });
            // Invalidate dashboard stats
            queryClient.invalidateQueries({ queryKey: queryKeys.dashboard });

            toast.success('Product deleted successfully!');
        },
        onError: (error: any) => {
            const message = error?.response?.data?.message || 'Failed to delete product';
            toast.error(message);
        },
    });
};

export const useUpdateProductStock = () => {
    const queryClient = useQueryClient();
    const { makeRequest } = useAuth();
    const productsApi = createProductsApi(makeRequest);

    return useMutation({
        mutationFn: ({ id, stockQuantity }: { id: string; stockQuantity: number }) =>
            productsApi.updateStock(id, stockQuantity),
        onSuccess: (_, { id }) => {
            queryClient.invalidateQueries({ queryKey: queryKeys.product(id) });
            queryClient.invalidateQueries({ queryKey: queryKeys.products });
            queryClient.invalidateQueries({ queryKey: queryKeys.dashboard });
            queryClient.invalidateQueries({ queryKey: queryKeys.lowStockProducts() });

            toast.success('Stock updated successfully!');
        },
        onError: (error: any) => {
            const message = error?.response?.data?.message || 'Failed to update stock';
            toast.error(message);
        },
    });
};
