# Analyzing Full-Stack Integration in Industry

**Course 7 — Full-Stack Integration** · Module 1 · Lesson 4 · `You Try It!`

> Analyze how front-end and back-end components cooperate in real applications built with
> **Blazor**, **.NET Minimal APIs**, and **SQL Server**. Walk through a worked e-commerce
> case study, then independently map the integration and data flow for a social media platform.

---

## 🎯 Objective

Understand and analyze how front-end and back-end components work together in real-world
applications using technologies like **Blazor**, **.NET Minimal APIs**, and **SQL Server**.
This activity reinforces your knowledge by examining practical use cases and then creating a
similar scenario to analyze independently.

---

## 📦 What you will produce

A written **integration analysis** that, for each feature of an application, captures three things:

| Artifact                | What it answers                                                        |
| ----------------------- | --------------------------------------------------------------------- |
| Component interaction   | Which layer (Blazor / Minimal API / SQL Server) does what             |
| Data flow               | The ordered path a request takes, front-end → API → database → back   |
| Result                  | The observable outcome the user experiences                           |

**Flow:** `Blazor (HttpClient)  →  Minimal API endpoint  →  SQL Server  →  JSON response  →  Blazor render`

---

## 🧭 Walkthrough — Case Study: E-Commerce System Integration

You are tasked with analyzing the integration of an e-commerce platform's features — product
search, user login, and order placement. Here is how each component works together.

### 1. Product Search Integration

- **Front-End:** The user searches for a product through an input field created in Blazor. The
  search request triggers an API call using `HttpClient`.
- **Back-End:** The Minimal API processes the request and queries the SQL Server database to find
  matching products. The results are formatted as JSON and sent back to the front-end.

**Data flow:**

```text
Search query (front-end) → Minimal API (back-end)
SQL query execution → JSON response → Results rendered on the front-end
```

### 2. User Login Integration

- **Front-End:** A Blazor form collects login credentials and sends them via an API request to
  the server.
- **Back-End:** The Minimal API verifies the credentials using SQL Server. If successful, a
  session token is returned to the client for future authentication.

**Data flow:**

```text
Credentials input → API → Validation in SQL Server
Token generation → Front-end receives token for secure session management
```

### 3. Order Placement Integration

- **Front-End:** The user adds products to the cart and clicks **"Place Order."** The cart
  details are sent to the back-end.
- **Back-End:** The Minimal API updates the SQL Server database, creating an order entry and
  adjusting inventory. A confirmation is sent back.

**Data flow:**

```text
Cart data → API → Database update
Confirmation → Front-end displays order summary
```

### Why it works — the three roles

| Concern                       | How the stack handles it                                                                 |
| ----------------------------- | ---------------------------------------------------------------------------------------- |
| **Front-end tools (Blazor)**  | Blazor builds interactive UIs in C#; its `HttpClient` consumes APIs and integrates with the back-end |
| **Back-end tools (Minimal APIs)** | .NET Minimal APIs handle server-side logic and define endpoints; routing is simple and SQL Server integrates easily |
| **Data flow**                 | Data moves from front-end (via `HttpClient`) to back-end (via APIs), is processed in SQL Server, and returns as JSON for rendering |
| **API testing & debugging**   | Tools like **Postman** or **Swagger** test endpoints and confirm the data exchange works as expected |

---

## 🚀 Your turn — Analyze Integration for a Social Media Platform

**Scenario:** You are analyzing a social media platform where users:

1. Log in to their accounts.
2. Post a new status update.
3. View a feed of status updates.

**Instructions:**

- Map out how the front-end (Blazor) and back-end (Minimal API with SQL Server) interact for each
  functionality.
- Identify the data flow for each interaction.
- Reflect on how the components work together to deliver the functionality.

> *Stack to assume:* **Blazor WebAssembly** (front-end), **.NET Minimal APIs** (back-end),
> **SQL Server** (database).

---

## ✅ What good looks like — Integration Analysis: Social Media Platform

**Technologies:** Blazor WebAssembly (front-end), .NET Minimal APIs (back-end), SQL Server (database).

The platform supports three core features: secure authentication, posting status updates, and
viewing a personalized feed. The architecture ensures seamless communication, state management,
and efficient UI rendering.

### 1. User Login

**Purpose:** Grant users authorized access to the system's interactive features.

