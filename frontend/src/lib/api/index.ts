import axios, { AxiosInstance, AxiosError } from 'axios';

const apiClient: AxiosInstance = axios.create({
    baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5113/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

apiClient.interceptors.request.use(
    (config) => {
        console.log('API Client - making request to:', config.url);
        return config;
    },
    (error) => {
        console.error('API Client request error:', error);
        return Promise.reject(error);
    }
);

apiClient.interceptors.response.use(
    (response) => {
        console.log('API Client response:', response.status, response.config.url);
        return response;
    },
    (error: AxiosError) => {
        const status = error.response?.status;
        console.error('API Client response error:', status, error.config?.url, error.message);

        return Promise.reject(error);
    }
);

export default apiClient;
