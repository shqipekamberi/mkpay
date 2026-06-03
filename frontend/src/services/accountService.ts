import apiClient from './apiClient';

export interface Account {
    id: string;
    accountNumber: string;
    balance: number;
    currency: string;
    userId: string;
}

export const accountService = {
    getMyAccount: async (): Promise<Account> => {
        const response = await apiClient.get('/account/me');
        return response.data.data || response.data;
    }
};