import React from 'react';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { useNavigate, Link } from 'react-router-dom';
import { authService, RegisterRequest } from '../services/authService';

const RegisterPage: React.FC = () => {
    const navigate = useNavigate();
    const [error, setError] = React.useState<string>('');
    const [success, setSuccess] = React.useState<string>('');

    const initialValues: RegisterRequest = {
        firstName: '',
        lastName: '',
        email: '',
        phoneNumber: '',
        password: '',
        confirmPassword: '',
    };

    const validationSchema = Yup.object({
        firstName: Yup.string()
            .min(2, 'First name must be at least 2 characters')
            .max(50, 'First name must be less than 50 characters')
            .required('First name is required'),

        lastName: Yup.string()
            .min(2, 'Last name must be at least 2 characters')
            .max(50, 'Last name must be less than 50 characters')
            .required('Last name is required'),

        email: Yup.string()
            .email('Invalid email address')
            .required('Email is required'),

        phoneNumber: Yup.string()
            .matches(/^[0-9+\-\s()]+$/, 'Invalid phone number')
            .min(8, 'Phone number must be at least 8 digits')
            .required('Phone number is required'),

        password: Yup.string()
            .min(6, 'Password must be at least 6 characters')
            .matches(
                /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/,
                'Password must contain at least one uppercase letter, one lowercase letter, and one number'
            )
            .required('Password is required'),
        confirmPassword: Yup.string()  // ← ADD THESE 3 LINES
            .oneOf([Yup.ref('password')], 'Passwords must match')
            .required('Please confirm your password'),
    });

    const handleSubmit = async (values: RegisterRequest) => {
        try {
            setError('');
            setSuccess('');
            await authService.register(values);
            setSuccess('Registration successful! Redirecting to dashboard...');
            setTimeout(() => {
                navigate('/dashboard');
            }, 1500);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Registration failed. Please try again.');
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-card" style={{maxWidth: '520px'}}>
                <div className="text-center mb-4">
                    <h2>Create MKPay Account</h2>
                    <p>Join thousands of users sending money instantly</p>
                </div>

                {error && (
                    <div className="alert alert-danger" role="alert">
                        {error}
                    </div>
                )}

                {success && (
                    <div className="alert alert-success" role="alert">
                        {success}
                    </div>
                )}

                <Formik
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onSubmit={handleSubmit}
                >
                    {({ isSubmitting }) => (
                        <Form>
                            <div className="row">
                                <div className="col-md-6 mb-3">
                                    <label htmlFor="firstName" className="form-label">
                                        First Name
                                    </label>
                                    <Field
                                        type="text"
                                        id="firstName"
                                        name="firstName"
                                        className="form-control"
                                        placeholder="Enter first name"
                                    />
                                    <ErrorMessage
                                        name="firstName"
                                        component="div"
                                        className="text-danger small mt-1"
                                    />
                                </div>

                                <div className="col-md-6 mb-3">
                                    <label htmlFor="lastName" className="form-label">
                                        Last Name
                                    </label>
                                    <Field
                                        type="text"
                                        id="lastName"
                                        name="lastName"
                                        className="form-control"
                                        placeholder="Enter last name"
                                    />
                                    <ErrorMessage
                                        name="lastName"
                                        component="div"
                                        className="text-danger small mt-1"
                                    />
                                </div>
                            </div>

                            <div className="mb-3">
                                <label htmlFor="email" className="form-label">
                                    Email Address
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

                            <div className="mb-3">
                                <label htmlFor="phoneNumber" className="form-label">
                                    Phone Number
                                </label>
                                <Field
                                    type="tel"
                                    id="phoneNumber"
                                    name="phoneNumber"
                                    className="form-control"
                                    placeholder="+389 XX XXX XXX"
                                />
                                <ErrorMessage
                                    name="phoneNumber"
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
                                    placeholder="Create a strong password"
                                />
                                <div className="form-text small">
                                    At least 6 characters with uppercase, lowercase, number and character
                                </div>
                                <ErrorMessage
                                    name="password"
                                    component="div"
                                    className="text-danger small mt-1"
                                />
                            </div>

                            <div className="mb-4">
                                <label htmlFor="confirmPassword" className="form-label">
                                    Confirm Password
                                </label>
                                <Field
                                    type="password"
                                    id="confirmPassword"
                                    name="confirmPassword"
                                    className="form-control"
                                    placeholder="Re-enter your password"
                                />
                                <ErrorMessage
                                    name="confirmPassword"
                                    component="div"
                                    className="text-danger small mt-1"
                                />
                            </div>


                            <button
                                type="submit"
                                className="btn btn-primary w-100 mb-3"
                                disabled={isSubmitting}
                            >
                                {isSubmitting ? 'Creating Account...' : 'Create Account'}
                            </button>

                            <div className="text-center">
                                <p className="mb-0">
                                    Already have an account?{' '}
                                    <Link to="/login" className="text-decoration-none fw-bold" style={{color: '#3d95ce'}}>
                                        Log in
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

export default RegisterPage;