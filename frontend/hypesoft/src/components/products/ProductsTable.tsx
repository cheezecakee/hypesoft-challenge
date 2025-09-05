import { useState } from 'react';
import { MoreHorizontal, Edit, Trash2, AlertTriangle } from 'lucide-react';
import Link from 'next/link';
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/components/ui/table';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { useProducts, useDeleteProduct } from '@/hooks/useProducts';
import { ProductsPagination } from './ProductsPagination';
import type { ProductsQueryParams } from '@/types';

interface ProductsTableProps {
    filters: ProductsQueryParams;
    onFiltersChange: (filters: Partial<ProductsQueryParams>) => void;
}

export function ProductsTable({ filters, onFiltersChange }: ProductsTableProps) {
    const [deleteProductId, setDeleteProductId] = useState<string | null>(null);

    const { data: productsData, isLoading, error } = useProducts(filters);
    const deleteProductMutation = useDeleteProduct();

    const handleDelete = async () => {
        if (deleteProductId) {
            await deleteProductMutation.mutateAsync(deleteProductId);
            setDeleteProductId(null);
        }
    };

    if (error) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle>Products</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="text-center p-8 text-red-600">
                        Failed to load products
                    </div>
                </CardContent>
            </Card>
        );
    }

    if (isLoading) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle>Products</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="space-y-3">
                        {Array.from({ length: 5 }).map((_, i) => (
                            <div key={i} className="flex items-center space-x-4 p-4">
                                <Skeleton className="h-4 w-[200px]" />
                                <Skeleton className="h-4 w-[100px]" />
                                <Skeleton className="h-4 w-[80px]" />
                                <Skeleton className="h-4 w-[60px]" />
                            </div>
                        ))}
                    </div>
                </CardContent>
            </Card>
        );
    }

    const products = productsData?.data || [];
    const totalCount = productsData?.totalCount || 0;

    if (products.length === 0) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle>Products</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="text-center p-8">
                        <p className="text-lg font-medium mb-2">No products found</p>
                        <p className="text-muted-foreground mb-4">
                            {filters.search || filters.categoryId
                                ? 'Try adjusting your search or filter criteria'
                                : 'Get started by creating your first product'
                            }
                        </p>
                        <Button asChild>
                            <Link href="/products/create">Create Product</Link>
                        </Button>
                    </div>
                </CardContent>
            </Card>
        );
    }

    return (
        <>
            <Card>
                <CardHeader>
                    <CardTitle>Products ({totalCount})</CardTitle>
                </CardHeader>
                <CardContent className="p-0">
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Product</TableHead>
                                <TableHead>Category</TableHead>
                                <TableHead>Price</TableHead>
                                <TableHead>Stock</TableHead>
                                <TableHead className="w-[100px]">Actions</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {products.map((product) => (
                                <TableRow key={product.id} className="hover:bg-muted/50">
                                    <TableCell>
                                        <div>
                                            <div className="font-medium">{product.name}</div>
                                            <div className="text-sm text-muted-foreground line-clamp-1">
                                                {product.description}
                                            </div>
                                        </div>
                                    </TableCell>
                                    <TableCell>
                                        <Badge variant="outline">
                                            {product.category?.name || 'No category'}
                                        </Badge>
                                    </TableCell>
                                    <TableCell className="font-medium">
                                        ${product.price.toFixed(2)}
                                    </TableCell>
                                    <TableCell>
                                        <div className="flex items-center gap-2">
                                            <span className={product.stockQuantity < 10 ? 'text-orange-600 font-medium' : ''}>
                                                {product.stockQuantity}
                                            </span>
                                            {product.stockQuantity < 10 && (
                                                <AlertTriangle className="h-4 w-4 text-orange-600" />
                                            )}
                                        </div>
                                    </TableCell>
                                    <TableCell>
                                        <DropdownMenu>
                                            <DropdownMenuTrigger asChild>
                                                <Button variant="ghost" className="h-8 w-8 p-0">
                                                    <MoreHorizontal className="h-4 w-4" />
                                                </Button>
                                            </DropdownMenuTrigger>
                                            <DropdownMenuContent align="end">
                                                <DropdownMenuItem asChild>
                                                    <Link href={`/products/${product.id}/edit`}>
                                                        <Edit className="mr-2 h-4 w-4" />
                                                        Edit
                                                    </Link>
                                                </DropdownMenuItem>
                                                <DropdownMenuItem
                                                    className="text-red-600"
                                                    onClick={() => setDeleteProductId(product.id)}
                                                >
                                                    <Trash2 className="mr-2 h-4 w-4" />
                                                    Delete
                                                </DropdownMenuItem>
                                            </DropdownMenuContent>
                                        </DropdownMenu>
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>

            <ProductsPagination
                currentPage={filters.pageNumber || 1}
                pageSize={filters.pageSize || 10}
                totalCount={totalCount}
                onPageChange={(page) => onFiltersChange({ pageNumber: page })}
                onPageSizeChange={(pageSize) => onFiltersChange({ pageSize, pageNumber: 1 })}
            />

            {/* Delete Confirmation Dialog */}
            <AlertDialog open={!!deleteProductId} onOpenChange={() => setDeleteProductId(null)}>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Delete Product</AlertDialogTitle>
                        <AlertDialogDescription>
                            Are you sure you want to delete this product? This action cannot be undone.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                        <AlertDialogAction
                            onClick={handleDelete}
                            className="bg-red-600 hover:bg-red-700"
                            disabled={deleteProductMutation.isPending}
                        >
                            {deleteProductMutation.isPending ? 'Deleting...' : 'Delete'}
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
}
