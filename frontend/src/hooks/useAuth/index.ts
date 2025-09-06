import { useKeycloak } from '@react-keycloak/web';
import { useCallback, useMemo } from 'react';
import { AxiosRequestConfig } from 'axios';
import apiClient from '@/lib/api';

interface User {
    id: string;
    username: string;
    email?: string;
    firstName?: string;
    lastName?: string;
    roles: string[];
}

interface AuthState {
    isAuthenticated: boolean;
    isLoading: boolean;
    user: User | null;
    token: string | null;
    login: () => void;
    logout: () => void;
    hasRole: (role: string) => boolean;
    hasAnyRole: (roles: string[]) => boolean;
    makeRequest: <T = any>(config: AxiosRequestConfig) => Promise<T>;
}

export function useAuth(): AuthState {
    const { keycloak, initialized } = useKeycloak();

    // Extract user info from token
    const user = useMemo((): User | null => {
        if (!keycloak?.tokenParsed) return null;

        const token = keycloak.tokenParsed as any;
        return {
            id: token.sub,
            username: token.preferred_username || token.sub,
            email: token.email,
            firstName: token.given_name,
            lastName: token.family_name,
            roles: token.realm_access?.roles || [],
        };
    }, [keycloak?.tokenParsed]);

    // Authenticated request function  
    const makeRequest = useCallback(async <T = any>(config: AxiosRequestConfig): Promise<T> => {
        if (!keycloak?.authenticated || !keycloak?.token) {
            throw new Error('User not authenticated');
        }

        // Ensure token is fresh
        await keycloak.updateToken(30);

        const response = await apiClient({
            ...config,
            headers: {
                ...config.headers,
                Authorization: `Bearer ${keycloak.token}`,
            },
        });

        return response.data;
    }, [keycloak]);

    // Login function
    const login = useCallback(() => {
        keycloak?.login();
    }, [keycloak]);

    // Logout function
    const logout = useCallback(() => {
        keycloak?.logout({
            redirectUri: window.location.origin,
        });
    }, [keycloak]);

    // Check if user has specific role
    const hasRole = useCallback((role: string): boolean => {
        return user?.roles.includes(role) || false;
    }, [user?.roles]);

    // Check if user has any of the specified roles
    const hasAnyRole = useCallback((roles: string[]): boolean => {
        return roles.some(role => hasRole(role));
    }, [hasRole]);

    return {
        isAuthenticated: !!keycloak?.authenticated,
        isLoading: !initialized,
        user,
        token: keycloak?.token || null,
        login,
        logout,
        hasRole,
        hasAnyRole,
        makeRequest,
    };
}
