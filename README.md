# dotnet-microservices-sample

A small, sanitised .NET 8 sample showing how I structure a workflow microservice:
clean API design, validation, health checks, and in-memory storage for local demo use.

This is sample code only. It contains no client code, no client data, and no
real workflow rules. All case data below is synthetic.

## What it shows

- Minimal API in .NET 8 with clear endpoints
- A simple domain model for a case workflow: Draft → Submitted → Approved / Rejected
- Validation and consistent error responses
- Health endpoint for container / load-balancer checks
- Separation of API, domain, and storage, so a real database can replace the in-memory store

## Run it

```bash
dotnet run --project src/WorkflowApi
```

Then open:

- `GET /health` — service health
- `GET /cases` — list synthetic cases
- `POST /cases` — create a case, e.g. `{"title": "Sample case", "owner": "deepthi"}`
- `POST /cases/{id}/submit` — move a case from Draft to Submitted
- `POST /cases/{id}/approve` — approve a Submitted case

## Architecture notes

- Keep the API thin: validation and state transitions live in the domain, not in controllers.
- Every state change is explicit, so an audit trail and form versioning can be added later.
- For a regulated system, add authentication, authorisation by role, and an audit log
  before using this pattern in production — none of that is in this demo.
