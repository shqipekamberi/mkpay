import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col, Card, Button, Modal, Form } from 'react-bootstrap';
import { authService } from '../services/authService';
import { useNavigate } from 'react-router-dom';
import { accountService } from '../services/accountService';
import { transactionService } from '../services/transactionService';
import { paymentRequestService } from '../services/paymentRequestService';

const DashboardPage: React.FC = () => {
    const navigate = useNavigate();
    const [user, setUser] = useState<any>(null);
    const [balance, setBalance] = useState<number>(0);
    const [loading, setLoading] = useState<boolean>(true);
    const [transactions, setTransactions] = useState<any[]>([]);

    // Send Money Modal State
    const [showSendModal, setShowSendModal] = useState(false);
    const [sendAmount, setSendAmount] = useState('');
    const [sendEmail, setSendEmail] = useState('');

    // Request Money Modal State
    const [showRequestModal, setShowRequestModal] = useState(false);
    const [requestAmount, setRequestAmount] = useState('');
    const [requestEmail, setRequestEmail] = useState('');

    const refreshData = useCallback(async () => {
        try {
            const account = await accountService.getMyAccount();
            setBalance(account.balance);

            const txns = await transactionService.getMyTransactions();
            setTransactions(txns.slice(0, 5));
        } catch (error) {
            console.error('Failed to refresh data:', error);
        }
    }, []);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const currentUser = authService.getCurrentUser();
                setUser(currentUser);
                await refreshData();
            } catch (error) {
                console.error('Failed to fetch data:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [refreshData]);

    const handleSendMoney = async () => {
        const amount = parseFloat(sendAmount);
        if (amount <= 0) {
            alert('Please enter a valid amount');
            return;
        }
        try {
            await transactionService.sendMoney({ receiverEmail: sendEmail, amount });
            setShowSendModal(false);
            setSendAmount('');
            setSendEmail('');
            alert(`Successfully sent ${amount.toFixed(2)} MKD to ${sendEmail}`);
            await refreshData();
        } catch (error: any) {
            alert(error.response?.data?.message || 'Failed to send money. Please try again.');
        }
    };

    const handleRequestMoney = async () => {
        const amount = parseFloat(requestAmount);
        if (amount <= 0) {
            alert('Please enter a valid amount');
            return;
        }
        try {
            await paymentRequestService.createPaymentRequest({ requesteeEmail: requestEmail, amount, description: '' });
            setShowRequestModal(false);
            setRequestAmount('');
            setRequestEmail('');
            alert(`Request sent for ${amount.toFixed(2)} MKD to ${requestEmail}`);
            await refreshData();
        } catch (error: any) {
            alert(error.response?.data?.message || 'Failed to send request. Please try again.');
        }
    };

    if (loading) {
        return (
            <Container className="text-center mt-5">
                <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div>
                <p className="mt-2">Loading your account...</p>
            </Container>
        );
    }

    return (
        <Container className="mt-4">
            {/* Header */}
            <Row className="justify-content-between align-items-center mb-4">
                <Col>
                    <h1>Welcome, {user?.firstName}!</h1>
                    <p className="text-muted">Manage your payments easily</p>
                </Col>
            </Row>

            {/* Balance Card - Venmo Style */}
            <Row className="mb-4">
                <Col md={12}>
                    <div className="balance-card text-center">
                        <h5 style={{ opacity: 0.9, fontWeight: 600 }}>Your Balance</h5>
                        <div className="display-4">
                            {balance.toFixed(2)} <span style={{ fontSize: '2rem' }}>MKD</span>
                        </div>
                        <p style={{ opacity: 0.8, marginTop: '1rem', marginBottom: 0, color: 'white' }}>
                            {user?.email}
                        </p>
                    </div>
                </Col>
            </Row>

            {/* Action Cards */}
            <Row className="mb-4">
                <Col md={6} className="mb-3">
                    <Card className="h-100">
                        <Card.Body className="text-center">
                            <Card.Title>Send Money</Card.Title>
                            <Card.Text>
                                Quickly send money to friends and family
                            </Card.Text>
                            <Button
                                variant="primary"
                                onClick={() => setShowSendModal(true)}
                            >
                                Send Money
                            </Button>
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={6} className="mb-3">
                    <Card className="h-100">
                        <Card.Body className="text-center">
                            <Card.Title>Request Money</Card.Title>
                            <Card.Text>
                                Request money from other users
                            </Card.Text>
                            <Button
                                variant="success"
                                onClick={() => setShowRequestModal(true)}
                            >
                                Request Money
                            </Button>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            {/* Send Money Modal */}
            <Modal show={showSendModal} onHide={() => setShowSendModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Send Money</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group className="mb-3">
                            <Form.Label>Recipient Email</Form.Label>
                            <Form.Control
                                type="email"
                                placeholder="Enter recipient email"
                                value={sendEmail}
                                onChange={(e) => setSendEmail(e.target.value)}
                            />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Amount (MKD)</Form.Label>
                            <Form.Control
                                type="number"
                                placeholder="Enter amount"
                                value={sendAmount}
                                onChange={(e) => setSendAmount(e.target.value)}
                            />
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowSendModal(false)}>
                        Cancel
                    </Button>
                    <Button variant="primary" onClick={handleSendMoney}>
                        Send {sendAmount || '0.00'} MKD
                    </Button>
                </Modal.Footer>
            </Modal>

            {/* Request Money Modal */}
            <Modal show={showRequestModal} onHide={() => setShowRequestModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Request Money</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group className="mb-3">
                            <Form.Label>From (Email)</Form.Label>
                            <Form.Control
                                type="email"
                                placeholder="Enter email"
                                value={requestEmail}
                                onChange={(e) => setRequestEmail(e.target.value)}
                            />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Amount (MKD)</Form.Label>
                            <Form.Control
                                type="number"
                                placeholder="Enter amount"
                                value={requestAmount}
                                onChange={(e) => setRequestAmount(e.target.value)}
                            />
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowRequestModal(false)}>
                        Cancel
                    </Button>
                    <Button variant="success" onClick={handleRequestMoney}>
                        Request {requestAmount || '0.00'} MKD
                    </Button>
                </Modal.Footer>
            </Modal>
            
            {/* Recent Transactions */}
            <Row className="mb-4">
                <Col md={12}>
                    <Card>
                        <Card.Header className="bg-white">
                            <h5 className="mb-0">Recent Transactions</h5>
                        </Card.Header>
                        <Card.Body className="p-0">
                            {transactions.length === 0 ? (
                                <div className="text-center py-4">
                                    <p className="text-muted mb-0">No transactions yet</p>
                                    <small className="text-muted">Start sending and receiving money!</small>
                                </div>
                            ) : (
                                <div className="list-group list-group-flush">
                                    {transactions.map((txn) => (
                                        <div
                                            key={txn.id}
                                            className="list-group-item d-flex justify-content-between align-items-center py-3"
                                        >
                                            <div>
                                                <h6 className="mb-1">{txn.description}</h6>
                                                <small className="text-muted">
                                                    {new Date(txn.createdAt).toLocaleDateString('en-US', {
                                                        month: 'short',
                                                        day: 'numeric',
                                                        year: 'numeric',
                                                        hour: '2-digit',
                                                        minute: '2-digit'
                                                    })}
                                                </small>
                                            </div>
                                            <div className="text-end">
                                                <div className={`fw-bold ${txn.type === 'Credit' ? 'text-success' : 'text-danger'}`}>
                                                    {txn.type === 'Credit' ? '+' : '-'}
                                                    {txn.amount.toFixed(2)} MKD
                                                </div>
                                                <small>
                                                    <span className="badge bg-success">{txn.status}</span>
                                                </small>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};

export default DashboardPage;