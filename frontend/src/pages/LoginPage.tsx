import React from 'react';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { useNavigate, Link } from 'react-router-dom';
import { authService, LoginRequest } from '../services/authService';

const LoginPage: React.FC = () => {
    const navigate = useNavigate();
    const [error, setError] = React.useState<string>('');

    const initialValues: LoginRequest = {
        email: '',
        password: '',
    };

    const validationSchema = Yup.object({
        email: Yup.string()
            .email('Invalid email address')
            .required('Email is required'),
        password: Yup.string()
            .min(6, 'Password must be at least 6 characters')
            .required('Password is required'),
    });

    const handleSubmit = async (values: LoginRequest) => {
        try {
            setError('');
            await authService.login(values);
            navigate('/dashboard');
        } catch (err: any) {
            setError(err.response?.data?.message || 'Login failed. Please try again.');
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-card">
                <div className="text-center mb-4">
                    <h2>Welcome back</h2>
                    <p>Log in to your MKPay account</p>
                </div>

                {error && (
                    <div className="alert alert-danger" role="alert">
                        {error}
                    </div>
                )}

                <Formik
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onSubmit={handleSubmit}
                >
                    {({ isSubmitting }) => (
                        <Form>
                            <div className="mb-3">
                                <label htmlFor="email" className="form-label">
                                    Email
                                </label>
                                <Field
                                    type="email"
                                    id="email"
                                    name="email"
                                    className="form-control"
                                    placeholder="you@example.com"
                                />
                                <ErrorMessage
                                    name="email"
                                    component="div"
                                    className="text-danger small mt-1"
                                />
                            </div>

                            <div className="mb-4">
                                <label htmlFor="password" className="form-label">
                                    Password
                                </label>
                                <Field
                                    type="password"
                                    id="password"
                                    name="password"
                                    className="form-control"
                                    placeholder="Enter your password"
                                />
                                <ErrorMessage
                                    name="password"
                                    component="div"
                                    className="text-danger small mt-1"
                                />
                            </div>

                            <button
                                type="submit"
                                className="btn btn-primary w-100 mb-3"
                                disabled={isSubmitting}
                            >
                                {isSubmitting ? 'Logging in...' : 'Log In'}
                            </button>

                            <div className="text-center">
                                <p className="mb-0">
                                    Don't have an account?{' '}
                                    <Link to="/register" className="text-decoration-none fw-bold" style={{color: '#3d95ce'}}>
                                        Sign up
                                    </Link>
                                </p>
                            </div>
                        </Form>
                    )}
                </Formik>
            </div>
        </div>
    );
};

export default LoginPage;