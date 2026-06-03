namespace DefaultNamespace;

public class index_ts
{
    export interface User {
        id: string;
        email: string;
        firstName: string;
        lastName: string;
        phoneNumber: string;
        balance: number;
        createdAt: string;
    }

    export interface Transaction {
        id: string;
        amount: number;
        description: string;
        senderId: string;
        receiverId: string;
        sender: User;
        receiver: User;
        createdAt: string;
    }

    export interface PaymentRequest {
        id: string;
        amount: number;
        description: string;
        senderId: string;
        receiverId: string;
        status: 'pending' | 'approved' | 'rejected';
        createdAt: string;
    }
}