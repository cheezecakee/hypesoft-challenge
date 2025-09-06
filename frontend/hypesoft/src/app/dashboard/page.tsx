'use client';

import { ProtectedRoute } from '@/components/auth/ProtectedRoute';
import { MainLayout } from '@/components/layout/MainLayout';
import { StatsCards } from '@/components/dashboard/StatsCards';
import { ProductsByCategoryChart } from '@/components/dashboard/ProductsByCategoryChart';
import { LowStockTable } from '@/components/dashboard/LowStockTable';
import { ApiDebugger } from '@/components/debug/ApiDebugger';

export default function DashboardPage() {
    return (
        <ProtectedRoute>
            <MainLayout>
                <div className="space-y-6">
                    {/* Page header */}
                    <div>
                        <h2 className="text-3xl font-bold tracking-tight">Dashboard</h2>
                        <p className="text-muted-foreground">
                            Overview of your product inventory and key metrics
                        </p>
                    </div>

                    <ApiDebugger />

                    {/* Stats cards */}
                    <StatsCards />

                    {/* Charts and tables */}
                    <div className="grid gap-6 lg:grid-cols-2">
                        <ProductsByCategoryChart />
                        <LowStockTable />
                    </div>
                </div>
            </MainLayout>
        </ProtectedRoute>
    );
}
