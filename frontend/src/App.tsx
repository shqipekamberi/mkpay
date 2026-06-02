import 'bootstrap/dist/css/bootstrap.min.css';
import { BrowserRouter as Router, Routes, Route, Navigate, Link } from 'react-router-dom';
import { authService } from './services/authService';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import DashboardPage from './pages/DashboardPage';
import LandingPage from './pages/LandingPage';
import PaymentRequestsPage from './pages/PaymentRequestsPage';
import TransactionHistoryPage from './pages/TransactionHistoryPage';
import ProfilePage from './pages/ProfilePage';
import { useNavigate} from "react-router-dom";
import React from "react";
import './App.css';

// Navigation Component
// Navigation Component with proper re-render
const Navigation = () => {
    const [isAuthenticated, setIsAuthenticated] = React.useState(authService.isAuthenticated());
    const navigate = useNavigate();

    React.useEffect(() => {
        const checkAuth = () => {
            setIsAuthenticated(authService.isAuthenticated());
        };

        // Listen for our custom auth state change event
        window.addEventListener('authStateChanged', checkAuth);

        return () => window.removeEventListener('authStateChanged', checkAuth);
    }, []);

    const handleLogout = () => {
        authService.logout();
        navigate('/login');
    };

    return (
        <nav className="navbar navbar-expand-lg navbar-light bg-white">
            <div className="container">
                <Link className="navbar-brand fw-bold" to="/" style={{color: '#3d95ce'}}>
                    MKPay
                </Link>

                <div className="d-flex align-items-center gap-2">
                    {isAuthenticated ? (
                        <>
                            <Link className="btn btn-link text-decoration-none" to="/dashboard">Dashboard</Link>
                            <Link className="btn btn-link text-decoration-none" to="/payment-requests">Requests</Link>
                            <Link className="btn btn-link text-decoration-none" to="/transactions">History</Link>
                            <Link className="btn btn-link text-decoration-none" to="/profile">Profile</Link>
                            <button
                                className="btn btn-outline-danger"
                                onClick={handleLogout}
                            >
                                Logout
                            </button>
                        </>
                    ) : (
                        <>
                            <Link to="/login">
                                <button className="btn btn-outline-primary">
                                    Log in
                                </button>
                            </Link>
                            <Link to="/register">
                                <button className="btn btn-primary">
                                    Sign up
                                </button>
                            </Link>
                        </>
                    )}
                </div>
            </div>
        </nav>
    );
};
// Protected Route Component
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
    if (!authService.isAuthenticated()) {
        return <Navigate to="/login" replace />;
    }
    return <>{children}</>;
};

function App() {
    const isAuthenticated = authService.isAuthenticated();

    return (
        <Router>
            <div className="App">
                {/* Show navigation on all pages */}
                <Navigation />

                <Routes>
                    {/* Home route - show landing if not logged in, dashboard if logged in */}
                    <Route
                        path="/"
                        element={
                            isAuthenticated ?
                                <Navigate to="/dashboard" replace /> :
                                <LandingPage />
                        }
                    />

                    {/* Auth routes */}
                    <Route path="/login" element={<LoginPage />} />
                    <Route path="/register" element={<RegisterPage />} />

                    {/* Protected dashboard */}
                    <Route
                        path="/dashboard"
                        element={
                            <ProtectedRoute>
                                <DashboardPage />
                            </ProtectedRoute>
                        }
                    />

                    {/* Protected pages */}
                    <Route
                        path="/payment-requests"
                        element={
                            <ProtectedRoute>
                                <PaymentRequestsPage />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/transactions"
                        element={
                            <ProtectedRoute>
                                <TransactionHistoryPage />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/profile"
                        element={
                            <ProtectedRoute>
                                <ProfilePage />
                            </ProtectedRoute>
                        }
                    />

                    {/* Catch all - redirect to home */}
                    <Route path="*" element={<Navigate to="/" replace />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;