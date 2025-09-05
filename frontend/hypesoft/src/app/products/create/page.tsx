'use client';

import { useRouter } from 'next/navigation';
import { MainLayout } from '@/components/layout/MainLayout';
import { ProductForm } from '@/components/forms/ProductForm';
import { useCreateProduct } from '@/hooks/useProducts';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import type { CreateProductDto } from '@/types';

export default function CreateProductPage() {
    const router = useRouter();
    const createProduct = useCreateProduct();

    const handleSubmit = async (data: CreateProductDto) => {
        try {
            await createProduct.mutateAsync(data);
            router.push('/products');
        } catch (error) {
        }
    };

    const handleCancel = () => {
        router.push('/products');
    };

    return (
        <MainLayout>
            <div className="space-y-6">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Create Product</h2>
                    <p className="text-muted-foreground">
                        Add a new product category to organize your inventory
                    </p>
                </div>

                <Card className="max-w-2xl">
                    <CardHeader>
                        <CardTitle>Product Details</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <ProductForm
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
