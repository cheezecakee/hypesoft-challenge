import { Plus } from 'lucide-react';
import { Button } from '@/components/ui/button';
import Link from 'next/link';

interface CategoriesHeaderProps {
    onRefresh: () => void;
}

export function CategoriesHeader({ onRefresh }: CategoriesHeaderProps) {
    return (
        <div className="flex items-center justify-between">
            <div>
                <h2 className="text-3xl font-bold tracking-tight">Categories</h2>
                <p className="text-muted-foreground">
                    Manage product categories and organization
                </p>
            </div>
            <Button asChild>
                <Link href="/categories/create">
                    <Plus className="mr-2 h-4 w-4" />
                    Add Category
                </Link>
            </Button>
        </div>
    );
}
