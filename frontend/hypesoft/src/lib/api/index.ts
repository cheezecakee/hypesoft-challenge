import axios, { AxiosInstance, AxiosError } from 'axios';

// Create axios instance with base configuration
const apiClient: AxiosInstance = axios.create({
    baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor for adding auth token
apiClient.interceptors.request.use(
    (config) => {
        // Get token from localStorage (we'll implement auth later)
        const token = typeof window !== 'undefined' ? localStorage.getItem('accessToken') : null;

        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Response interceptor for error handling
apiClient.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
        // Handle common errors
        const status = error.response?.status;

        if (status === 401) {
            // Token expired or invalid
            if (typeof window !== 'undefined') {
                localStorage.removeItem('accessToken');
                localStorage.removeItem('refreshToken');
                // Redirect to login (we'll implement this later)
                window.location.href = '/login';
            }
        }

        if (status === 403) {
            // Forbidden - insufficient permissions
            console.error('Access denied: Insufficient permissions');
        }

        if (status && status >= 500) {
            // Server error
            console.error('Server error:', error.response?.data);
        }

        return Promise.reject(error);
    }
);

export default apiClient;
