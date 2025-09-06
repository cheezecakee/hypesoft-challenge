'use client';

import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from 'recharts';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useProductsByCategory } from '@/hooks/useDashboard';

const COLORS = [
    '#3b82f6', // blue
    '#ef4444', // red  
    '#10b981', // green
    '#f59e0b', // yellow
    '#8b5cf6', // purple
    '#06b6d4', // cyan
    '#f97316', // orange
    '#84cc16', // lime
];

export function ProductsByCategoryChart() {
    const { data: chartData, isLoading, error } = useProductsByCategory();

    if (error) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle>Products by Category</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="text-center p-8 text-red-600">
                        Failed to load chart data
                    </div>
                </CardContent>
            </Card>
        );
    }

    if (isLoading) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle>Products by Category</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="h-[300px] flex items-center justify-center">
                        <div className="space-y-3">
                            <Skeleton className="h-4 w-[200px]" />
                            <Skeleton className="h-32 w-32 rounded-full mx-auto" />
                            <Skeleton className="h-4 w-[150px]" />
                        </div>
                    </div>
                </CardContent>
            </Card>
        );
    }

    if (!chartData || chartData.length === 0) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle>Products by Category</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="h-[300px] flex items-center justify-center text-muted-foreground">
                        No data available
                    </div>
                </CardContent>
            </Card>
        );
    }

    // Transform data for recharts - use productCount instead of count
    const pieData = chartData.map((item) => ({
        name: item.categoryName,
        value: item.productCount,
    }));

    const CustomTooltip = ({ active, payload }: any) => {
        if (active && payload && payload.length) {
            const data = payload[0];
            const totalProducts = chartData.reduce((sum, item) => sum + item.productCount, 0);
            return (
                <div className="bg-white p-3 border rounded-lg shadow-lg">
                    <p className="font-medium">{data.name}</p>
                    <p className="text-sm text-muted-foreground">
                        {data.value} products ({((data.value / totalProducts) * 100).toFixed(1)}%)
                    </p>
                </div>
            );
        }
        return null;
    };

    return (
        <Card>
            <CardHeader>
                <CardTitle>Products by Category</CardTitle>
                <p className="text-sm text-muted-foreground">
                    Distribution of products across categories
                </p>
            </CardHeader>
            <CardContent>
                <div className="h-[300px]">
                    <ResponsiveContainer width="100%" height="100%">
                        <PieChart>
                            <Pie
                                data={pieData}
                                cx="50%"
                                cy="50%"
                                outerRadius={80}
                                innerRadius={60}
                                dataKey="value"
                                label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                            >
                                {pieData.map((_entry, index) => (
                                    <Cell
                                        key={`cell-${index}`}
                                        fill={COLORS[index % COLORS.length]}
                                    />
                                ))}
                            </Pie>
                            <Tooltip content={<CustomTooltip />} />
                        </PieChart>
                    </ResponsiveContainer>
                </div>

                {/* Legend */}
                <div className="grid grid-cols-2 gap-2 mt-4">
                    {pieData.map((entry, index) => (
                        <div key={entry.name} className="flex items-center gap-2">
                            <div
                                className="w-3 h-3 rounded-full"
                                style={{ backgroundColor: COLORS[index % COLORS.length] }}
                            />
                            <span className="text-sm text-muted-foreground truncate">
                                {entry.name}
                            </span>
                        </div>
                    ))}
                </div>
            </CardContent>
        </Card>
    );
}
