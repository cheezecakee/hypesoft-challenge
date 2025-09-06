'use client';

import { useRouter } from 'next/navigation';
import { MainLayout } from '@/components/layout/MainLayout';
import { CategoryForm } from '@/components/forms/CategoryForm';
import { useCreateCategory } from '@/hooks/useCategories';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import type { CreateCategoryDto } from '@/types';

export default function CreateCategoryPage() {
    const router = useRouter();
    const createCategory = useCreateCategory();

    const handleSubmit = async (data: CreateCategoryDto) => {
        try {
            await createCategory.mutateAsync(data);
            router.push('/categories');
        } catch (error) {
        }
    };

    const handleCancel = () => {
        router.push('/categories');
    };

    return (
        <MainLayout>
            <div className="space-y-6">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Create Category</h2>
                    <p className="text-muted-foreground">
                        Add a new product category to organize your inventory
                    </p>
                </div>

                <Card className="max-w-2xl">
                    <CardHeader>
                        <CardTitle>Category Details</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <CategoryForm
                            onSubmit={handleSubmit}
                            onCancel={handleCancel}
                            isLoading={createCategory.isPending}
                        />
                    </CardContent>
                </Card>
            </div>
        </MainLayout>
    );
}
