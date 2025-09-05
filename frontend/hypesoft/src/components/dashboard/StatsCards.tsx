'use client';

import { Package, DollarSign, AlertTriangle, Tags } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useDashboardStats } from '@/hooks/useDashboard';

export function StatsCards() {
    const { data: stats, isLoading, error } = useDashboardStats();

    if (error) {
        return (
            <div className="text-center p-8 text-red-600">
                Failed to load dashboard statistics
            </div>
        );
    }

    if (isLoading) {
        return (
            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
                {Array.from({ length: 4 }).map((_, i) => (
                    <Card key={i}>
                        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                            <Skeleton className="h-4 w-[120px]" />
                            <Skeleton className="h-4 w-4 rounded" />
                        </CardHeader>
                        <CardContent>
                            <Skeleton className="h-7 w-[100px] mb-1" />
                            <Skeleton className="h-3 w-[140px]" />
                        </CardContent>
                    </Card>
                ))}
            </div>
        );
    }

    const statsData = [
        {
            title: 'Total Products',
            value: stats?.totalProducts || 0,
            description: 'Active products in inventory',
            icon: Package,
            color: 'text-blue-600',
            bgColor: 'bg-blue-100',
        },
        {
            title: 'Total Stock Value',
            value: `$${(stats?.totalStockValue || 0).toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            })}`,
            description: 'Total inventory value',
            icon: DollarSign,
            color: 'text-green-600',
            bgColor: 'bg-green-100',
        },
        {
            title: 'Low Stock Items',
            value: stats?.lowStockProducts?.length || 0,
            description: 'Products with < 10 units',
            icon: AlertTriangle,
            color: 'text-orange-600',
            bgColor: 'bg-orange-100',
        },
        {
            title: 'Categories',
            value: stats?.categoriesStats?.length || 0,
            description: 'Product categories',
            icon: Tags,
            color: 'text-purple-600',
            bgColor: 'bg-purple-100',
        },
    ];

    return (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            {statsData.map((stat, index) => (
                <Card key={index} className="hover:shadow-md transition-shadow">
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium text-muted-foreground">
                            {stat.title}
                        </CardTitle>
                        <div className={`p-2 rounded-md ${stat.bgColor}`}>
                            <stat.icon className={`h-4 w-4 ${stat.color}`} />
                        </div>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{stat.value}</div>
                        <p className="text-xs text-muted-foreground">{stat.description}</p>
                    </CardContent>
                </Card>
            ))}
        </div>
    );
}
