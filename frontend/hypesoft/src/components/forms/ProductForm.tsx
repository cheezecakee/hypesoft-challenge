import { useEffect } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { CurrencySelector } from '@/components/ui/currency-selector';
import { productSchema, type ProductFormData, DEFAULT_CURRENCY } from '@/lib/validation/schemas';
import type { CreateProductDto, UpdateProductDto, Product, Category } from '@/types';

interface ProductFormProps<T extends CreateProductDto | UpdateProductDto = CreateProductDto | UpdateProductDto> {
    initialData?: Product;
    categories: Category[];
    onSubmit: (data: T) => Promise<void>;
    onCancel: () => void;
    isLoading?: boolean;
}

export function ProductForm<T extends CreateProductDto | UpdateProductDto = CreateProductDto | UpdateProductDto>({
    initialData,
    categories,
    onSubmit,
    onCancel,
    isLoading = false,
}: ProductFormProps<T>) {
    const {
        register,
        handleSubmit,
        watch,
        reset,
        control,
        setValue,
        formState: { errors, isSubmitting },
    } = useForm<ProductFormData>({
        resolver: zodResolver(productSchema),
        defaultValues: {
            name: initialData?.name || '',
            description: initialData?.description || '',
            price: initialData?.price || 0,
            currency: initialData?.currency || DEFAULT_CURRENCY,
            categoryId: initialData?.categoryId || '',
            stockQuantity: initialData?.stockQuantity || 0,
        },
    });

    useEffect(() => {
        if (initialData) {
            reset({
                name: initialData.name,
                description: initialData.description || '',
                price: initialData.price,
                currency: initialData.currency,
                categoryId: initialData.categoryId,
                stockQuantity: initialData.stockQuantity,
            });
        }
    }, [initialData, reset]);

    const description = watch('description') || '';
    const currency = watch('currency');
    const isProcessing = isLoading || isSubmitting;

    const onFormSubmit = async (data: ProductFormData) => {
        try {
            await onSubmit({
                name: data.name,
                description: data.description || undefined,
                price: data.price,
                currency: data.currency,
                categoryId: data.categoryId,
                stockQuantity: data.stockQuantity,
            } as T);
        } catch (error) {
            // Error handling is done in parent
        }
    };

    return (
        <form onSubmit={handleSubmit(onFormSubmit)} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="space-y-2">
                    <Label htmlFor="name">Product Name <span className="text-red-500">*</span></Label>
                    <Input
                        id="name"
                        type="text"
                        {...register('name')}
                        placeholder="Enter product name"
                        disabled={isProcessing}
                        className={errors.name ? 'border-red-500' : ''}
                    />
                    {errors.name && <p className="text-sm text-red-500">{errors.name.message}</p>}
                </div>

                <div className="space-y-2">
                    <Label htmlFor="categoryId">Category <span className="text-red-500">*</span></Label>
                    <Controller
                        name="categoryId"
                        control={control}
                        render={({ field }) => (
                            <Select
                                value={field.value}
                                onValueChange={field.onChange}
                                disabled={isProcessing}
                            >
                                <SelectTrigger className={errors.categoryId ? 'border-red-500' : ''}>
                                    <SelectValue placeholder="Select a category" />
                                </SelectTrigger>
                                <SelectContent>
                                    {categories.map((category) => (
                                        <SelectItem key={category.id} value={category.id}>{category.name}</SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                        )}
                    />
                    {errors.categoryId && <p className="text-sm text-red-500">{errors.categoryId.message}</p>}
                </div>

                <div className="space-y-2">
                    <Label htmlFor="price">Price <span className="text-red-500">*</span></Label>
                    <Input
                        id="price"
                        type="number"
                        step="0.01"
                        min="0"
                        {...register('price', { valueAsNumber: true })}
                        placeholder="0.00"
                        disabled={isProcessing}
                        className={errors.price ? 'border-red-500' : ''}
                    />
                    {errors.price && <p className="text-sm text-red-500">{errors.price.message}</p>}
                </div>

                <CurrencySelector
                    value={currency}
                    onValueChange={(value) => setValue('currency', value)}
                    error={errors.currency?.message}
                    required
                />

                <div className="space-y-2">
                    <Label htmlFor="stockQuantity">Stock Quantity <span className="text-red-500">*</span></Label>
                    <Input
                        id="stockQuantity"
                        type="number"
                        min="0"
                        {...register('stockQuantity', { valueAsNumber: true })}
                        placeholder="0"
                        disabled={isProcessing}
                        className={errors.stockQuantity ? 'border-red-500' : ''}
                    />
                    {errors.stockQuantity && <p className="text-sm text-red-500">{errors.stockQuantity.message}</p>}
                </div>
            </div>

            <div className="space-y-2">
                <Label htmlFor="description">Description</Label>
                <Textarea
                    id="description"
                    {...register('description')}
                    placeholder="Enter product description (optional)"
                    disabled={isProcessing}
                    rows={4}
                    className={errors.description ? 'border-red-500' : ''}
                />
                {errors.description && <p className="text-sm text-red-500">{errors.description.message}</p>}
                <p className="text-sm text-muted-foreground">{description.length}/1000 characters</p>
            </div>

            <div className="flex gap-4 pt-4">
                <Button type="submit" disabled={isProcessing}>
                    {isProcessing ? 'Saving...' : initialData ? 'Update Product' : 'Create Product'}
                </Button>
                <Button type="button" variant="outline" onClick={onCancel} disabled={isProcessing}>
                    Cancel
                </Button>
            </div>
        </form>
    );
}
