# Transactions Dispute Portal

![CI](https://github.com/USER/REPO/actions/workflows/ci.yml/badge.svg)

A bank customer portal that lets customers view their transactions, raise disputes,
and track each dispute through its full status history. Back-office agents/referred to as just 'agents' move
disputes from submitted, through review, to resolution.

**Stack:** .NET 10 Web API · PostgreSQL · EF Core · Angular · Docker · JWT auth

## Architecture

The architecture is a clean-architecture derived design:

- **Domain** — entities, the dispute state machine, business rules only. This project references no other, internal or otherwise.
- **Infrastructure** — EF Core DbContext, mappings, migrations.
- **Api** — controllers, auth, DTOs. Composition root.
- **Tests** — unit tests are domain specifir with no DB interaction. Integration tests are full stack

See [DECISIONS.md](./DECISIONS.md) for additional context around key decision made through out this project.

## Running it

Requires Docker.

```bash
docker compose up --build
```

This starts three services:

- **PostgreSQL** — the database
- **API** (.NET) — applies migrations and seeds demo data on startup; at http://localhost:5290
- **Web** (Angular, served by nginx) — the UI; at **http://localhost:4200**

Open **http://localhost:4200** and log in with a seeded user below.

### Seeded users

| Email             | Password       | Role     |
|-------------------|----------------|----------|
| joe@email.com     | Password123!   | Customer |
| agent@abcbank.com | Password123!   | Agent    |

The customer UI covers the full flow: view transactions, raise a dispute,
withdraw it, and view its status history. The agent actions are only available via the API
(see below).

### Exercising agent actions (no agent UI, by design)

Agent operations are role-gated API endpoints. Log in as the agent for a token,
then drive a dispute through its lifecycle:

```bash
# get an agent token
curl -X POST http://localhost:5290/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"agent@abcbank.com","password":"Password123!"}'

# move a dispute: review -> resolved
curl -X POST http://localhost:5290/api/disputes/<id>/review  -H "Authorization: Bearer <agent-token>" -H "Content-Type: application/json" -d '{}'
curl -X POST http://localhost:5290/api/disputes/<id>/resolve -H "Authorization: Bearer <agent-token>" -H "Content-Type: application/json" -d '{}'
```

## Development (without Docker)

- **API:** `dotnet run --project src/DisputePortal.Api` (Postgres is required, you `docker compose up db`)
- **Web:** `cd web && npm install && ng serve`(there is a proxy `/api` to the .net web API via `proxy.conf.json`)

## Testing

```bash
dotnet test
```
This runs the unit tests and integration tests. The integration tests are full stack and spin up real PostgreSQL test container. Docker must be running to achieve a successful test run.

## Security highlights

- **Insecure Direct Object Reference prevention:** The user identity is read only from the signed JWT (`sub` claim),
  It is never passed from request parameters.
- A non-owner requesting another user's dispute receives **404, not 403** — To reduce the malicious attack surface the API does not confirm another user's records exist.
- Role-gated endpoints are enforced from token claims. Currently there are 2 roles ccustomer vs agent.
- Passwords hashed with PBKDF2; to further reduce the attack surface, login returns a uniform error to prevent user enumeration and simulation.
- CI runs a CodeQL static-analysis (SAST) scan on every push.

## Project layout

    src/DisputePortal.Domain          domain model + state machine
    src/DisputePortal.Infrastructure  EF Core persistence
    src/DisputePortal.Api             Web API
    web/                              Angular frontend
    tests/                            unit + integration tests

## Known limitations / next steps

- Agent actions are exposed via API only, there is no back-office UI in this scope.
- No refresh-token rotation; short-lived access tokens only.
- `User` model/class currently serves both auth identity and account ownership; would split `User` / `Customer`if the where requirements to do so.