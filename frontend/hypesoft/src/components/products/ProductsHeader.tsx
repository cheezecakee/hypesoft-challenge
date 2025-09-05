'use client';

import { Plus } from 'lucide-react';
import { Button } from '@/components/ui/button';
import Link from 'next/link';

export function ProductsHeader() {
    return (
        <div className="flex items-center justify-between">
            <div>
                <h2 className="text-3xl font-bold tracking-tight">Products</h2>
                <p className="text-muted-foreground">
                    Manage your product inventory and details
                </p>
            </div>
            <Button asChild>
                <Link href="/products/create">
                    <Plus className="mr-2 h-4 w-4" />
                    Add Product
                </Link>
            </Button>
        </div>
    );
}
