# Architecture & Decisions

This document illustrates significant decision decisions, and trade offs.
It meant to answer the question "Why did you do it in that way?", and give glimpse to my thinking for things I deliberately left out.

## Architecture

Clean-Architecture based architecture, with a vertically layered stack flow.
This means dependnices are from the most other project inwards. The Domain Project is the core, and base for all other projects. Infrastrure references Domain, API references Infrastructure. Domain references no other project, internal or otherwise.

## Domain design

- **Strong Idempotiancy** Entities use private setters and constructors,
  so an object can't exist in an invalid state. `Dispute.Status` changes only through
  named methods (Withdraw / Resolve / Reject / MoveToUnderReview) that funnel into one
  private guarded `Transition`.

- **State machine as a whitelist.** The system has a `HashSet` of allowed action to role relationship in the form of (from, to, role) tuple matrix. A move is permitted only if present; everything else is rejected. Terminal states have no outgoing entries. This method of whitelisting role based actions was selected due to the perfomance benefits for a project of this size.

- **Audit trail inside the aggregate.** Every transition appends a `DisputeStatusHistory`row, and thus the history cannot be tempered with.

## Security (primary focus)

- **Insecure Directed Object Reference(IDOR) prevention.** The current user's userId is not passed via route or body parameters, and comes from the JWT claim. never from a route or body parameter.

- **404, not 403, for non-owners.** To reduce the attack surface of the system, attempting to retrieve another user's un authorised information(Transactions, Dispute, Dispute history, etc). This will prevent information of the existence of user data, unlike a 403 would leak existence.

- **Roles: API enforces, domain assumes.** Transition methods hardcode the acting role;
  the API gates endpoints with `[Authorize(Roles=...)]` read from the token. Trade-off:
  the domain has no independent role backstop.

- **Passwords** hashed with `PasswordHasher<T>` (PBKDF2, per-password salt). Login
  returns a uniform "invalid credentials" for both unknown email and wrong password,
  to reduce the attack surface of the system.

- **JWT is signed, not encrypted** The claims are signed and contain non sensitive information, Each token has a Short expiry. The dev signing key is in config for clone-and-run; in production it would
  come from an environment variable / key vault. *Next step:* refresh-token rotation.

## Persistence

- **Enums stored as strings** this makes it readable in the DB and safe if the enum is reordered.
- **Partial unique index** enforces "one active dispute per transaction" at the
  database. This is the last line of defence and is atomic — it catches a race a
  pre-check in code could miss. Violations surface as **409 Conflict**.
- **`DateTimeOffset` (UTC) -> timestampts ** we recommenrd mestamps are UTC.

## API

- **Domain exceptions map to HTTP status by meaning.** An illegal transition or a
  duplicate active dispute is a *conflict with current state*, both return **409**,
  not 400, the request is well-formed.
- **DTOs are a gateway to communication** controllers never return entitie direcly, so the API contract is
  independent of the persistence model.

## Testing

- **Unit tests** The domain has no reference to any other library or entity(dtabase, API, etc), Unit tests cover the domain (state machine, invariants) efficiently as a result.
- **One integration test** drives real HTTP through the full stack against a real
  PostgreSQL Testcontainers, proving auth, EF, migrations, and seeding wire
  together.

## Deliberately out of scope

- **Agent/back-office UI** agent operations (review/resolve/reject) are fully
  implemented and role-gated at the API, and demonstrable via curl. A back-office UI
  is the first thing I'd add next; left out to keep one complete, polished
  customer-facing slice.
- Real notifications; multi-account users; refresh tokens.