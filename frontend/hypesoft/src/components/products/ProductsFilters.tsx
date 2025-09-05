import { useState } from 'react';
import { Search, X } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { Card, CardContent } from '@/components/ui/card';
import { useCategories } from '@/hooks/useCategories';
import type { ProductsQueryParams } from '@/types';

interface ProductsFiltersProps {
    filters: ProductsQueryParams;
    onFiltersChange: (filters: Partial<ProductsQueryParams>) => void;
}

export function ProductsFilters({ filters, onFiltersChange }: ProductsFiltersProps) {
    const [searchInput, setSearchInput] = useState(filters.search || '');
    const { data: categories } = useCategories();

    const handleSearchSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onFiltersChange({ search: searchInput });
    };

    const handleClearFilters = () => {
        setSearchInput('');
        onFiltersChange({
            search: '',
            categoryId: '',
            sortBy: 'name',
            sortOrder: 'asc',
            pageNumber: 1,
        });
    };

    const hasActiveFilters = filters.search || filters.categoryId ||
        filters.sortBy !== 'name' || filters.sortOrder !== 'asc';

    return (
        <Card>
            <CardContent className="p-4">
                <div className="flex flex-col gap-4 md:flex-row md:items-center">
                    {/* Search */}
                    <form onSubmit={handleSearchSubmit} className="flex-1 max-w-sm">
                        <div className="relative">
                            <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
                            <Input
                                placeholder="Search products..."
                                value={searchInput}
                                onChange={(e) => setSearchInput(e.target.value)}
                                className="pl-10"
                            />
                        </div>
                    </form>

                    {/* Category Filter */}
                    <Select
                        value={filters.categoryId || ''}
                        onValueChange={(value) => onFiltersChange({ categoryId: value || '' })}
                    >
                        <SelectTrigger className="w-[180px]">
                            <SelectValue placeholder="All categories" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="">All categories</SelectItem>
                            {categories?.map((category) => (
                                <SelectItem key={category.id} value={category.id}>
                                    {category.name}
                                </SelectItem>
                            ))}
                        </SelectContent>
                    </Select>

                    {/* Sort */}
                    <Select
                        value={`${filters.sortBy}-${filters.sortOrder}`}
                        onValueChange={(value) => {
                            const [sortBy, sortOrder] = value.split('-');
                            onFiltersChange({
                                sortBy: sortBy as 'name' | 'price' | 'stock',
                                sortOrder: sortOrder as 'asc' | 'desc'
                            });
                        }}
                    >
                        <SelectTrigger className="w-[160px]">
                            <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="name-asc">Name A-Z</SelectItem>
                            <SelectItem value="name-desc">Name Z-A</SelectItem>
                            <SelectItem value="price-asc">Price Low-High</SelectItem>
                            <SelectItem value="price-desc">Price High-Low</SelectItem>
                            <SelectItem value="stock-asc">Stock Low-High</SelectItem>
                            <SelectItem value="stock-desc">Stock High-Low</SelectItem>
                        </SelectContent>
                    </Select>

                    {/* Clear Filters */}
                    {hasActiveFilters && (
                        <Button
                            variant="outline"
                            size="sm"
                            onClick={handleClearFilters}
                            className="shrink-0"
                        >
                            <X className="mr-2 h-4 w-4" />
                            Clear
                        </Button>
                    )}
                </div>
            </CardContent>
        </Card>
    );
}
