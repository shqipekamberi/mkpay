import React from 'react';
import { Container, Row, Col, Button } from 'react-bootstrap';
import { Link } from 'react-router-dom';

const LandingPage: React.FC = () => {
    return (
        <div className="landing-hero">
            <Container>
                <Row className="justify-content-center text-center">
                    <Col md={10} lg={8}>
                        <h1>Pay friends.<br />Pay for everything.</h1>
                        <p className="lead mb-4">
                            Send and receive money instantly across North Macedonia.<br />
                            Fast, secure, and easy to use.
                        </p>
                        <div className="d-flex gap-3 justify-content-center flex-wrap">
                            <Link to="/register">
                                <Button variant="primary" size="lg">
                                    Get Started
                                </Button>
                            </Link>
                            <Link to="/login">
                                <Button variant="outline-primary" size="lg">
                                    Log In
                                </Button>
                            </Link>
                        </div>
                    </Col>
                </Row>

                {/* Optional: Add feature highlights */}
                <Row className="mt-5 pt-5">
                    <Col md={4} className="text-center mb-4">
                        <div className="feature-icon mb-3" style={{fontSize: '3rem'}}>⚡</div>
                        <h5>Instant Transfer</h5>
                        <p className="text-muted">Send money to anyone in seconds</p>
                    </Col>
                    <Col md={4} className="text-center mb-4">
                        <div className="feature-icon mb-3" style={{fontSize: '3rem'}}>🔒</div>
                        <h5>Secure</h5>
                        <p className="text-muted">Bank-level security for your money</p>
                    </Col>
                    <Col md={4} className="text-center mb-4">
                        <div className="feature-icon mb-3" style={{fontSize: '3rem'}}>📱</div>
                        <h5>Easy to Use</h5>
                        <p className="text-muted">Simple interface, no hassle</p>
                    </Col>
                </Row>
            </Container>
        </div>
    );
};

export default LandingPage;