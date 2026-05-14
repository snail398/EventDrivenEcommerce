# Event-driven E-commerce Backend

Pet project for practicing production-style backend development with ASP.NET Core, PostgreSQL, RabbitMQ, Docker, Outbox Pattern, idempotent consumers and structured logging.

## Services

- Catalog.Api
- Order.Api
- Payment.Api
- Notification.Api

## Infrastructure

- PostgreSQL
- RabbitMQ
- Docker Compose

## Main Flow dasd

POST /api/v1/orders
↓
Order.Api saves order + outbox message
↓
OutboxProcessor publishes OrderCreatedEvent
↓
Payment.Api consumes order.created
↓
Payment.Api publishes payment.succeeded or payment.failed
↓
Order.Api consumes payment event
↓
Order status becomes Confirmed or Failed

## Patterns
- API Versioning
- Repository Pattern
- Outbox Pattern
- Manual ACK
- Retry / DLQ
- Idempotency
- Health Checks
- Structured Logging
- CorrelationId

## Run
docker compose up -d --build

## Health Checks
GET http://localhost:5001/health
GET http://localhost:5002/health
GET http://localhost:5003/health
GET http://localhost:5004/health