'use client';

import { ReactKeycloakProvider } from '@react-keycloak/web';
import { useState, useEffect } from 'react';
import Keycloak from 'keycloak-js';

interface KeycloakWrapperProps {
    children: React.ReactNode;
}

export function KeycloakWrapper({ children }: KeycloakWrapperProps) {
    const [keycloak, setKeycloak] = useState<Keycloak | null>(null);

    useEffect(() => {
        const kc = new Keycloak({
            url: process.env.NEXT_PUBLIC_KEYCLOAK_URL || 'http://localhost:8080',
            realm: process.env.NEXT_PUBLIC_KEYCLOAK_REALM || 'hypesoft',
            clientId: process.env.NEXT_PUBLIC_KEYCLOAK_CLIENT_ID || 'hypesoft-frontend',
        });
        setKeycloak(kc);
    }, []);

    if (!keycloak) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
            </div>
        );
    }

    const initOptions = {
        onLoad: 'check-sso' as const,
        checkLoginIframe: false,
        pkceMethod: 'S256' as const,
    };

    return (
        <ReactKeycloakProvider
            authClient={keycloak}
            initOptions={initOptions}
        >
            {children}
        </ReactKeycloakProvider>
    );
}
