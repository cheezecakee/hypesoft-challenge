import { useState } from 'react';
import { Package, Loader2 } from 'lucide-react';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';

interface StockUpdateDialogProps {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: (newStock: number) => Promise<void>;
    productName: string;
    currentStock: number;
    isLoading?: boolean;
}

export function StockUpdateDialog({
    isOpen,
    onClose,
    onConfirm,
    productName,
    currentStock,
    isLoading = false
}: StockUpdateDialogProps) {
    const [stockValue, setStockValue] = useState<string>(currentStock.toString());
    const [inputError, setInputError] = useState<string>('');

    const handleStockChange = (value: string) => {
        setStockValue(value);
        setInputError('');
    };

    const validateAndSubmit = async () => {
        const numericValue = parseInt(stockValue, 10);
        
        if (isNaN(numericValue) || numericValue < 0) {
            setInputError('Please enter a valid non-negative number');
            return;
        }

        if (numericValue > 999999) {
            setInputError('Stock quantity cannot exceed 999,999');
            return;
        }

        try {
            await onConfirm(numericValue);
            handleClose();
        } catch (error) {
            // Error handling is done by the parent component
            console.error('Stock update failed:', error);
        }
    };

    const handleClose = () => {
        setStockValue(currentStock.toString());
        setInputError('');
        onClose();
    };

    const handleKeyDown = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter' && !isLoading) {
            e.preventDefault();
            validateAndSubmit();
        }
    };

    const hasChanges = parseInt(stockValue, 10) !== currentStock;
    const isValidInput = !isNaN(parseInt(stockValue, 10)) && parseInt(stockValue, 10) >= 0;

    return (
        <Dialog open={isOpen} onOpenChange={handleClose}>
            <DialogContent className="sm:max-w-md">
                <DialogHeader>
                    <DialogTitle className="flex items-center gap-2">
                        <Package className="h-5 w-5" />
                        Update Stock
                    </DialogTitle>
                    <DialogDescription>
                        Update the stock quantity for{' '}
                        <span className="font-medium text-foreground">
                            {productName}
                        </span>
                    </DialogDescription>
                </DialogHeader>

                <div className="space-y-4 py-4">
                    <div className="space-y-2">
                        <Label htmlFor="stock-input">
                            Stock Quantity
                        </Label>
                        <div className="relative">
                            <Input
                                id="stock-input"
                                type="number"
                                min="0"
                                max="999999"
                                value={stockValue}
                                onChange={(e) => handleStockChange(e.target.value)}
                                onKeyDown={handleKeyDown}
                                placeholder="Enter stock quantity"
                                className={inputError ? 'border-red-500 focus-visible:ring-red-500' : ''}
                                disabled={isLoading}
                                autoFocus
                            />
                        </div>
                        {inputError && (
                            <p className="text-sm text-red-600">{inputError}</p>
                        )}
                    </div>

                    <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <span>Current stock:</span>
                        <span className="font-medium">{currentStock} units</span>
                    </div>
                </div>

                <DialogFooter className="flex-col sm:flex-row gap-2">
                    <Button 
                        variant="outline" 
                        onClick={handleClose}
                        disabled={isLoading}
                    >
                        Cancel
                    </Button>
                    <Button
                        onClick={validateAndSubmit}
                        disabled={!hasChanges || !isValidInput || isLoading}
                        className="min-w-[100px]"
                    >
                        {isLoading ? (
                            <>
                                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                Updating...
                            </>
                        ) : (
                            'Update Stock'
                        )}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}
