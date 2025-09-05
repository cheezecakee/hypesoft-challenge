'use client';

import { useRouter, useParams } from 'next/navigation';
import { MainLayout } from '@/components/layout/MainLayout';
import { CategoryForm } from '@/components/forms/CategoryForm';
import { useCategory, useUpdateCategory } from '@/hooks/useCategories';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import type { UpdateCategoryDto } from '@/types';

export default function EditCategoryPage() {
    const router = useRouter();
    const params = useParams();
    const categoryId = params?.id as string;

    const { data: category, isLoading, error } = useCategory(categoryId);
    const updateCategory = useUpdateCategory();

    const handleSubmit = async (data: UpdateCategoryDto) => {
        try {
            await updateCategory.mutateAsync({ id: categoryId, data });
            router.push('/categories');
        } catch (error) {
            // Error is handled by the mutation
        }
    };

    const handleCancel = () => {
        router.push('/categories');
    };

    if (error) {
        return (
            <MainLayout>
                <div className="text-center p-8 text-red-600">
                    Category not found or failed to load
                </div>
            </MainLayout>
        );
    }

    if (isLoading) {
        return (
            <MainLayout>
                <div className="space-y-6">
                    <div>
                        <Skeleton className="h-8 w-64 mb-2" />
                        <Skeleton className="h-4 w-96" />
                    </div>
                    <Card className="max-w-2xl">
                        <CardHeader>
                            <Skeleton className="h-6 w-32" />
                        </CardHeader>
                        <CardContent>
                            <div className="space-y-4">
                                <Skeleton className="h-4 w-16" />
                                <Skeleton className="h-10 w-full" />
                                <Skeleton className="h-4 w-20" />
                                <Skeleton className="h-32 w-full" />
                            </div>
                        </CardContent>
                    </Card>
                </div>
            </MainLayout>
        );
    }

    return (
        <MainLayout>
            <div className="space-y-6">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Edit Category</h2>
                    <p className="text-muted-foreground">
                        Update category information and organization
                    </p>
                </div>

                <Card className="max-w-2xl">
                    <CardHeader>
                        <CardTitle>Category Details</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <CategoryForm
                            initialData={category}
                            onSubmit={handleSubmit}
                            onCancel={handleCancel}
                            isLoading={updateCategory.isPending}
                        />
                    </CardContent>
                </Card>
            </div>
        </MainLayout>
    );
}
