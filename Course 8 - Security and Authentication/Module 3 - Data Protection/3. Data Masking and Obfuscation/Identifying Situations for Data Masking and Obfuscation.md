# Identifying Situations for Data Masking and Obfuscation

**Course 8 — Security and Authentication** · Module 3 · Lesson 3 · `You Try It!`

> Decide, for each requirement, whether to reach for **data masking** or **data
> obfuscation**. The pivot is simple: *masking* limits what a **legitimate viewer**
> sees (a partial or realistic-but-fake version), while *obfuscation* makes data
> **unintelligible to anyone unauthorized** — both where it is **stored** and while
> it is **transmitted**.

---

## 🎯 Objective

Analyze scenarios where sensitive data must be protected and determine **when to use
data masking** and **when to use data obfuscation**, based on the context and the
requirement.

---

## 📦 What you will produce

For each requirement, a short decision with three parts:

| Part | What you answer |
| ------------- | -------------------------------------------------------------- |
| **Technique** | Masking or obfuscation |
| **Sub-method** | Dynamic / static masking · tokenization · scrambling / encryption |
| **Why** | What the requirement protects, and why this method fits |

**Lens:** `Requirement → Data state (in use / at rest / in transit) → Masking vs Obfuscation → Technique → Why`

---

## 🧭 Masking vs Obfuscation — quick reference

| Dimension | 🎭 Data Masking | 🔒 Data Obfuscation |
| ---------------------- | --------------------------------------------- | --------------------------------------------------- |
| **Goal** | Limit what a *legitimate* viewer sees | Make data *unintelligible* to the unauthorized |
| **Where it applies** | Data **in use** / on display, or a shared dataset | Data **at rest** (storage) and **in transit** |
| **Protects against** | Over-exposure to authorized-but-limited users | Attackers who reach the storage or the traffic |
| **Common techniques** | Dynamic masking, static masking | Tokenization, scrambling, encryption |
| **Reversibility** | Dynamic: the rest is simply never shown · Static: copy is permanent | Reversible with a vault (tokenization) or key (encryption) |

### Decision cheat-sheet

| If the requirement says… | Use | Technique |
| ----------------------------------------------------------------- | --------------- | ----------------------- |
| "show only the last four digits / partial display / conceal on screen" | **Masking** | Dynamic masking |
| "anonymize for research / reporting / dev-test, keep it realistic" | **Masking** | Static masking |
| "store securely / keep unreadable in the database" | **Obfuscation** | Tokenization |
| "protect during transmission / between client & server / between servers" | **Obfuscation** | Scrambling / encryption |

---

## 🔍 Walkthrough — worked examples

Two solved scenarios that model the expected reasoning.

### Example 1 — E-Commerce Platform

**Scenario.** An e-commerce platform collects and processes customer payment details. Payment
information must be stored securely, customer service should see only the last four digits of a
card, and data must stay secure in transit between client and server.

| Requirement | Technique | Why |
| ----------------------------------------- | ------------------------------- | --------------------------------------------------------------------- |
| Secure storage of payment data in the DB | **Obfuscation** — tokenization | Even if the database is breached, tokens are meaningless without the vault. |
| Customer service sees only the last four digits | **Masking** — dynamic | Enough to verify the customer without exposing the full card number. |
| Secure data in transit (client ↔ server) | **Obfuscation** — scrambling/encryption | Transforms the payload so an intercepted stream is unreadable. |

### Example 2 — Healthcare System

**Scenario.** A hospital system handles patient data, including medical records and Social Security
numbers. Research data must be anonymized, SSNs must be partially hidden in portals, and medical
records must stay secure in the database.

| Requirement | Technique | Why |
| ------------------------------------------- | ------------------------------ | --------------------------------------------------------------------- |
| Anonymize patient data for research | **Masking** — static | Replaces real values with realistic-but-fictitious data, keeping the set useful for research. |
| Partially hide SSNs in portals (`XXX-XX-6789`) | **Masking** — dynamic | Allows limited verification while concealing the full SSN. |
| Secure storage of medical records | **Obfuscation** — tokenization | Records stay unintelligible to anyone who reaches the database. |

---

## ✍️ Your turn

Apply the same decision (technique → sub-method → why) to each requirement below.

### Task 1 — Financial data protection

**Scenario.** A financial institution processes sensitive data, including customer bank account
details and transaction records.

Requirements:

- Account numbers must be **concealed in customer-facing apps**, showing only the last four digits.
- Transaction records must remain **secure while stored** in the database.
- Transaction records must be **protected during transmission** between servers.

### Task 2 — Learning Management System (LMS)

**Scenario.** An LMS handles sensitive information about students and instructors.

Requirements:

- Student grades must be **hidden from unauthorized users**.
- Instructor access logs must be **anonymized for reporting** purposes.
- Personal student information (e.g., email addresses) must remain **secure during transmission**.

---

## ▶️ What good looks like

A strong answer names the **technique** (masking vs obfuscation) *and* the **sub-method**, and
justifies it by *where the data lives* and *who must see it*. The tell-tales: "show only part on
screen" → **dynamic masking**; "anonymize a dataset for research/reporting" → **static masking**;
"secure in storage" → **tokenization (obfuscation)**; "secure in transit" →
**scrambling/encryption (obfuscation)**.

---

## ☑️ Definition of done

- [ ] Every requirement is labelled **masking** or **obfuscation**, with a named sub-method
- [ ] Each choice is justified by data state (in use / at rest / in transit) and audience
- [ ] Task 1 — account-number display → **dynamic masking**; storage → **tokenization**; transmission → **scrambling/encryption**
- [ ] Task 2 — grades hidden by role → **dynamic masking**; anonymized logs → **static masking**; email in transit → **obfuscation (encryption)**
- [ ] Where the course says "obfuscation" for storage/transit, you note it is realized in practice as **tokenization and encryption**

---

## 🔑 Key concepts

- **The pivot is audience + data state** — masking serves a *legitimate viewer* who should see less;
  obfuscation defends data *at rest and in transit* from the unauthorized.
- **Two faces of masking** — *dynamic* masking partially reveals at display time (last four digits);
  *static* masking bakes realistic-but-fake values into a copy for research, reporting, or dev/test.
- **Two faces of obfuscation** — *tokenization* protects data at rest (swap the value for a token
  backed by a secure vault); *scrambling/encryption* protects data in transit.
- **"Obfuscation" here ≈ encryption + tokenization** — the course uses *obfuscation* broadly; in
  production these storage/transit requirements are met by real **encryption** (TLS in transit,
  AES at rest) and **tokenization**, not by reversible "scrambling" alone.
- **Match the method to the requirement, not the data type** — the *same* card number is masked for a
  support agent, tokenized in the database, and encrypted on the wire; one field, three techniques,
  three different goals.
