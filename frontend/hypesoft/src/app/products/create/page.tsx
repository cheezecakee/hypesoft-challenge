
'use client';

import { useRouter } from 'next/navigation';
import { MainLayout } from '@/components/layout/MainLayout';
import { ProductForm } from '@/components/forms/ProductForm';
import { useCreateProduct } from '@/hooks/useProducts';
import { useCategories } from '@/hooks/useCategories';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import type { CreateProductDto } from '@/types';

export default function CreateProductPage() {
    const router = useRouter();
    const createProduct = useCreateProduct();
    const { data: categoriesData, isLoading: isCategoriesLoading } = useCategories();
    const categories = categoriesData || [];

    const handleSubmit = async (data: CreateProductDto) => {
        try {
            await createProduct.mutateAsync(data);
            router.push('/products');
        } catch (error) {
            // Error handled in mutation hook
        }
    };

    const handleCancel = () => router.push('/products');

    if (isCategoriesLoading) {
        return (
            <MainLayout>
                <Skeleton className="h-8 w-64 mb-2" />
                <Skeleton className="h-4 w-96" />
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
            </MainLayout>
        );
    }

    return (
        <MainLayout>
            <div className="space-y-6">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Create Product</h2>
                    <p className="text-muted-foreground">
                        Add a new product to organize your inventory
                    </p>
                </div>

                <Card className="max-w-2xl">
                    <CardHeader>
                        <CardTitle>Product Details</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <ProductForm<CreateProductDto>
                            categories={categories}
                            onSubmit={handleSubmit}
                            onCancel={handleCancel}
                            isLoading={createProduct.isPending}
                        />
                    </CardContent>
                </Card>
            </div>
        </MainLayout>
    );
}

