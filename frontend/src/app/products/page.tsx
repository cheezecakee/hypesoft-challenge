'use client';

import { useState } from 'react';
import { MainLayout } from '@/components/layout/MainLayout';
import { ProductsTable } from '@/components/products/ProductsTable';
import { ProductsHeader } from '@/components/products/ProductsHeader';
import { ProductsFilters } from '@/components/products/ProductsFilters';
import type { ProductsQueryParams } from '@/types';
import { ProtectedRoute } from '@/components/auth/ProtectedRoute';

export default function ProductsPage() {
    const [filters, setFilters] = useState<ProductsQueryParams>({
        page: 1,
        pageSize: 10,
        search: '',
        categoryId: '',
        sortBy: 'name',
        sortOrder: 'asc',
    });

    const handleFiltersChange = (newFilters: Partial<ProductsQueryParams>) => {
        setFilters(prev => ({
            ...prev,
            ...newFilters,
            page: newFilters.page ?? 1,
        }));
    };

    return (
        <ProtectedRoute>
            <MainLayout>
                <div className="space-y-6">
                    <ProductsHeader />
                    <ProductsFilters filters={filters} onFiltersChange={handleFiltersChange} />
                    <ProductsTable filters={filters} onFiltersChange={handleFiltersChange} />
                </div>
            </MainLayout>
        </ProtectedRoute>
    );
}
