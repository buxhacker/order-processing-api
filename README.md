# Order Processing Service

Small microservice in C# (.NET 8) that processes incoming “orders” asynchronously and stores them in a database.

## Features

- HTTP API: Submit orders instantly; background processing via RabbitMQ.
- Orders persisted to PostgreSQL via EF Core.
- Asynchronous worker marks orders processed.
- Observability through simple logging.

## How to Run

```bash
docker-compose up --build
```
Visit `http://localhost:8080/swagger` for API docs.

## Design Decisions

- **WolverineFx** greatly streamlines async workflows, background processing, and RabbitMQ integration. Message handlers are clean and reliable.
- **RabbitMQ** chosen by Wolverine for transport, enabling reliable, decoupled processing.
- **PostgreSQL** for transactional storage.
- Logging for processed order metrics. Easily extended with Prometheus/OpenTelemetry.

## API Example

```http
POST /api/orders
{
  "orderId": "abcd-1334",
  "customerId": "cust-42",
  "totalAmount": 150.0,
  "items": [
    { "name": "itemA", "quantity": 2, "price": 50.0 },
    { "name": "itemB", "quantity": 1, "price": 30.0 },
    { "name": "itemC", "quantity": 3, "price": 10.0 }
  ]
}
```