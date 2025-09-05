import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { categoriesApi } from '@/services';
import { queryKeys } from '@/lib/queryClient';
import type { Category, CreateCategoryDto, UpdateCategoryDto } from '@/types';

export const useCategories = () => {
    return useQuery({
        queryKey: queryKeys.categoriesList(),
        queryFn: () => categoriesApi.getCategories(),
        staleTime: 1000 * 60 * 10, // 10 minutes - categories don't change often
    });
};

export const useCategory = (id: string) => {
    return useQuery({
        queryKey: queryKeys.category(id),
        queryFn: () => categoriesApi.getCategory(id),
        enabled: !!id,
    });
};

export const useCreateCategory = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateCategoryDto) => categoriesApi.createCategory(data),
        onSuccess: (_newCategory) => {
            // Invalidate categories list
            queryClient.invalidateQueries({ queryKey: queryKeys.categories });
            // Invalidate dashboard stats (for category stats)
            queryClient.invalidateQueries({ queryKey: queryKeys.dashboard });

            toast.success('Category created successfully!');
        },
        onError: (error: any) => {
            const message = error?.response?.data?.message || 'Failed to create category';
            toast.error(message);
        },
    });
};

export const useUpdateCategory = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateCategoryDto }) =>
            categoriesApi.updateCategory(id, data),
        onSuccess: (updatedCategory, { id }) => {
            // Update the category in cache
            queryClient.setQueryData(queryKeys.category(id), updatedCategory);
            // Invalidate categories list
            queryClient.invalidateQueries({ queryKey: queryKeys.categories });
            // Invalidate products that might reference this category
            queryClient.invalidateQueries({ queryKey: queryKeys.products });
            // Invalidate dashboard stats
            queryClient.invalidateQueries({ queryKey: queryKeys.dashboard });

            toast.success('Category updated successfully!');
        },
        onError: (error: any) => {
            const message = error?.response?.data?.message || 'Failed to update category';
            toast.error(message);
        },
    });
};

export const useDeleteCategory = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (id: string) => categoriesApi.deleteCategory(id),
        onSuccess: (_, id) => {
            // Remove from cache
            queryClient.removeQueries({ queryKey: queryKeys.category(id) });
            // Invalidate categories list
            queryClient.invalidateQueries({ queryKey: queryKeys.categories });
            // Invalidate products (they might need to update category references)
            queryClient.invalidateQueries({ queryKey: queryKeys.products });
            // Invalidate dashboard stats
            queryClient.invalidateQueries({ queryKey: queryKeys.dashboard });

            toast.success('Category deleted successfully!');
        },
        onError: (error: any) => {
            const message = error?.response?.data?.message || 'Failed to delete category';
            toast.error(message);
        },
    });
};
