# MKPay - Peer-to-Peer Payment Application

A modern payment platform for North Macedonia, built with .NET 9.0 and React TypeScript.

## Project Information

- **Course:** C# .NET Programming
- **Team Members:** Fitore Ahmedi, Shqipe Kamberi
- **Date:** February 2026
- **Repository:** https://github.com/fitoreahmedi/mkpay

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
- **Formik & Yup** - Form Validation
- **Bootstrap 5** - UI Components

### Cloud & DevOps
- **Azure App Service** - Backend Hosting
- **Azure SQL Database** - Database Hosting
- **GitHub Actions** - CI/CD Pipeline

## ✨ Key Features

### Implemented
- ✅ User Registration with validation
- ✅ JWT-based Login System
- ✅ Real-time Balance Display
- ✅ Account Management
- ✅ Send Money (UI Complete)
- ✅ Request Money (UI Complete)
- ✅ Responsive Design (Mobile & Desktop)

### Security Features
- Password requirements (8+ chars, uppercase, lowercase, number, special char)
- JWT token authentication with 24-hour expiration
- Secure password hashing with ASP.NET Identity
- CORS configuration for frontend security

## Live Deployment

- **Backend API:** https://mkpay-api-ddgsbycjghc9f4cn.italynorth-01.azurewebsites.net
- **API Documentation:** https://mkpay-api-ddgsbycjghc9f4cn.italynorth-01.azurewebsites.net/swagger
- **Database:** Azure SQL Database (mkpay-db)

## Local Development

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+
- SQL Server or Azure SQL Database

### Backend Setup
```bash
# Clone repository
git clone https://github.com/fitoreahmedi/mkpay.git
cd mkpay/MKPay.API

# Update appsettings.json with your connection string

# Run migrations
dotnet ef database update

# Run the API
dotnet run
# Backend runs on http://localhost:5251
```

### Frontend Setup
```bash
cd frontend

# Install dependencies
npm install

# Create environment file
echo "REACT_APP_API_URL=https://mkpay-api-ddgsbycjghc9f4cn.italynorth-01.azurewebsites.net/api" > .env.local

# Start development server
npm start
# Frontend runs on http://localhost:3000
```

##  Database Schema

**Main Entities:**
- `ApplicationUser` - User accounts with ASP.NET Identity
- `Account` - User payment accounts with balance tracking
- `Transaction` - Money transfer records
- `PaymentRequest` - Payment request records
- `AuditLog` - User activity tracking

## Project Structure
```
mkpay/
├── MKPay.API/              # Web API Controllers & Configuration
├── MKPay.Core/             # Domain Models, DTOs, Interfaces
├── MKPay.Infrastructure/   # Data Access, Services, Repositories
├── MKPay.Tests/            # Unit & Integration Tests
└── frontend/               # React TypeScript Application
    ├── src/
    │   ├── pages/          # React Page Components
    │   ├── services/       # API Service Clients
    │   └── types/          # TypeScript Type Definitions
    └── public/
```

##  Learning Outcomes

This project demonstrates:
- RESTful API design and implementation
- JWT authentication and authorization
- Entity Framework Core with Code-First migrations
- React TypeScript application development
- Azure cloud deployment
- CI/CD with GitHub Actions
- Full-stack application integration

## Future Enhancements

- Transaction history with search and filtering
- Real-time notifications
- Bill splitting functionality
- QR code-based payments
- Email notifications for transactions
- Mobile application (iOS/Android)

## Contributors

- **Fitore Ahmedi** - Backend Development & Database Architecture
- **Shqipe Kamberi** - Frontend Development & UI/UX Design & Azure Deployment
