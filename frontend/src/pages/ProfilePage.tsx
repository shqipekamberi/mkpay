import React, { useState, useEffect } from 'react';
import {
    Container, Row, Col, Card, Form, Button, Alert, Spinner,
} from 'react-bootstrap';
import apiClient from '../services/apiClient';

interface UserProfile {
    firstName: string;
    lastName: string;
    email: string;
    profilePictureUrl?: string;
}

const ProfilePage: React.FC = () => {
    const [profile, setProfile] = useState<UserProfile>({
        firstName: '',
        lastName: '',
        email: '',
        profilePictureUrl: '',
    });
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const response = await apiClient.get('/account/profile');
                const data = response.data.data || response.data;
                setProfile({
                    firstName: data.firstName || '',
                    lastName: data.lastName || '',
                    email: data.email || '',
                    profilePictureUrl: data.profilePictureUrl || '',
                });
            } catch {
                setError('Failed to load profile.');
            } finally {
                setLoading(false);
            }
        };
        fetchProfile();
    }, []);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setProfile(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSaving(true);
        setError(null);
        setSuccess(null);
        try {
            await apiClient.patch('/account/profile', {
                firstName: profile.firstName,
                lastName: profile.lastName,
                profilePictureUrl: profile.profilePictureUrl || null,
            });
            setSuccess('Profile updated successfully.');

            // Keep localStorage user info in sync
            const stored = localStorage.getItem('user');
            if (stored) {
                const user = JSON.parse(stored);
                user.firstName = profile.firstName;
                user.lastName = profile.lastName;
                localStorage.setItem('user', JSON.stringify(user));
            }
        } catch (err: any) {
            const msg =
                err.response?.data?.message ||
                err.response?.data?.errors?.[0] ||
                'Failed to update profile.';
            setError(msg);
        } finally {
            setSaving(false);
        }
    };

    if (loading) {
        return (
            <Container className="text-center mt-5">
                <Spinner animation="border" variant="primary" />
                <p className="mt-2">Loading profile...</p>
            </Container>
        );
    }

    return (
        <Container className="mt-4">
            <Row className="justify-content-center">
                <Col md={7} lg={6}>
                    <h2 className="mb-1">Profile</h2>
                    <p className="text-muted mb-4">Update your personal information</p>

                    {error && <Alert variant="danger" onClose={() => setError(null)} dismissible>{error}</Alert>}
                    {success && <Alert variant="success" onClose={() => setSuccess(null)} dismissible>{success}</Alert>}

                    {profile.profilePictureUrl && (
                        <div className="text-center mb-4">
                            <img
                                src={profile.profilePictureUrl}
                                alt="Profile"
                                className="rounded-circle"
                                style={{ width: 96, height: 96, objectFit: 'cover' }}
                                onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }}
                            />
                        </div>
                    )}

                    <Card>
                        <Card.Body>
                            <Form onSubmit={handleSubmit}>
                                <Row>
                                    <Col sm={6}>
                                        <Form.Group className="mb-3">
                                            <Form.Label>First Name</Form.Label>
                                            <Form.Control
                                                type="text"
                                                name="firstName"
                                                value={profile.firstName}
                                                onChange={handleChange}
                                                required
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col sm={6}>
                                        <Form.Group className="mb-3">
                                            <Form.Label>Last Name</Form.Label>
                                            <Form.Control
                                                type="text"
                                                name="lastName"
                                                value={profile.lastName}
                                                onChange={handleChange}
                                                required
                                            />
                                        </Form.Group>
                                    </Col>
                                </Row>

                                <Form.Group className="mb-3">
                                    <Form.Label>Email</Form.Label>
                                    <Form.Control
                                        type="email"
                                        value={profile.email}
                                        readOnly
                                        className="bg-light"
                                    />
                                    <Form.Text className="text-muted">Email cannot be changed.</Form.Text>
                                </Form.Group>

                                <Form.Group className="mb-4">
                                    <Form.Label>Profile Picture URL</Form.Label>
                                    <Form.Control
                                        type="url"
                                        name="profilePictureUrl"
                                        value={profile.profilePictureUrl || ''}
                                        onChange={handleChange}
                                        placeholder="https://example.com/photo.jpg"
                                    />
                                </Form.Group>

                                <Button
                                    type="submit"
                                    variant="primary"
                                    disabled={saving}
                                    className="w-100"
                                >
                                    {saving ? (
                                        <><Spinner size="sm" animation="border" className="me-2" />Saving...</>
                                    ) : (
                                        'Save Changes'
                                    )}
                                </Button>
                            </Form>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};

export default ProfilePage;
