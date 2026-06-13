# JWT Structure Analysis

**Course 8 — Security and Authentication** · Module 2 · Lesson 2: *JWT Structure*

> A JSON Web Token has three Base64URL-encoded parts separated by dots:
> `header.payload.signature`.
> The header and payload are just Base64URL → JSON (anyone can read them).
> The signature is a cryptographic MAC — it cannot be "read", only **verified**.

---

## 📋 Assignment

### Task 1 — JWT for an E-Commerce System

**Scenario:** A token issued to an e-commerce user. It carries the user's ID, role,
and an expiration that should limit the token's validity to **24 hours**.

```text
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
eyJzdWIiOiI1Njc4OTAiLCJyb2xlIjoidXNlciIsImV4cCI6MTY5MjIzMDAwfQ.
Hx73oTzVZj5lfZlyyRcAJo3hH9M3VBB9-LD9ACBRUjk
```

**To do:** identify the signing algorithm (Header); extract the user's ID, role and
expiration (Payload); explain how the Signature protects the token; summarize in a table.

### Task 2 — JWT for a Healthcare Application

**Scenario:** A token issued to a doctor. It carries the user's ID, role, and an
expiration that should limit the token's validity to **12 hours**.

```text
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
eyJzdWIiOiIxMjM0NTUwIiwicm9sZSI6ImRvY3RvciIsImV4cCI6MTY4MjQ2NjAwMH0.
dsg5KlRsdpQZn1uvKjMf2M3Kw8E3ljUlShVlxVc43F4
```

**To do:** identify the algorithm (Header); extract the user's ID, role and expiration
(Payload); explain how the Signature secures the token; summarize in a table.

---

## ✅ Solution

### Task 1 — E-Commerce

**1. Header** &nbsp;`eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9`

```json
{ "alg": "HS256", "typ": "JWT" }
```

- `alg = HS256` → the token is signed with **HMAC-SHA256** (symmetric: one shared secret signs *and* verifies).
- `typ = JWT` → declares the token type so the server knows how to process it.

**2. Payload** &nbsp;`eyJzdWIiOiI1Njc4OTAiLCJyb2xlIjoidXNlciIsImV4cCI6MTY5MjIzMDAwfQ`

```json
{ "sub": "567890", "role": "user", "exp": 169223000 }
```

| Claim  | Value       | Meaning                                   |
| ------ | ----------- | ----------------------------------------- |
| `sub`  | `567890`    | Subject — the user's unique ID            |
| `role` | `user`      | Authorization role (standard customer)    |
| `exp`  | `169223000` | Expiration time, in Unix epoch seconds    |

> ⚠️ **Data note (reported as decoded).** `169223000` is only **9 digits**, which decodes to
> **1975-05-13 14:23:20 UTC** — a date in the past. A present-day Unix timestamp has **10 digits**
> (~1.7 billion). The value looks truncated by one digit; the intended value was almost certainly
> `1692230000` → **2023-08-16 23:53:20 UTC**, i.e. ~24 h after a 2023-08-16 issuance, matching the
> scenario. **As written, a conforming server would treat this token as already expired.**

**3. Signature** &nbsp;`Hx73oTzVZj5lfZlyyRcAJo3hH9M3VBB9-LD9ACBRUjk`

- Computed as `HMAC-SHA256( base64url(header) + "." + base64url(payload), secret )`.
- 43 Base64URL characters = **32 bytes = a 256-bit MAC** → consistent with the `HS256` header.
- It is **not decodable** into readable text. To verify, the server recomputes the HMAC with its own
  secret and compares. Change the header or payload by even one bit (e.g. `role` → `admin`) and the
  recomputed signature no longer matches, so the token is **rejected**.

**Summary**

| Component  | Decoded content                                       | Explanation                                                        |
| ---------- | ----------------------------------------------------- | ------------------------------------------------------------------ |
| Header     | `{"alg":"HS256","typ":"JWT"}`                         | Signed with HMAC-SHA256; token type JWT.                           |
| Payload    | `{"sub":"567890","role":"user","exp":169223000}`      | User ID `567890`, role `user`, expiry (value truncated — see note).|
| Signature  | `Hx73oTzVZj5lfZlyyRcAJo3hH9M3VBB9-LD9ACBRUjk`         | 256-bit HMAC; guarantees integrity — any tampering invalidates it. |

---

### Task 2 — Healthcare

**1. Header** &nbsp;`eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9`

```json
{ "alg": "HS256", "typ": "JWT" }
```

- Identical to Task 1: signed with **HMAC-SHA256**, type `JWT`.

**2. Payload** &nbsp;`eyJzdWIiOiIxMjM0NTUwIiwicm9sZSI6ImRvY3RvciIsImV4cCI6MTY4MjQ2NjAwMH0`

```json
{ "sub": "1234550", "role": "doctor", "exp": 1682466000 }
```

| Claim  | Value        | Meaning                                                   |
| ------ | ------------ | --------------------------------------------------------- |
| `sub`  | `1234550`    | Subject — the doctor's unique ID                          |
| `role` | `doctor`     | Authorization role (grants clinical/PHI access)           |
| `exp`  | `1682466000` | Expiration = **2023-04-25 23:40:00 UTC** (valid 10-digit) |

> The `exp` claim caps how long the token is accepted. After this instant the server rejects it,
> forcing re-authentication — this is what limits exposure if a token is stolen. In healthcare,
> a short window (here 12 h) is important because the `doctor` role can reach protected health information.

**3. Signature** &nbsp;`dsg5KlRsdpQZn1uvKjMf2M3Kw8E3ljUlShVlxVc43F4`

- Same construction: `HMAC-SHA256( base64url(header) + "." + base64url(payload), secret )`.
- 43 Base64URL characters = **32 bytes = 256-bit** MAC → consistent with `HS256`.
- Protects integrity and authenticity: only a party holding the secret can produce a matching
  signature, so an attacker cannot silently change `role` or `exp` and still pass verification.

**Summary**

| Component  | Decoded content                                          | Explanation                                                       |
| ---------- | -------------------------------------------------------- | ----------------------------------------------------------------- |
| Header     | `{"alg":"HS256","typ":"JWT"}`                            | Signed with HMAC-SHA256; token type JWT.                          |
| Payload    | `{"sub":"1234550","role":"doctor","exp":1682466000}`     | Doctor ID `1234550`, role `doctor`, expires 2023-04-25 23:40 UTC. |
| Signature  | `dsg5KlRsdpQZn1uvKjMf2M3Kw8E3ljUlShVlxVc43F4`            | 256-bit HMAC; ensures the token has not been tampered with.       |

---

## 🔑 Key takeaways

- **Structure:** `header.payload.signature` — three dot-separated, Base64URL-encoded parts.
- **Encoded ≠ encrypted:** header and payload are readable by anyone, so **never put secrets in a JWT**.
- **Signature = integrity + authenticity, not confidentiality:** it proves the token wasn't altered
  and was issued by the secret holder — it does **not** hide the contents.
- **HS256 is symmetric:** the same secret signs and verifies. If that secret leaks, anyone can forge
  valid tokens — protect it like a password.
- **Always validate `exp` server-side:** reject expired (and implausibly dated) tokens, as Task 1 shows.

---

<sub>Decoding method: each part is Base64URL-decoded (`-`→`+`, `_`→`/`, re-padded) to UTF-8 JSON;
`exp` is read as Unix epoch seconds. Verified with the `base64` and `date` utilities. The signature
is an opaque HMAC-SHA256 value and is shown encoded, as it is meant to be verified, not decoded.</sub>