**Component interaction:**

| Layer        | Responsibility                                                                   |
| ------------ | -------------------------------------------------------------------------------- |
| Blazor (UI)  | Displays login form, collects credentials, initiates API request                 |
| Minimal API  | Validates user credentials in SQL Server, generates authentication token (e.g., JWT) |
| SQL Server   | Stores user records and password hashes                                          |

**Data flow:**

- The login form sends a `POST` request to `/api/auth/login` via `HttpClient`.
- The API queries SQL Server to verify credentials.
- If valid, the API returns a JSON response containing a token.
- The token is saved on the client side (local storage or session storage).

**Result:** The user gains access to protected endpoints such as posting and feed retrieval.

### 2. Posting a Status Update

**Purpose:** Allow an authenticated user to publish a new status message.

**Component interaction:**

| Layer        | Responsibility                                                        |
| ------------ | --------------------------------------------------------------------- |
| Blazor       | Captures post content, sends it along with the token in headers       |
| Minimal API  | Authenticates the token, inserts the post into SQL Server             |
| SQL Server   | Stores post content and metadata (`UserId`, timestamp)                |

**Data flow:**

- `POST /api/posts` with status data and an `Authorization` header.
- API inserts a new record into the `Posts` table.
- Confirmation returned: post ID and timestamp.
- Blazor re-renders the UI to show the new post immediately.

**Result:** The user sees their newly created post in the feed.

### 3. Viewing the Feed

**Purpose:** Display the latest content posted by the user and their connections.

**Component interaction:**

| Layer        | Responsibility                                                        |
| ------------ | --------------------------------------------------------------------- |
| Blazor       | Requests feed data, binds JSON to UI components                       |
| Minimal API  | Authenticates the request, retrieves filtered posts                   |
| SQL Server   | Executes `SELECT` statements with `JOIN`s (posts, users, friends)     |

**Data flow:**

- `GET /api/feed` with a valid token.
- API identifies the user and fetches personalized feed data.
- SQL Server returns a post list including author info and timestamps.
- JSON response is returned to Blazor.
- Blazor renders the feed, using pagination or virtualization for performance.

**Result:** The user receives an updated, personalized content stream.

### Key integration considerations

| Concern                          | Description                                                  |
| -------------------------------- | ----------------------------------------------------------- |
| Authentication & Authorization   | Token validated for each protected endpoint                 |
| Serialization                    | JSON provides a lightweight data exchange format            |
| State Management                 | Token stored and reused for authenticated API requests      |
| Performance                      | Pagination, caching, and efficient SQL queries              |
| Testing Tools                    | Swagger and Postman ensure endpoint correctness and debugging |

**Final reflection:** The presentation, service, and data layers function cohesively. Blazor
delivers rich UI interactivity and triggers secure API requests; .NET Minimal APIs manage
business logic, routing, and security efficiently; SQL Server ensures reliable data persistence
and relational data modeling. This coordinated architecture enables core social networking
features: secure login, content creation, and real-time feed rendering.

---

## ☑️ Definition of done

- [ ] Walked through all three e-commerce flows (search, login, order) and named the role of each layer
- [ ] Mapped Blazor ↔ Minimal API ↔ SQL Server interaction for **login**, **posting**, and **feed**
- [ ] Wrote an ordered data flow for each social-media feature, including endpoint and token handling
- [ ] Listed cross-cutting concerns (auth, serialization, state, performance, testing)
- [ ] Closed with a reflection on how the three layers cooperate to deliver the features

---

## 🔑 Key concepts

- **Three-layer separation** — Blazor (presentation) consumes APIs via `HttpClient`, Minimal APIs
  (service/business logic) own routing and security, and SQL Server (data) persists state; each
  layer has one clear responsibility.
- **Request/response data flow** — every feature follows the same shape: front-end request → API
  endpoint → SQL query → JSON response → UI render, which makes integrations predictable to reason about.
- **Token-based state management** — a token issued at login is stored client-side and replayed in
  the `Authorization` header so the stateless API can authorize each protected request.
- **JSON as the contract** — JSON is the lightweight serialization format that lets the front-end
  and back-end exchange data independently of their internal implementations.
- **Verify the seams** — Swagger and Postman test the endpoints in isolation, confirming the
  contract between front-end and back-end holds before the UI consumes it.
