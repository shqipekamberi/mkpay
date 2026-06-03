import React, { useState, useEffect } from 'react';
import {
    Container, Row, Col, Card, Table, Badge,
    Pagination, Spinner, Alert,
} from 'react-bootstrap';
import apiClient from '../services/apiClient';

interface Transaction {
    id: string;
    amount: number;
    currency: string;
    description: string;
    type: string;
    status: string;
    createdAt: string;
}

interface PagedResponse {
    items: Transaction[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
}

const PAGE_SIZE = 10;

const TransactionHistoryPage: React.FC = () => {
    const [transactions, setTransactions] = useState<Transaction[]>([]);
    const [totalCount, setTotalCount] = useState(0);
    const [page, setPage] = useState(1);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchTransactions = async () => {
            setLoading(true);
            setError(null);
            try {
                const response = await apiClient.get('/transaction/my-transactions', {
                    params: { pageNumber: page, pageSize: PAGE_SIZE },
                });
                const data: PagedResponse = response.data.data || response.data;
                if (Array.isArray(data)) {
                    setTransactions(data);
                    setTotalCount(data.length);
                } else {
                    setTransactions(data.items || []);
                    setTotalCount(data.totalCount || 0);
                }
            } catch {
                setError('Failed to load transactions.');
            } finally {
                setLoading(false);
            }
        };
        fetchTransactions();
    }, [page]);

    const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

    const formatDate = (iso: string) =>
        new Date(iso).toLocaleDateString('en-US', {
            month: 'short', day: 'numeric', year: 'numeric',
            hour: '2-digit', minute: '2-digit',
        });

    const typeBadge = (type: string) => {
        const isCredit = type?.toLowerCase() === 'credit';
        return (
            <span className={`fw-bold ${isCredit ? 'text-success' : 'text-danger'}`}>
                {isCredit ? '+' : '-'}
            </span>
        );
    };

    const statusBadge = (status: string) => {
        const map: Record<string, string> = {
            completed: 'success',
            pending: 'warning',
            failed: 'danger',
        };
        return (
            <Badge bg={map[status?.toLowerCase()] ?? 'secondary'}>
                {status}
            </Badge>
        );
    };

    const paginationItems = [];
    for (let i = 1; i <= totalPages; i++) {
        paginationItems.push(
            <Pagination.Item key={i} active={i === page} onClick={() => setPage(i)}>
                {i}
            </Pagination.Item>
        );
    }

    return (
        <Container className="mt-4">
            <Row className="mb-4">
                <Col>
                    <h2>Transaction History</h2>
                    <p className="text-muted">All your past transactions</p>
                </Col>
            </Row>

            {error && <Alert variant="danger">{error}</Alert>}

            <Card>
                <Card.Body className="p-0">
                    {loading ? (
                        <div className="text-center py-5">
                            <Spinner animation="border" variant="primary" />
                            <p className="mt-2 text-muted">Loading transactions...</p>
                        </div>
                    ) : transactions.length === 0 ? (
                        <div className="text-center py-5 text-muted">
                            <p className="mb-1">No transactions found</p>
                            <small>Transactions will appear here after you send or receive money.</small>
                        </div>
                    ) : (
                        <Table responsive hover className="mb-0">
                            <thead className="table-light">
                                <tr>
                                    <th>Date</th>
                                    <th>Description</th>
                                    <th>Type</th>
                                    <th className="text-end">Amount</th>
                                    <th className="text-center">Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                {transactions.map(txn => (
                                    <tr key={txn.id}>
                                        <td className="text-muted small align-middle">
                                            {formatDate(txn.createdAt)}
                                        </td>
                                        <td className="align-middle">{txn.description || '—'}</td>
                                        <td className="align-middle">{txn.type}</td>
                                        <td className="text-end align-middle fw-bold">
                                            {typeBadge(txn.type)}
                                            {txn.amount.toFixed(2)} {txn.currency}
                                        </td>
                                        <td className="text-center align-middle">
                                            {statusBadge(txn.status)}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </Table>
                    )}
                </Card.Body>

                {!loading && totalPages > 1 && (
                    <Card.Footer className="d-flex justify-content-between align-items-center bg-white">
                        <small className="text-muted">
                            Showing page {page} of {totalPages} ({totalCount} total)
                        </small>
                        <Pagination className="mb-0">
                            <Pagination.Prev disabled={page === 1} onClick={() => setPage(p => p - 1)} />
                            {paginationItems}
                            <Pagination.Next disabled={page === totalPages} onClick={() => setPage(p => p + 1)} />
                        </Pagination>
                    </Card.Footer>
                )}
            </Card>
        </Container>
    );
};

export default TransactionHistoryPage;
