# Decision Records

## Out of Scope

Given the requirements, the following assumptions were made due to lack of specification:

### Authentication & Authorization

Endpoints do not require authentication or authorization at this stage. In a production system, this would typically be addressed using API keys or OAuth 2.0 bearer tokens scoped per merchant.

### Merchant and Payer Identification

No merchant or payer information is included in the payment request. A production implementation would associate each request with a merchant account and optionally a payer profile to support reconciliation and fraud detection.

### Multiple Payment Provider Channels

It is assumed there is a single acquiring bank integration. The architecture is designed to accommodate additional providers in the future without significant rework (see `IPaymentProcessor` section below).

### Idempotency

No idempotency key mechanism is implemented. In a production payment gateway, duplicate submission protection is critical to prevent double charges. Without user/merchant identification, it's not feasible to assume a uniqueness of a transaction since the application cannot store the full card number.

### Webhooks / Async Notifications

Payment processing is handled synchronously. Asynchronous processing with webhook-based notifications is out of scope but would be required for production-grade reliability.

---

## Project Architecture

**Context:**
Define a general solution architecture and distribute responsibilities across projects to maintain separation of concerns.

**Decision:**
Adopt an N-Tier layered architecture to isolate components, aiming for maintainability and extensibility.

| Layer | Project | Responsibilities |
|---|---|---|
| API | `PaymentGateway.Api` | Controllers, middlewares, dependency injection setup |
| Contracts | `PaymentGateway.Contracts` | Request/response DTOs, input format constraints |
| Application | `PaymentGateway.Application` | Business logic, validation, models, interfaces, services |

**Alternatives Considered:**

DDD and CQRS were evaluated but not adopted. The domain is simple and does not contain sufficient complexity to justify the overhead these patterns introduce. They remain viable candidates if the domain grows significantly.

---

## Storing the Authorization Code from the Acquiring Bank

**Context:**
The acquiring bank returns an authorization code upon a successful transaction.

**Decision:**
Store the authorization code in the payment record to enable reconciliation between the payment gateway and the acquiring bank's transaction log.

**Rationale:**
This code is the primary linkage between the two systems and is essential for dispute resolution, refunds, and auditing.

---

## Input Format Constraints in Contracts; Business Logic Validation in Application

**Context:**
Validation concerns are split between structural (format) and logical (business rules).

**Decision:**
- Format constraints (e.g., card number length, expiry date format) are enforced in `PaymentGateway.Contracts` using data annotations.
- Business logic validations (e.g., expiry date must be in the future, Luhn check) are handled in `PaymentGateway.Application` using a dedicated validator.
- `SubmitPaymentRequest` is defined as a `record` to ensure immutability across service boundaries, preventing accidental mutation of sensitive data.

---

## API Error Responses Follow RFC 7807 (Problem Details)

**Decision:**
Use the .NET built-in `ProblemDetails` format as the standard error response structure.

**Rationale:**
RFC 7807 is an industry standard for HTTP API error responses, improving interoperability and providing a consistent contract for API consumers.

---

## `IPaymentProcessor` Designed for Extensibility

**Context:**
The gateway currently integrates with a single acquiring bank.

**Decision:**
Implement `IPaymentProcessor` as a single-provider abstraction backed by `AcquiringBankProcessor`. A factory pattern was considered but not implemented to avoid over-engineering.

**Future Extension:**
A factory or strategy pattern could be introduced when multiple providers are required. The decision point would likely be driven by merchant-level configuration, where each merchant is associated with a preferred or pre-negotiated payment provider.

---

## Sensitive Data Handling

**Decision:**
Full card numbers and CVVs are never persisted. Only masked card details (last 4 digits) are stored in the payment record after processing.

**Rationale:**
Reduces PCI-DSS compliance scope and minimises the risk of sensitive data exposure in logs, databases, or error traces.

---

## In-Memory Data Store

**Decision:**
An in-memory repository is used in place of a persistent database.

**Rationale:**
Simplifies the implementation for this exercise. In production, this would be replaced with a durable storewith appropriate indexing on payment ID and merchant ID.





