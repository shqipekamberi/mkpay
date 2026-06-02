import apiClient from './apiClient';

// Define interfaces to match backend
export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    password: string;
    confirmPassword: string;
}

export interface AuthResponse {
    token: string;
    userId: string;
    email: string;
    firstName: string;
    lastName: string;
    role: string;
    expiresAt: string;
}

// Helper to trigger re-render in components
export const authStateChanged = new Event('authStateChanged');

// REAL API service
export const authService = {
    login: async (credentials: LoginRequest): Promise<AuthResponse> => {
        console.log('Calling real API login with:', credentials);

        try {
            const response = await apiClient.post('/auth/login', credentials);

            console.log('Full response object:', response);
            console.log('Response data:', response.data);
            console.log('Response status:', response.status);

            // Handle both wrapped (ApiResponse) and unwrapped responses
            // Backend might return: { success: true, data: {...} } or just {...}
            let authData: AuthResponse;

            if (response.data.data) {
                // Wrapped in ApiResponse
                console.log('Response is wrapped in ApiResponse');
                authData = response.data.data;
            } else if (response.data.token) {
                // Direct AuthResponseDto
                console.log('Response is direct AuthResponseDto');
                authData = response.data;
            } else {
                console.error('Unexpected response structure:', response.data);
                throw new Error('Invalid response structure from server');
            }

            console.log('Extracted auth data:', authData);

            // Store token
            localStorage.setItem('token', authData.token);

            // Store user info in the format the app expects
            const userInfo = {
                id: authData.userId,
                email: authData.email,
                firstName: authData.firstName,
                lastName: authData.lastName,
                phoneNumber: '', // Not in auth response, will get from account endpoint
                role: authData.role,
                balance: 0 // Will get from account endpoint
            };

            console.log('Storing user info:', userInfo);
            localStorage.setItem('user', JSON.stringify(userInfo));

            // Notify components that auth state changed
            window.dispatchEvent(authStateChanged);

            console.log('Login successful!');
            return authData;

        } catch (error: any) {
            console.error('Login failed');
            console.error('Error object:', error);
            console.error('Error response:', error.response?.data);
            console.error('Error status:', error.response?.status);
            console.error('Error headers:', error.response?.headers);

            // Re-throw with better error message
            const errorMessage = error.response?.data?.message
                || error.response?.data?.errors?.[0]
                || error.message
                || 'Login failed. Please try again.';

            throw new Error(errorMessage);
        }
    },

    register: async (userData: RegisterRequest): Promise<AuthResponse> => {
        console.log('Calling real API register with:', {
            ...userData,
            password: '***HIDDEN***' 
        });

        try {
            const response = await apiClient.post('/auth/register', userData);

            console.log('Full response object:', response);
            console.log('Response data:', response.data);
            console.log('Response status:', response.status);

            // Handle both wrapped (ApiResponse) and unwrapped responses
            let authData: AuthResponse;

            if (response.data.data) {
                console.log('Response is wrapped in ApiResponse');
                authData = response.data.data;
            } else if (response.data.token) {
                console.log('Response is direct AuthResponseDto');
                authData = response.data;
            } else {
                console.error('Unexpected response structure:', response.data);
                throw new Error('Invalid response structure from server');
            }

            console.log('Extracted auth data:', authData);

            // Store token
            localStorage.setItem('token', authData.token);

            // Store user info
            const userInfo = {
                id: authData.userId,
                email: authData.email,
                firstName: authData.firstName,
                lastName: authData.lastName,
                phoneNumber: userData.phoneNumber, // From registration form
                role: authData.role,
                balance: 0
            };

            console.log('Storing user info:', userInfo);
            localStorage.setItem('user', JSON.stringify(userInfo));

            // Notify components that auth state changed
            window.dispatchEvent(authStateChanged);

            console.log('Registration successful!');
            return authData;

        } catch (error: any) {
            console.error('Registration failed');
            console.error('Error object:', error);
            console.error('Error response:', error.response?.data);
            console.error('Error status:', error.response?.status);
            console.error('Error headers:', error.response?.headers);

            // Re-throw with better error message
            const errorMessage = error.response?.data?.message
                || error.response?.data?.errors?.[0]
                || error.message
                || 'Registration failed. Please try again.';

            throw new Error(errorMessage);
        }
    },

    logout: (): void => {
        console.log('Logging out');
        localStorage.removeItem('token');
        localStorage.removeItem('user');

        // Notify components that auth state changed
        window.dispatchEvent(authStateChanged);
    },

    getCurrentUser: () => {
        const userStr = localStorage.getItem('user');
        return userStr ? JSON.parse(userStr) : null;
    },

    isAuthenticated: (): boolean => {
        const hasToken = !!localStorage.getItem('token');
        console.log('Auth check:', hasToken ? 'Authenticated' : 'Not authenticated');
        return hasToken;
    },
};