import apiClient from './apiClient';

export interface Transaction {
    id: string;
    amount: number;
    currency: string;
    description: string;
    type: string;
    status: string;
    createdAt: string;
}

export interface SendMoneyRequest {
    receiverEmail: string;
    amount: number;
    description?: string;
}

export const transactionService = {
    getMyTransactions: async (): Promise<Transaction[]> => {
        try {
            console.log('Fetching recent transactions...');

            const response = await apiClient.get('/Transaction/recent', {
                params: {
                    count: 10
                }
            });

            console.log('Response:', response.data);

            // Return the array directly or handle wrapped response
            return response.data.data || response.data || [];

        } catch (error: any) {
            console.error('Failed to fetch transactions:', error);
            return [];
        }
    },

    sendMoney: async (request: SendMoneyRequest): Promise<any> => {
        try {
            console.log('Sending money:', request);
            const response = await apiClient.post('/transaction/send', request);
            console.log('Send money response:', response.data);
            return response.data;
        } catch (error: any) {
            console.error('Send money failed:', error);
            console.error('Error details:', error.response?.data);
            throw error;
        }
    }
};