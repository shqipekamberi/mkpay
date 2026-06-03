import apiClient from './apiClient';

export interface PaymentRequest {
    id: string;
    requesterId: string;
    requesterEmail: string;
    requesterFirstName: string;
    requesterLastName: string;
    amount: number;
    currency: string;
    description?: string;
    status: 'Pending' | 'Accepted' | 'Declined' | 'Cancelled';
    createdAt: string;
}

export interface CreatePaymentRequestDto {
    requesteeEmail: string;
    amount: number;
    description: string;
}

export const paymentRequestService = {
    getReceivedRequests: async (): Promise<PaymentRequest[]> => {
        const response = await apiClient.get('/paymentrequest/received');
        return response.data.data || response.data || [];
    },

    acceptRequest: async (id: string): Promise<void> => {
        await apiClient.post(`/paymentrequest/${id}/accept`);
    },

    declineRequest: async (id: string): Promise<void> => {
        await apiClient.post(`/paymentrequest/${id}/reject`);
    },

    createPaymentRequest: async (dto: CreatePaymentRequestDto): Promise<PaymentRequest> => {
        const response = await apiClient.post('/paymentrequest', dto);
        return response.data.data || response.data;
    },
};
