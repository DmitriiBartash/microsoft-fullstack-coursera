# Planning Security for API Endpoints with JWT Validation

**Course 8 — Security and Authentication** · Module 2 · Lesson 5 · `You Try It!`

> Plan **endpoint-level security** for a REST API: decide which routes require JWT
> validation, then which **roles** and **claims** gate each one. You'll study two worked
> examples (healthcare, e-commerce), then plan two APIs of your own — always separating
> **authentication** (*who you are*) from **authorization** (*what you may do*).

---

## 🎯 Objective

Plan security for API endpoints by **identifying routes that require JWT validation** and
**defining which roles or claims are needed** for access. Practice reasoning about each route
as a separate decision: is it public or protected, which roles may call it, and does access
have to be narrowed to a *specific record* the caller owns.

---

## 📦 What you'll produce

For each API, a short security plan built on **three axes**:

| Deliverable             | What it contains                                                                       |
| ----------------------- | ------------------------------------------------------------------------------------- |
| **Route classification** | For every route: does it require JWT validation (protected), or is it public?         |
| **Role & claim mapping** | The roles allowed on each protected route, plus any claim / ownership check that scopes access to specific records |
| **Security plan table**  | A summary mapping every route to **JWT validation · Roles · Claims**                  |

Read the three axes as a funnel:

```
Request ──▶ [ 1. JWT valid? ] ──▶ [ 2. Right role? ] ──▶ [ 3. Owns this record? ] ──▶ allow
              authentication          authorization           resource-based authz
```

- **Axis 1 — JWT validation (authentication).** Is a valid, unexpired, correctly-signed token
  required at all? Public routes skip this; protected routes reject anonymous callers.
- **Axis 2 — Roles (coarse authorization).** Which role(s) may reach this route — `doctor`,
  `admin`, `customer`? A role grants access to a *type* of operation.
- **Axis 3 — Claims (fine authorization).** Even with the right role, can the caller touch
  *this specific record*? A claim (typically the caller's own user/account ID in the token)
  is compared against the resource's owner so users only act on data that is theirs.

---

## 🧭 Walkthrough

Two fully worked examples show the method before you try it yourself. For each route, follow
the same three steps: **(1)** decide if JWT validation is required, **(2)** specify roles and
claims, **(3)** explain why — then summarize in a table.

### Example 1 — Medical Records System

**Scenario.** A medical company manages patient data through an API with three key routes:

- `GET /patients/{id}` — returns patient information.
- `PUT /patients/{id}` — updates patient records.
- `GET /reports` — provides access to system-wide reports.

**Step 1 — Which routes require JWT validation?**

- `GET /patients/{id}` — returns **sensitive** patient information, so only authenticated
  users may access it. JWT validation proves the requester is legitimate.
- `PUT /patients/{id}` — updates sensitive records; must be restricted to authenticated users
  with a specific role (e.g., doctors).
- `GET /reports` — exposes system-wide data relevant only to administrative roles. JWT
  validation keeps unauthorized users out.

> **All three routes require JWT validation** — none of this data is public.

**Step 2 — Specify roles and claims.**

- `GET /patients/{id}` — Roles: `doctor`, `nurse`. Claim: the caller must be authorized for
  the specific patient `{id}` (their identity/assignment is checked against the record).
- `PUT /patients/{id}` — Roles: `doctor`. Claim: the caller must be authorized for the
  specific patient `{id}` they are updating.
- `GET /reports` — Roles: `manager`, `admin`. Claims: none beyond the role.

**Step 3 — Summarize the plan.**

| Route               | JWT validation | Roles            | Claims                                  |
| ------------------- | -------------- | ---------------- | --------------------------------------- |
| `GET /patients/{id}` | Required       | `doctor`, `nurse` | Must be authorized for the patient `{id}`. |
| `PUT /patients/{id}` | Required       | `doctor`         | Must be authorized for the patient `{id}`. |
| `GET /reports`       | Required       | `manager`, `admin` | None.                                  |

**Why these restrictions are necessary.** JWT validation ensures only authenticated users
reach sensitive routes. Restricting roles reduces the risk of unauthorized actions — nurses
may *view* patient data but not *modify* it, while managers see only administrative reports.
Claims add a further layer: users interact only with data they're authorized for (the
patient-specific record), not every record the route can serve.

### Example 2 — E-Commerce Platform

**Scenario.** An e-commerce platform exposes three routes:

- `GET /products` — lists all available products.
- `POST /orders` — places a new order.
- `GET /orders/{id}` — returns order details for a specific order.

**Step 1 — Which routes require JWT validation?**

- `GET /products` — **public**, so JWT validation is **not** required.
- `POST /orders` — places an order; requires authentication to ensure the user is legitimate.
- `GET /orders/{id}` — returns sensitive order details; restricted to authenticated users.

**Step 2 — Specify roles and claims.**

