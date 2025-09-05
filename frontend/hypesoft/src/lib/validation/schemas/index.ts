import { z } from 'zod';

// Category schemas
export const categorySchema = z.object({
    name: z
        .string()
        .min(2, 'Category name must be at least 2 characters')
        .max(100, 'Category name must be less than 100 characters')
        .trim(),
    description: z
        .string()
        .max(500, 'Description must be less than 500 characters')
        .trim()
        .optional()
        .or(z.literal('')),
});

export const updateCategorySchema = categorySchema.partial();

// Product schemas
export const productSchema = z.object({
    name: z
        .string()
        .min(2, 'Product name must be at least 2 characters')
        .max(200, 'Product name must be less than 200 characters')
        .trim(),
    description: z
        .string()
        .max(1000, 'Description must be less than 1000 characters')
        .trim()
        .optional()
        .or(z.literal('')),
    price: z
        .number()
        .min(0.01, 'Price must be greater than 0')
        .max(999999.99, 'Price is too high')
        .multipleOf(0.01, 'Price must have at most 2 decimal places'),
    categoryId: z
        .string()
        .min(1, 'Please select a category'),
    stockQuantity: z
        .number()
        .int('Stock quantity must be a whole number')
        .min(0, 'Stock quantity cannot be negative')
        .max(999999, 'Stock quantity is too high'),
});

export const updateProductSchema = productSchema.partial();

// Stock update schema
export const stockUpdateSchema = z.object({
    stockQuantity: z
        .number()
        .int('Stock quantity must be a whole number')
        .min(0, 'Stock quantity cannot be negative')
        .max(999999, 'Stock quantity is too high'),
});

// Search/filter schemas
export const productSearchSchema = z.object({
    search: z.string().optional(),
    categoryId: z.string().optional(),
    minPrice: z.number().min(0).optional(),
    maxPrice: z.number().min(0).optional(),
    inStock: z.boolean().optional(),
    page: z.number().int().min(1).default(1),
    limit: z.number().int().min(1).max(100).default(10),
});

// Export types
export type CategoryFormData = z.infer<typeof categorySchema>;
export type UpdateCategoryFormData = z.infer<typeof updateCategorySchema>;
export type ProductFormData = z.infer<typeof productSchema>;
export type UpdateProductFormData = z.infer<typeof updateProductSchema>;
export type StockUpdateFormData = z.infer<typeof stockUpdateSchema>;
export type ProductSearchData = z.infer<typeof productSearchSchema>;
