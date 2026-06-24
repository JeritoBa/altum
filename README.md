# Altum - Apartment Rental Management Platform

## Overview

**Altum** is a modern apartment rental management platform designed to provide security, comfort, and efficiency for both property owners and tenants.

The application is built using **ASP.NET MVC**, leveraging **ASP.NET Identity** for authentication and authorization, while following a scalable architecture that integrates messaging, cloud storage, artificial intelligence, and background processing services.

---

## Architecture

### Core Components

#### 1. Main Application (ASP.NET MVC)

The primary web application responsible for:

* User registration and authentication
* Apartment management
* Rental requests
* Administrative operations
* Communication with external services

Technologies:

* ASP.NET MVC
* ASP.NET Identity
* Entity Framework Core

---

#### 2. RabbitMQ (Under Construction 🚧)

RabbitMQ acts as the system's message broker and event bus.

Responsibilities:

* Publish domain events
* Decouple services
* Enable asynchronous processing
* Improve scalability and reliability

> **Status:** Under Construction

---

#### 3. EmailWorker (Under Construction 🚧)

Background worker responsible for processing email-related events.

Planned responsibilities:

* Listen to RabbitMQ events
* Send registration emails
* Send notifications
* Send system alerts

> **Status:** Under Construction

---

#### 4. IdentityWorker (Under Construction 🚧)

Background worker responsible for identity verification.

Planned responsibilities:

* Listen to RabbitMQ events
* Process user verification requests
* Retrieve uploaded photos
* Communicate with AI services for identity validation

> **Status:** Under Construction

---

#### 5. PostgreSQL

Primary relational database used by the platform.

Responsibilities:

* User data storage
* Apartment information
* Rental records
* Audit information
* Application data persistence

---

#### 6. Cloudinary

Cloud-based media storage service.

Responsibilities:

* Secure image storage
* Image optimization
* Image delivery
* User photo management

---

#### 7. Grok AI

Artificial Intelligence service used for image recognition and identity verification.

Responsibilities:

* Analyze uploaded user photos
* Assist identity validation processes
* Support automated verification workflows

---

## System Flow

```text
Main Application
├── PostgreSQL
├── Cloudinary
└── RabbitMQ
    ├── EmailWorker
    └── IdentityWorker
            └── Grok AI
```

### Identity Verification Flow

1. User uploads an identification photo.
2. Main Application stores the image in Cloudinary.
3. Main Application publishes a verification event to RabbitMQ.
4. IdentityWorker consumes the event.
5. IdentityWorker retrieves the image.
6. IdentityWorker sends the image to Grok AI.
7. Grok AI returns the verification result.
8. IdentityWorker processes the result and publishes the outcome.
9. Main Application updates the user's verification status.

---

## Technology Stack

| Technology            | Purpose                        |
| --------------------- | ------------------------------ |
| ASP.NET MVC           | Web Application                |
| ASP.NET Identity      | Authentication & Authorization |
| Entity Framework Core | ORM                            |
| PostgreSQL            | Relational Database            |
| RabbitMQ              | Message Broker                 |
| Cloudinary            | Image Storage                  |
| Grok AI               | Image Recognition              |
| C#                    | Backend Language               |

---

## Demo Accounts

### Owner Account

```text
Email: owner@altum.com
Password: Owner1234$
```

### Guest Account

```text
Email: guest@altum.com
Password: Guest1234$
```

---

## Current Development Status

| Component               | Status                |
| ----------------------- | --------------------- |
| Main Application        | ✅ Active Development  |
| PostgreSQL Integration  | ✅ Implemented         |
| Cloudinary Integration  | ✅ Implemented         |
| RabbitMQ Infrastructure | 🚧 Under Construction |
| EmailWorker             | 🚧 Under Construction |
| IdentityWorker          | 🚧 Under Construction |
| Grok AI Integration     | 🚧 Under Construction |

---

## Vision

Altum aims to provide a secure, luxurious, and reliable apartment rental experience by combining modern software architecture, cloud services, and artificial intelligence to streamline property management and tenant verification.

**Altum**
*Your peace of mind, our priority.*
