# Identifying Situations for Data Masking and Obfuscation

> Worked answers to the two student tasks. Each requirement is mapped to a **technique**
> (masking vs obfuscation), a **sub-method**, and a **why**, then grounded in how it is
> actually implemented in production.

---

## Task 1: Financial Data Protection

### Scenario

A financial institution processes sensitive data, including customer bank account details and
transaction records. Account numbers must be concealed in customer-facing applications (showing only
the last four digits), transaction records must remain secure while stored in the database, and
transaction records must be protected during transmission between servers.

### Requirement 1 — Conceal account numbers, showing only the last four digits

- **Technique:** Data **masking** — *dynamic masking*.
- **Why it fits:** The viewer is a **legitimate user** (the account holder, or a support agent) who
  needs *just enough* to recognize the account — not the full number. Dynamic masking is applied at
  **display/query time**: the application returns `**** **** **** 6789` while the full account number
  stays intact in storage. Nothing is permanently altered; the mask is a *view*, not a
  transformation of the stored value.
- **Production notes:** Implemented at the presentation layer or via database **dynamic data
  masking** (e.g., SQL Server `MASKED WITH (FUNCTION = 'partial(0,"XXXX-XXXX-XXXX-",4)')`). It is
  **role-based** and must be paired with proper **access control / RBAC** — masking is a
  least-exposure control, not an authentication control. A privileged role (fraud investigation)
  may still be granted the unmasked value.

### Requirement 2 — Keep transaction records secure while stored in the database

- **Technique:** Data **obfuscation** — *tokenization* (in practice, combined with **encryption at rest**).
- **Why it fits:** This is **data at rest**. The goal is that anyone who reaches the database — a
  breach, a curious DBA, a stolen backup — sees values they **cannot interpret**. Tokenization
  replaces each sensitive value with a non-sensitive **token**; the real value lives in a separate,
  hardened **token vault**, so the records table holds nothing meaningful on its own.
- **Production notes:** Tokenization (often required for **PCI DSS** scope reduction) plus
  **AES-256 encryption at rest** (TDE / column encryption). Keys are held in a **KMS/HSM**, separate
  from the data. Unlike masking, this is **reversible** — but only for authorized services holding
  the vault mapping or the key.

### Requirement 3 — Protect transaction records during transmission between servers

- **Technique:** Data **obfuscation** — *scrambling / encryption in transit*.
- **Why it fits:** This is **data in transit**. The threat is **interception** (man-in-the-middle,
  packet sniffing) on the server-to-server hop. Obfuscating the payload — i.e., encrypting it —
  makes an intercepted stream **unreadable** without the key.
- **Production notes:** In practice this is **TLS 1.2/1.3** for the channel, and **mTLS** for
  service-to-service trust inside the institution. Sensitive fields may also be **encrypted at the
  message/application layer** so they stay protected even after TLS terminates at a gateway.

### Task 1 — summary

| Requirement | Technique | Sub-method | Core reason |
| ------------------------------------------- | --------------- | ----------------------- | ----------------------------------------- |
| Conceal account # (last four digits) | **Masking** | Dynamic masking | Legitimate viewer needs a partial, on-screen view |
| Secure transaction records in storage | **Obfuscation** | Tokenization (+ encryption at rest) | Data at rest must be unintelligible if breached |
| Protect transaction records in transmission | **Obfuscation** | Scrambling / encryption | Data in transit must survive interception |

---

## Task 2: Learning Management System (LMS)

### Scenario

An LMS handles sensitive information about students and instructors. Student grades must be hidden
from unauthorized users, instructor access logs must be anonymized for reporting purposes, and
personal student information (e.g., email addresses) must remain secure during transmission.

### Requirement 1 — Student grades hidden from unauthorized users

- **Technique:** Data **masking** — *dynamic masking* (role-based), backed by access control.
- **Why it fits:** Grades are **data in use** on a page that different roles can open. The student and
  their instructor must see the real grade; everyone else (other students, unprivileged staff) must
  **not**. Dynamic masking conceals the value at display time for unauthorized roles while revealing
  it to authorized ones — concealment driven by *who is looking*, not by transforming the stored grade.
- **Production notes:** Pair masking with **RBAC/ABAC** so the authorization decision drives the
  mask. Masking is the *display* safeguard; access control is the *gate*. (If the requirement were
  about protecting grades *in the database* rather than *on screen*, the answer would shift toward
  **obfuscation/encryption** — read the data state carefully.)

### Requirement 2 — Instructor access logs anonymized for reporting

- **Technique:** Data **masking** — *static masking* (anonymization).
- **Why it fits:** Reporting/analytics needs a dataset that is **statistically useful but not
  personally identifying**. Static masking produces a **separate, sanitized copy** in which real
  identifiers (instructor name, ID, IP) are replaced with **realistic-but-fictitious** values, so
  reports can be run and shared without exposing who did what. This mirrors the healthcare
  "anonymize research data" example exactly.
- **Production notes:** Apply once, to a copy, **irreversibly** — drop or hash identifiers, generalize
  timestamps, and consider **k-anonymity / pseudonymization** so individuals cannot be re-identified
  by correlation. The live log keeps the real data; only the reporting copy is masked.

### Requirement 3 — Personal student information (email) secure during transmission

- **Technique:** Data **obfuscation** — *scrambling / encryption in transit*.
- **Why it fits:** This is **data in transit** between client and server. Encrypting the channel
  makes intercepted traffic **unreadable**, protecting email addresses from sniffing or MITM.
- **Production notes:** **HTTPS/TLS 1.3** end-to-end (HSTS to force it). Email addresses are also
  **PII**, so encrypt them **at rest** as well — in transit protection alone is not enough for the
  full lifecycle.

### Task 2 — summary

| Requirement | Technique | Sub-method | Core reason |
| ------------------------------------- | --------------- | ----------------------- | ----------------------------------------------- |
| Hide student grades from unauthorized | **Masking** | Dynamic masking (+ RBAC) | Conceal on display based on *who is looking* |
| Anonymize instructor access logs | **Masking** | Static masking | Realistic-but-fake copy for safe reporting |
| Secure student email in transmission | **Obfuscation** | Scrambling / encryption | Data in transit must survive interception |

---

## Method recap

| Method | Sub-method | Trigger phrase | Data state |
| ----------- | ---------------- | ----------------------------------------- | ---------- |
| Masking | Dynamic | "show only part", "partial display", "hidden from / by role" | In use / on display |
| Masking | Static | "anonymize for research / reporting / dev-test" | A shared copy |
| Obfuscation | Tokenization | "store securely", "unreadable in the database" | At rest |
| Obfuscation | Scrambling/encryption | "secure during transmission", "between servers" | In transit |

**A note on terminology.** Coursera groups tokenization, scrambling, and encryption under the single
word *obfuscation*. In real-world security practice these storage/transit requirements are met by
genuine **encryption** (TLS in transit, AES at rest) and **tokenization** — strong, key-/vault-based
controls — rather than by reversible "scrambling," which on its own is weak. The course's
masking-vs-obfuscation split is a useful teaching frame; production systems combine **masking
(display), tokenization (storage), and encryption (storage + transit)** as layered defenses on the
*same* sensitive field.
