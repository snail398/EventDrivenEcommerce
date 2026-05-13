# Event-driven E-commerce Backend

Backend pet project for practicing ASP.NET Core, RabbitMQ, PostgreSQL, Docker, microservices, Outbox Pattern and idempotent event consumers.

## Services

- Catalog Service
- Basket Service
- Order Service
- Payment Service
- Notification Service

## Main Flow

OrderCreated -> PaymentRequested -> PaymentSucceeded/PaymentFailed -> OrderConfirmed/OrderFailed