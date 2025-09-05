'use client';

import { AlertTriangle, Package } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import { useLowStockProducts } from '@/hooks/useProducts';
import Link from 'next/link';

export function LowStockTable() {
    const { data: lowStockProducts, isLoading, error } = useLowStockProducts();

    if (error) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                        <AlertTriangle className="h-5 w-5 text-orange-600" />
                        Low Stock Alert
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="text-center p-8 text-red-600">
                        Failed to load low stock products
                    </div>
                </CardContent>
            </Card>
        );
    }

    if (isLoading) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                        <AlertTriangle className="h-5 w-5 text-orange-600" />
                        Low Stock Alert
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="space-y-3">
                        {Array.from({ length: 3 }).map((_, i) => (
                            <div key={i} className="flex items-center justify-between p-3 border rounded-lg">
                                <div className="space-y-2">
                                    <Skeleton className="h-4 w-[120px]" />
                                    <Skeleton className="h-3 w-[80px]" />
                                </div>
                                <Skeleton className="h-6 w-[60px] rounded-full" />
                            </div>
                        ))}
                    </div>
                </CardContent>
            </Card>
        );
    }

    if (!lowStockProducts || lowStockProducts.length === 0) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                        <Package className="h-5 w-5 text-green-600" />
                        Stock Status
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="text-center p-8">
                        <Package className="h-12 w-12 text-green-600 mx-auto mb-4" />
                        <p className="text-lg font-medium text-green-600">All Good!</p>
                        <p className="text-sm text-muted-foreground">
                            No products with low stock levels
                        </p>
                    </div>
                </CardContent>
            </Card>
        );
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle className="flex items-center gap-2">
                    <AlertTriangle className="h-5 w-5 text-orange-600" />
                    Low Stock Alert
                </CardTitle>
                <p className="text-sm text-muted-foreground">
                    Products with less than 10 units in stock
                </p>
            </CardHeader>
            <CardContent>
                <div className="space-y-3 max-h-[280px] overflow-y-auto">
                    {lowStockProducts.map((product) => (
                        <div
                            key={product.id}
                            className="flex items-center justify-between p-3 border rounded-lg hover:bg-muted/50 transition-colors"
                        >
                            <div className="flex-1 min-w-0">
                                <div className="flex items-center gap-2 mb-1">
                                    <h4 className="font-medium truncate">{product.name}</h4>
                                    <Badge
                                        variant={product.stockQuantity === 0 ? "destructive" : "secondary"}
                                        className="shrink-0"
                                    >
                                        {product.stockQuantity} units
                                    </Badge>
                                </div>
                                <p className="text-sm text-muted-foreground truncate">
                                    ${product.price.toFixed(2)} • {product.category?.name || 'No category'}
                                </p>
                            </div>
                            <Button asChild size="sm" variant="outline">
                                <Link href={`/products/${product.id}/edit`}>
                                    Update Stock
                                </Link>
                            </Button>
                        </div>
                    ))}
                </div>

                {lowStockProducts.length > 5 && (
                    <div className="mt-4 pt-4 border-t text-center">
                        <Button asChild variant="outline">
                            <Link href="/products?filter=low-stock">
                                View All Low Stock Products
                            </Link>
                        </Button>
                    </div>
                )}
            </CardContent>
        </Card>
    );
}
