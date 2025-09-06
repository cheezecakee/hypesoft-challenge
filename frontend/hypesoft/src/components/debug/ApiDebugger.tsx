'use client';

import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useAuth } from '@/hooks/useAuth';
import { createProductsApi, createDashboardApi, createCategoriesApi } from '@/services';

export function ApiDebugger() {
    const [responses, setResponses] = useState<Record<string, any>>({});
    const [loading, setLoading] = useState<Record<string, boolean>>({});
    const { makeRequest } = useAuth();

    const testEndpoint = async (name: string, apiCall: () => Promise<any>) => {
        setLoading(prev => ({ ...prev, [name]: true }));
        try {
            const result = await apiCall();
            console.log(`${name} response:`, result);
            setResponses(prev => ({ ...prev, [name]: result }));
        } catch (error) {
            console.error(`${name} error:`, error);
            const errorMessage = error instanceof Error ? error.message : 'Unknown error';
            setResponses(prev => ({ ...prev, [name]: { error: errorMessage } }));
        }
        setLoading(prev => ({ ...prev, [name]: false }));
    };

    if (!makeRequest) {
        return <div>Not authenticated</div>;
    }

    const productsApi = createProductsApi(makeRequest);
    const dashboardApi = createDashboardApi(makeRequest);
    const categoriesApi = createCategoriesApi(makeRequest);

    return (
        <div className="grid gap-4">
            <Card>
                <CardHeader>
                    <CardTitle>API Response Debugger</CardTitle>
                    <p className="text-sm text-muted-foreground">
                        Test API endpoints to see actual response structure
                    </p>
                </CardHeader>
                <CardContent className="space-y-4">
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-2">
                        <Button
                            onClick={() => testEndpoint('products', () => productsApi.getProducts())}
                            disabled={loading.products}
                        >
                            {loading.products ? 'Loading...' : 'Test Products'}
                        </Button>

                        <Button
                            onClick={() => testEndpoint('categories', () => categoriesApi.getCategories())}
                            disabled={loading.categories}
                        >
                            {loading.categories ? 'Loading...' : 'Test Categories'}
                        </Button>

                        <Button
                            onClick={() => testEndpoint('lowStock', () => productsApi.getLowStockProducts())}
                            disabled={loading.lowStock}
                        >
                            {loading.lowStock ? 'Loading...' : 'Test Low Stock'}
                        </Button>

                        <Button
                            onClick={() => testEndpoint('dashboard', () => dashboardApi.getStats())}
                            disabled={loading.dashboard}
                        >
                            {loading.dashboard ? 'Loading...' : 'Test Dashboard'}
                        </Button>

                        <Button
                            onClick={() => testEndpoint('categoryStats', () => dashboardApi.getProductsByCategory())}
                            disabled={loading.categoryStats}
                        >
                            {loading.categoryStats ? 'Loading...' : 'Test Category Stats'}
                        </Button>
                    </div>

                    <div className="space-y-4 max-h-96 overflow-y-auto">
                        {Object.entries(responses).map(([key, response]) => (
                            <Card key={key}>
                                <CardHeader>
                                    <CardTitle className="text-sm">{key}</CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <pre className="text-xs bg-gray-100 p-2 rounded overflow-x-auto">
                                        {JSON.stringify(response, null, 2)}
                                    </pre>
                                </CardContent>
                            </Card>
                        ))}
                    </div>
                </CardContent>
            </Card>
        </div>
    );
}
