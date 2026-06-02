import React, { useState, useEffect, useCallback } from 'react';
import { Container, Row, Col, Card, Badge, Button, Alert, Spinner } from 'react-bootstrap';
import { paymentRequestService, PaymentRequest } from '../services/paymentRequestService';

const PaymentRequestsPage: React.FC = () => {
    const [requests, setRequests] = useState<PaymentRequest[]>([]);
    const [loading, setLoading] = useState(true);
    const [actionLoading, setActionLoading] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [successMsg, setSuccessMsg] = useState<string | null>(null);

    const fetchRequests = useCallback(async () => {
        try {
            setError(null);
            const data = await paymentRequestService.getReceivedRequests();
            setRequests(data);
        } catch {
            setError('Failed to load payment requests.');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchRequests();
    }, [fetchRequests]);

    const handleAccept = async (id: string) => {
        setActionLoading(id + '-accept');
        try {
            await paymentRequestService.acceptRequest(id);
            setSuccessMsg('Payment request accepted.');
            await fetchRequests();
        } catch {
            setError('Failed to accept request.');
        } finally {
            setActionLoading(null);
        }
    };

    const handleDecline = async (id: string) => {
        setActionLoading(id + '-decline');
        try {
            await paymentRequestService.declineRequest(id);
            setSuccessMsg('Payment request declined.');
            await fetchRequests();
        } catch {
            setError('Failed to decline request.');
        } finally {
            setActionLoading(null);
        }
    };

    const pending = requests.filter(r => r.status === 'Pending');
    const past = requests.filter(r => r.status !== 'Pending');

    const formatDate = (iso: string) =>
        new Date(iso).toLocaleDateString('en-US', {
            month: 'short', day: 'numeric', year: 'numeric',
            hour: '2-digit', minute: '2-digit',
        });

    const statusBadge = (status: PaymentRequest['status']) => {
        const map: Record<string, string> = {
            Pending: 'warning',
            Accepted: 'success',
            Declined: 'danger',
            Cancelled: 'secondary',
        };
        return <Badge bg={map[status] ?? 'secondary'}>{status}</Badge>;
    };

    const RequestCard = ({ req, showActions }: { req: PaymentRequest; showActions: boolean }) => (
        <div className="list-group-item d-flex justify-content-between align-items-center py-3">
            <div>
                <h6 className="mb-1">
                    {req.requesterFirstName} {req.requesterLastName}
                    <span className="text-muted ms-2 fw-normal small">{req.requesterEmail}</span>
                </h6>
                {req.description && <small className="text-muted">{req.description}</small>}
                <div><small className="text-muted">{formatDate(req.createdAt)}</small></div>
            </div>
            <div className="text-end">
                <div className="fw-bold text-primary mb-1">
                    {req.amount.toFixed(2)} {req.currency}
                </div>
                {showActions ? (
                    <div className="d-flex gap-2">
                        <Button
                            size="sm"
                            variant="success"
                            disabled={!!actionLoading}
                            onClick={() => handleAccept(req.id)}
                        >
                            {actionLoading === req.id + '-accept'
                                ? <Spinner size="sm" animation="border" />
                                : 'Accept'}
                        </Button>
                        <Button
                            size="sm"
                            variant="outline-danger"
                            disabled={!!actionLoading}
                            onClick={() => handleDecline(req.id)}
                        >
                            {actionLoading === req.id + '-decline'
                                ? <Spinner size="sm" animation="border" />
                                : 'Decline'}
                        </Button>
                    </div>
                ) : (
                    statusBadge(req.status)
                )}
            </div>
        </div>
    );

    if (loading) {
        return (
            <Container className="text-center mt-5">
                <Spinner animation="border" variant="primary" />
                <p className="mt-2">Loading payment requests...</p>
            </Container>
        );
    }

    return (
        <Container className="mt-4">
            <Row className="mb-4">
                <Col>
                    <h2>Payment Requests</h2>
                    <p className="text-muted">Manage incoming payment requests</p>
                </Col>
            </Row>

            {error && <Alert variant="danger" onClose={() => setError(null)} dismissible>{error}</Alert>}
            {successMsg && <Alert variant="success" onClose={() => setSuccessMsg(null)} dismissible>{successMsg}</Alert>}

            <Row className="mb-4">
                <Col>
                    <Card>
                        <Card.Header className="bg-white">
                            <h5 className="mb-0">
                                Pending{' '}
                                {pending.length > 0 && (
                                    <Badge bg="warning" text="dark">{pending.length}</Badge>
                                )}
                            </h5>
                        </Card.Header>
                        <Card.Body className="p-0">
                            {pending.length === 0 ? (
                                <div className="text-center py-4 text-muted">No pending requests</div>
                            ) : (
                                <div className="list-group list-group-flush">
                                    {pending.map(req => (
                                        <RequestCard key={req.id} req={req} showActions />
                                    ))}
                                </div>
                            )}
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            <Row>
                <Col>
                    <Card>
                        <Card.Header className="bg-white">
                            <h5 className="mb-0">Past Requests</h5>
                        </Card.Header>
                        <Card.Body className="p-0">
                            {past.length === 0 ? (
                                <div className="text-center py-4 text-muted">No past requests</div>
                            ) : (
                                <div className="list-group list-group-flush">
                                    {past.map(req => (
                                        <RequestCard key={req.id} req={req} showActions={false} />
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

export default PaymentRequestsPage;
