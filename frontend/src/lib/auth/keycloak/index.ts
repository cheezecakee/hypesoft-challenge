import Keycloak from 'keycloak-js';

let keycloak: Keycloak | undefined;

if (typeof window !== 'undefined') {
    keycloak = new Keycloak({
        url: process.env.NEXT_PUBLIC_KEYCLOAK_URL || 'http://localhost:8080',
        realm: process.env.NEXT_PUBLIC_KEYCLOAK_REALM || 'hypesoft',
        clientId: process.env.NEXT_PUBLIC_KEYCLOAK_CLIENT_ID || 'hypesoft-frontend',
    });
}

export default keycloak;
