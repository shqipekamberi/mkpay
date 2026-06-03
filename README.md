# MKPay - Peer-to-Peer Payment Application

A modern payment platform for North Macedonia, built with .NET 9.0 and React TypeScript.

## Project Information

- **Course:** Software Engineering (CCS-502)
- **Team Members:** Fitore Ahmedi, Shqipe Kamberi
- **Date:** June 2026
- **Repository:** https://github.com/shqipekamberi/mkpay

## Overview

MKPay is a Venmo-style peer-to-peer payment application that enables users to send and receive money instantly within North Macedonia. The application features secure authentication, real-time balance tracking, and a modern, responsive user interface.

## Technology Stack

### Backend
- **.NET 9.0** - Web API Framework
- **Entity Framework Core** - Database ORM
- **ASP.NET Identity** - User Management
- **JWT Authentication** - Secure Token-based Auth
- **Azure SQL Database** - Cloud Database

### Frontend
- **React 18** with TypeScript
- **React Router** - Client-side Routing
- **Axios** - HTTP Client
- **Formik & Yup** - Form Validation (Login & Register)
- **Bootstrap 5** - UI Components

## Key Features

### Implemented
- ✅ User Registration with validation
- ✅ JWT-based Login System
- ✅ Real-time Balance Display
- ✅ Account Management
- ✅ Send Money with real-time transfer
- ✅ Request Money with Accept/Decline
- ✅ Transaction History page
- ✅ Update Profile
- ✅ Email notifications on transactions
- ✅ Payment Requests management
- ✅ Responsive Design (Mobile & Desktop)

### Security Features
- Password requirements (8+ chars, uppercase, lowercase, number, special char)
- JWT token authentication with 24-hour expiration
- Secure password hashing with ASP.NET Identity
- CORS configuration for frontend security

## Local Development

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+
- Azure SQL Database

### Backend Setup
```bash
# Clone repository
git clone https://github.com/shqipekamberi/mkpay.git
cd mkpay

# Create appsettings.Development.json in MKPay.API/ with your connection string:
# {
#   "ConnectionStrings": {
#     "DefaultConnection": "your-connection-string-here"
#   },
#   "JwtSettings": {
#     "SecretKey": "your-secret-key-min-32-chars",
#     "Issuer": "MKPay",
#     "Audience": "MKPayUsers",
#     "ExpirationHours": 24
#   }
# }

# Run migrations
dotnet ef database update --project MKPay.Infrastructure --startup-project MKPay.API

# Run the API
cd MKPay.API
dotnet run
# Backend runs on http://localhost:5251
```

### Frontend Setup
```bash
cd frontend

# Install dependencies
npm install

# Create environment file
echo "REACT_APP_API_URL=http://localhost:5251/api" > .env.local

# Start development server
npm start
# Frontend runs on http://localhost:3000
```

### Running Tests
```bash
dotnet test MKPay.Tests
# 20 tests should pass
```

## Database Schema

**Main Entities:**
- `ApplicationUser` - User accounts with ASP.NET Identity
- `Account` - User payment accounts with balance tracking
- `Transaction` - Money transfer records
- `PaymentRequest` - Payment request records
- `AuditLog` - User activity tracking

## Project Structure
mkpay/
├── MKPay.API/              # Web API Controllers & Configuration
├── MKPay.Core/             # Domain Models, DTOs, Interfaces
├── MKPay.Infrastructure/   # Data Access, Services, Repositories
├── MKPay.Tests/            # Unit Tests (20 tests)
└── frontend/               # React TypeScript Application
├── src/
│   ├── pages/          # React Page Components
│   ├── services/       # API Service Clients
│   └── types/          # TypeScript Type Definitions
└── public/

## Deployment

The application is designed to be deployed on Azure App Service with Azure SQL Database. See local setup instructions above for running the project locally.

## Learning Outcomes

This project demonstrates:
- RESTful API design and implementation
- JWT authentication and authorization
- Entity Framework Core with Code-First migrations
- React TypeScript application development
- Clean Architecture (Core / Infrastructure / API layers)
- Full-stack application integration

## Future Enhancements

- Transaction history with search and filtering
- Real-time notifications
- Bill splitting functionality
- QR code-based payments
- Mobile application (iOS/Android)

## Contributors

- **Fitore Ahmedi** - Backend Development & Database Architecture
- **Shqipe Kamberi** - Frontend Development, UI/UX Design, Payment Requests & Transaction History, Azure Deployment