- `GET /products` — no JWT validation needed; no roles, no claims.
- `POST /orders` — Roles: `customer`. Claim: the caller's user ID, so the order is associated
  with the correct account.
- `GET /orders/{id}` — Roles: `customer`. Claim: the caller may only read an order whose owner
  matches their user ID — they see *their own* orders only.

**Step 3 — Summarize the plan.**

| Route             | JWT validation | Roles      | Claims                                    |
| ----------------- | -------------- | ---------- | ----------------------------------------- |
| `GET /products`    | Not required   | None       | None.                                     |
| `POST /orders`     | Required       | `customer` | Must include the user ID (sets the owner). |
| `GET /orders/{id}` | Required       | `customer` | Order's owner must match the caller's ID. |

**Why these restrictions are necessary.** Public access to `GET /products` lets anyone browse
without friction. Authentication on `POST /orders` ensures only legitimate users place orders,
and binding the order to a user ID prevents fraud. On `GET /orders/{id}`, JWT validation plus
the ownership claim ensure users can only view their own orders, protecting sensitive
information.

---

## ✍️ Your turn

Apply the same three-step method (JWT? → roles & claims → table) to plan security for the
following APIs. For each route, identify whether JWT validation is required, specify the roles
or claims needed for access, and summarize your plan in a table.

### Task 1 — Social Media Platform

A social media platform exposes the following API routes:

- `GET /users/{id}` — returns profile information for a specific user.
- `POST /posts` — creates a new post.
- `DELETE /posts/{id}` — deletes a specific post.

### Task 2 — Learning Management System (LMS)

An LMS exposes the following API routes:

- `GET /courses` — lists all available courses.
- `POST /assignments` — creates a new assignment.
- `GET /grades/{id}` — returns grades for a specific student.

---

## 🌟 What good looks like

- **Every route is classified first.** Decide *public vs protected* before anything else; a
  route that needs no JWT (a public catalog) is a deliberate choice, not an oversight.
- **Authentication and authorization stay separate.** JWT validation answers *who are you*;
  roles and claims answer *what may you do*. A valid token alone is never enough for a
  protected, role-scoped route.
- **Roles gate the operation; claims gate the record.** A role lets you reach a *type* of
  endpoint; an ownership claim narrows you to *your own* record (your order, your post, your
  grades) — the two work together.
- **Write operations are tighter than reads.** Viewing may be open to several roles, but
  creating, updating, or deleting is restricted to the owner or an elevated role (a nurse
  reads but a doctor writes; an author or moderator deletes).
- **Self-access is scoped to ownership.** Where a route serves one record, end users reach
  *only theirs* — compare a claim (the caller's ID) against the resource owner, and deny by
  default on mismatch.
- **The table is the deliverable.** A clean *route → JWT · Roles · Claims* table makes the
  security posture reviewable at a glance and easy to translate into code.

---

## 🔧 From plan to code (ASP.NET Core)

The plan maps directly onto the framework once the JWT bearer pipeline is configured:

| Plan element                | ASP.NET Core enforcement                                                    |
| --------------------------- | -------------------------------------------------------------------------- |
| Public route (no JWT)       | `[AllowAnonymous]`                                                          |
| JWT required (any role)     | `[Authorize]`                                                               |
| JWT required + specific roles | `[Authorize(Roles = "doctor,nurse")]` or a role policy                    |
| Claim / ownership check     | Compare `User.FindFirst(ClaimTypes.NameIdentifier)` to the record's owner, or a resource-based `IAuthorizationHandler` |

The role gate (axis 2) is **declarative** (an attribute). The ownership check (axis 3) is
usually **imperative or resource-based**, because the framework can't know who owns record
`{id}` until it loads it.

---

## ☑️ Definition of done

- [ ] Every route classified as **JWT-required** or **public**, with a reason
- [ ] Roles specified for each protected route
- [ ] Claim / ownership checks specified wherever a route serves a **specific record**
- [ ] A **table** summarizes each API as *route → JWT · Roles · Claims*
- [ ] Write/delete routes are at least as restricted as the matching read routes
- [ ] Public routes are a **deliberate** choice (no sensitive data is exposed anonymously)

---

## 🔑 Key concepts

- **Authentication vs authorization** — JWT validation authenticates (*who you are*); roles
  and claims authorize (*what you may do*). They are separate gates and you need both on a
  protected route.
- **JWT validation** — the API verifies the token's signature, expiry, issuer, and audience
  before trusting any identity inside it; an invalid token is rejected at the door.
- **Roles (coarse-grained)** — a role grants access to a *type* of operation (`doctor`,
  `admin`, `customer`); ideal for the first authorization decision on a route.
- **Claims & resource-based authorization (fine-grained)** — a claim (often the caller's own
  ID) is checked against a specific record's owner so users act only on data that is theirs.
- **Least privilege & deny-by-default** — grant the narrowest access each route needs and
  reject anything not explicitly allowed; tighten write/delete beyond read.
