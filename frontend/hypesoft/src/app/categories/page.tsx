'use client';

import { useState } from 'react';
import { MainLayout } from '@/components/layout/MainLayout';
import { CategoriesTable } from '@/components/categories/CategoriesTable';
import { CategoriesHeader } from '@/components/categories/CategoriesHeader';
import { ProtectedRoute } from '@/components/auth/ProtectedRoute';

export default function CategoriesPage() {
    const [refreshKey, setRefreshKey] = useState(0);

    const handleRefresh = () => {
        setRefreshKey(prev => prev + 1);
    };

    return (
        <ProtectedRoute>
            <MainLayout>
                <div className="space-y-6">
                    <CategoriesHeader onRefresh={handleRefresh} />
                    <CategoriesTable key={refreshKey} onRefresh={handleRefresh} />
                </div>
            </MainLayout>
        </ProtectedRoute>
    );
}
