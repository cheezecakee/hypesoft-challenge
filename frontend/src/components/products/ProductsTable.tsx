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
import { useProducts, useDeleteProduct, useUpdateProductStock } from '@/hooks/useProducts';
import { StockUpdateDialog } from '@/components/products/StockUpdateDialog';
import { ProductsPagination } from './ProductsPagination';
import type { ProductsQueryParams } from '@/types';
import { formatPrice } from '@/lib/validation/schemas';

interface ProductsTableProps {
    filters: ProductsQueryParams;
    onFiltersChange: (filters: Partial<ProductsQueryParams>) => void;
}

export function ProductsTable({ filters, onFiltersChange }: ProductsTableProps) {
    const [deleteProductId, setDeleteProductId] = useState<string | null>(null);
    const [stockUpdateProduct, setStockUpdateProduct] = useState<{
        id: string;
        name: string;
        currentStock: number;
    } | null>(null);

    const { data: productsData, isLoading, error } = useProducts(filters);
    const deleteProductMutation = useDeleteProduct();
    const updateStockMutation = useUpdateProductStock();

    const handleDelete = async () => {
        if (deleteProductId) {
            await deleteProductMutation.mutateAsync(deleteProductId);
            setDeleteProductId(null);
        }
    };

    const handleStockUpdate = async (newStock: number) => {
        if (stockUpdateProduct) {
            await updateStockMutation.mutateAsync({
                id: stockUpdateProduct.id,
                stockQuantity: newStock
            });
            setStockUpdateProduct(null);
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
                        Failed to load products: {error?.message || 'Unknown error'}
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

    const products = productsData?.products || [];
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
                                            {product.categoryName || 'No category'}
                                        </Badge>
                                    </TableCell>
                                    <TableCell className="font-medium">
                                        {formatPrice(product.price, product.currency)}
                                    </TableCell>
                                    <TableCell>
                                        <div className="flex items-center gap-2">
                                            <button
                                                onClick={() => setStockUpdateProduct({
                                                    id: product.id,
                                                    name: product.name,
                                                    currentStock: product.stockQuantity
                                                })}
                                                onKeyDown={(e) => {
                                                    if (e.key === 'Enter' || e.key === ' ') {
                                                        e.preventDefault();
                                                        setStockUpdateProduct({
                                                            id: product.id,
                                                            name: product.name,
                                                            currentStock: product.stockQuantity
                                                        });
                                                    }
                                                }}
                                                className="hover:underline focus:outline-none focus:underline cursor-pointer rounded px-1 py-0.5 hover:bg-muted transition-colors"
                                                title="Click to update stock"
                                                type="button"
                                            >
                                                <span className={product.isLowStock ? 'text-orange-600 font-medium' : ''}>
                                                    {product.stockQuantity}
                                                </span>
                                            </button>
                                            {product.isLowStock && (
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
                currentPage={filters.page || 1}
                pageSize={filters.pageSize || 10}
                totalCount={totalCount}
                onPageChange={(page) => onFiltersChange({ page })}
                onPageSizeChange={(pageSize) => onFiltersChange({ pageSize, page: 1 })}
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

            {/* Stock Update Dialog */}
            <StockUpdateDialog
                isOpen={!!stockUpdateProduct}
                onClose={() => setStockUpdateProduct(null)}
                onConfirm={handleStockUpdate}
                productName={stockUpdateProduct?.name || ''}
                currentStock={stockUpdateProduct?.currentStock || 0}
                isLoading={updateStockMutation.isPending}
            />
        </>
    );
}
