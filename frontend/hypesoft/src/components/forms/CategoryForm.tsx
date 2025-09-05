import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { categorySchema, type CategoryFormData } from '@/lib/validation/schemas';
import type { CreateCategoryDto, UpdateCategoryDto, Category } from '@/types';

interface CategoryFormProps {
    initialData?: Category;
    onSubmit: (data: CreateCategoryDto | UpdateCategoryDto) => Promise<void>;
    onCancel: () => void;
    isLoading?: boolean;
}

export function CategoryForm({ initialData, onSubmit, onCancel, isLoading = false }: CategoryFormProps) {
    const {
        register,
        handleSubmit,
        watch,
        reset,
        formState: { errors, isSubmitting }
    } = useForm<CategoryFormData>({
        resolver: zodResolver(categorySchema),
        defaultValues: {
            name: initialData?.name || '',
            description: initialData?.description || '',
        }
    });

    // Reset form when initialData changes
    useEffect(() => {
        if (initialData) {
            reset({
                name: initialData.name,
                description: initialData.description || '',
            });
        }
    }, [initialData, reset]);

    const description = watch('description') || '';
    const isProcessing = isLoading || isSubmitting;

    const onFormSubmit = async (data: CategoryFormData) => {
        try {
            await onSubmit({
                name: data.name,
                description: data.description || undefined,
            });
        } catch (error) {
            // Error handling is done in the parent component
        }
    };

    return (
        <form onSubmit={handleSubmit(onFormSubmit)} className="space-y-4">
            <div className="space-y-2">
                <Label htmlFor="name">
                    Category Name <span className="text-red-500">*</span>
                </Label>
                <Input
                    id="name"
                    type="text"
                    {...register('name')}
                    placeholder="Enter category name"
                    disabled={isProcessing}
                    className={errors.name ? 'border-red-500' : ''}
                />
                {errors.name && (
                    <p className="text-sm text-red-500">{errors.name.message}</p>
                )}
            </div>

            <div className="space-y-2">
                <Label htmlFor="description">Description</Label>
                <Textarea
                    id="description"
                    {...register('description')}
                    placeholder="Enter category description (optional)"
                    disabled={isProcessing}
                    rows={4}
                    className={errors.description ? 'border-red-500' : ''}
                />
                {errors.description && (
                    <p className="text-sm text-red-500">{errors.description.message}</p>
                )}
                <p className="text-sm text-muted-foreground">
                    {description.length}/500 characters
                </p>
            </div>

            <div className="flex gap-4 pt-4">
                <Button type="submit" disabled={isProcessing}>
                    {isProcessing ? 'Saving...' : initialData ? 'Update Category' : 'Create Category'}
                </Button>
                <Button type="button" variant="outline" onClick={onCancel} disabled={isProcessing}>
                    Cancel
                </Button>
            </div>
        </form>
    );
}
