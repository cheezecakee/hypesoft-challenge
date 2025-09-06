'use client';

import { useRouter, useParams } from 'next/navigation';
import { MainLayout } from '@/components/layout/MainLayout';
import { ProductForm } from '@/components/forms/ProductForm';
import { useProduct, useUpdateProduct } from '@/hooks/useProducts';
import { useCategories } from '@/hooks/useCategories';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import type { UpdateProductDto } from '@/types';

export default function EditProductPage() {
    const router = useRouter();
    const params = useParams();
    const productId = params?.id as string;

    const { data: product, isLoading, error } = useProduct(productId);
    const { data: categoriesData } = useCategories();
    const categories = categoriesData || [];

    const updateProduct = useUpdateProduct();

    const handleSubmit = async (data: UpdateProductDto) => {
        try {
            await updateProduct.mutateAsync({ id: productId, data });
            router.push('/products');
        } catch (error) {
            // Error handling is done in mutation hook
        }
    };

    const handleCancel = () => router.push('/products');

    if (error) {
        return (
            <MainLayout>
                <div className="text-center p-8 text-red-600">
                    Product not found or failed to load
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
                    <h2 className="text-3xl font-bold tracking-tight">Edit Product</h2>
                    <p className="text-muted-foreground">
                        Update product information and organization
                    </p>
                </div>

                <Card className="max-w-2xl">
                    <CardHeader>
                        <CardTitle>Product Details</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <ProductForm<UpdateProductDto>
                            initialData={product}
                            categories={categories}
                            onSubmit={handleSubmit}
                            onCancel={handleCancel}
                            isLoading={updateProduct.isPending}
                        />
                    </CardContent>
                </Card>
            </div>
        </MainLayout>
    );
}

