'use client';

import { useAuth } from '@/hooks/useAuth';
import { Skeleton } from '@/components/ui/skeleton';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

interface ProtectedRouteProps {
    children: React.ReactNode;
    requiredRole?: string;
    requiredRoles?: string[];
    fallback?: React.ReactNode;
    redirectToLogin?: boolean;
}

export function ProtectedRoute({
    children,
    requiredRole,
    requiredRoles = [],
    fallback,
    redirectToLogin = true
}: ProtectedRouteProps) {
    const { isAuthenticated, isLoading, hasAnyRole } = useAuth();
    const router = useRouter();

    useEffect(() => {
        if (!isLoading && !isAuthenticated && redirectToLogin) {
            router.push('/login');
        }
    }, [isAuthenticated, isLoading, redirectToLogin, router]);

    if (isLoading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-center space-y-4">
                    <Skeleton className="h-8 w-48 mx-auto" />
                    <Skeleton className="h-4 w-32 mx-auto" />
                    <p className="text-muted-foreground">Loading...</p>
                </div>
            </div>
        );
    }

    if (!isAuthenticated) {
        if (redirectToLogin) {
            // Show loading while redirecting
            return (
                <div className="min-h-screen flex items-center justify-center">
                    <div className="text-center space-y-4">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary mx-auto"></div>
                        <p className="text-muted-foreground">Redirecting to login...</p>
                    </div>
                </div>
            );
        }

        return fallback || (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-center space-y-4">
                    <h1 className="text-2xl font-bold">Access Denied</h1>
                    <p className="text-muted-foreground">
                        You need to be logged in to access this page.
                    </p>
                </div>
            </div>
        );
    }

    const allRequiredRoles = requiredRole ? [requiredRole, ...requiredRoles] : requiredRoles;

    if (allRequiredRoles.length > 0 && !hasAnyRole(allRequiredRoles)) {
        return fallback || (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-center space-y-4">
                    <h1 className="text-2xl font-bold">Insufficient Permissions</h1>
                    <p className="text-muted-foreground">
                        You don't have the required permissions to access this page.
                    </p>
                </div>
            </div>
        );
    }

    return <>{children}</>;
}
