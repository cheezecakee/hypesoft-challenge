import { Label } from '@/components/ui/label';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { SUPPORTED_CURRENCIES } from '@/lib/validation/schemas';

interface CurrencySelectorProps {
    value: string;
    onValueChange: (value: string) => void;
    error?: string;
    label?: string;
    placeholder?: string;
    required?: boolean;
}

export function CurrencySelector({
    value,
    onValueChange,
    error,
    label = 'Currency',
    placeholder = 'Select currency',
    required = false,
}: CurrencySelectorProps) {
    return (
        <div className="space-y-2">
            <Label htmlFor="currency">
                {label}
                {required && <span className="text-red-500 ml-1">*</span>}
            </Label>
            <Select value={value} onValueChange={onValueChange}>
                <SelectTrigger className={error ? 'border-red-500' : ''}>
                    <SelectValue placeholder={placeholder} />
                </SelectTrigger>
                <SelectContent>
                    {SUPPORTED_CURRENCIES.map((currency) => (
                        <SelectItem key={currency.code} value={currency.code}>
                            <div className="flex items-center gap-2">
                                <span className="font-medium">{currency.symbol}</span>
                                <span>{currency.name}</span>
                                <span className="text-muted-foreground">({currency.code})</span>
                            </div>
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>
            {error && (
                <p className="text-sm text-red-500">{error}</p>
            )}
        </div>
    );
}
