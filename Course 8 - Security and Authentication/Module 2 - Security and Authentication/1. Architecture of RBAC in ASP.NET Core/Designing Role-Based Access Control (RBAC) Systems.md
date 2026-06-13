# Designing Role-Based Access Control (RBAC) Systems

**Course 8 — Security and Authentication** · Module 2 · Lesson 1 · `You Try It!`

> Design the **architecture** of Role-Based Access Control for real-world applications:
> define roles, assign permissions, and enforce secure access. You'll study two worked
> examples (healthcare, e-commerce), then design RBAC systems of your own — always
> applying the **principle of least privilege**.

---

## 🎯 Objective

Develop and articulate an RBAC architecture for real-world applications by defining roles,
assigning permissions, and ensuring secure access control. Gain practice in **analyzing
system requirements** and creating RBAC designs tailored to specific scenarios.

---

## 📦 What you'll produce

For each scenario, a short RBAC design consisting of:

| Deliverable          | What it contains                                                        |
| -------------------- | ----------------------------------------------------------------------- |
| **Role definitions** | Each role and the responsibility it covers in the workflow              |
| **Permission set**   | The permissions granted to each role, with a justification for each     |
| **Role/permission table** | A summary mapping every role to its permissions                    |

Each design is judged against the **principle of least privilege**: a role gets only the
access its job requires, and nothing more.

---

## 🧭 Walkthrough

Two fully worked examples show the method before you try it yourself.

### Example 1 — Healthcare Management System

**Scenario.** A healthcare system must securely manage access for a **System Admin**
(manages the system, users, and settings), a **Doctor** (manages care for assigned
patients — prescribing, viewing records), a **Nurse** (updates patient vitals but cannot
see prescriptions or test results), and a **Patient** (views only their own records and
test results).

**Step 1 — Identify roles and responsibilities.**

- **System Admin** — manages the overall system; adds users, assigns roles, configures
  settings, so needs unrestricted access to all resources.
- **Doctor** — delivers direct care; accesses and updates records for *assigned* patients,
  views their test results, prescribes medication. *Limiting to assigned patients reduces
  the risk of exposing private information unnecessarily.*
- **Nurse** — records and updates vitals; does **not** need test results or prescriptions
  (handled by doctors). Limiting access reduces errors and unauthorized actions.
- **Patient** — views their own records to stay informed; restricting them to their own
  data ensures privacy and security.

**Step 2 — Assign permissions.**

- **System Admin** — full access to all system resources, including user management and
  patient records (to maintain functionality and security comprehensively).
- **Doctor** — view/update assigned patient records, view test results for assigned
  patients, prescribe medications for assigned patients.
- **Nurse** — view/update vitals for patients (responsibilities limited to assisting with
  basic medical data).
- **Patient** — view their own medical records only (ensuring privacy, preventing access
  to others' information).

**Step 3 — Summarize the design.**

| Role           | Permissions                                                                  |
| -------------- | ---------------------------------------------------------------------------- |
| `System Admin` | Full access to all system resources, user management, patient records.       |
| `Doctor`       | View/update assigned patient records, view test results, prescribe medications. |
| `Nurse`        | View/update vitals for patients.                                             |
| `Patient`      | View personal medical records.                                               |

**Why this design works.** Roles map to distinct responsibilities in a typical healthcare
workflow — admins oversee the system, doctors deliver care, nurses support, patients access
their own data. Each permission is tied to what the role needs while minimizing access to
unnecessary or sensitive data (**least privilege**), so sensitive patient data is reachable
only by those who need it, minimizing breach risk.

### Example 2 — E-Commerce Platform

**Scenario.** An e-commerce platform must securely manage access for a **Super Admin**
(manages users, inventory, and orders), **Warehouse Staff** (handles inventory and shipping
but cannot access customer data), a **Customer Service Agent** (manages customer orders and
communications), and a **Customer** (browses products, places orders, views their order
history).

**Step 1 — Identify roles and responsibilities.**

- **Super Admin** — oversees the entire platform (users, inventory, orders); requires
  unrestricted access to manage operations.
- **Warehouse Staff** — handle inventory updates and shipping logistics; do **not** need
  customer data, reducing the risk of exposing private information.
- **Customer Service Agent** — interact with customers, manage orders, resolve issues; do
  **not** modify inventory or access backend data.
- **Customer** — browse products, place orders, view their own order history; restricting
  access stops them viewing or modifying other users' data.

**Step 2 — Assign permissions.**

- **Super Admin** — full permissions for user, inventory, and order management.
- **Warehouse Staff** — update inventory and manage shipping (backend stock/logistics only,
  no customer data).
- **Customer Service Agent** — view/manage orders and access customer communications, but
  not inventory or backend data.
- **Customer** — browse products, place orders, view personal order history (own data only).

**Step 3 — Summarize the design.**

| Role                     | Permissions                                                |
| ------------------------ | ---------------------------------------------------------- |
| `Super Admin`            | Full access to users, inventory, and orders.               |
| `Warehouse Staff`        | Update inventory, manage shipping.                         |
| `Customer Service Agent` | View/manage orders, access customer communications.        |
| `Customer`               | Browse products, place orders, view personal order history.|

**Why this design works.** Roles reflect the platform's primary operations; permissions
match each role's responsibilities while minimizing unnecessary access. Limiting roles and
permissions to specific tasks keeps customer data secure, reduces breach risk, and prevents
unauthorized actions.

---

## ✍️ Your turn

Apply the same three-step method (roles → permissions → table) to design RBAC systems for
the following. For each, define the roles and their responsibilities, assign permissions
based on the tasks each role performs, and summarize the design in a role/permission table.

### Task 1 — Learning Management System (LMS)

An LMS must securely manage access for:

- **Admin** — manages users, courses, and settings.
- **Instructor** — manages their own courses, grades assignments, views enrolled students.
- **Student** — views their enrolled courses, assignments, and grades.
- **Guest** — views general course information but cannot enroll or access private data.

### Task 2 — Retail Bank

A retail bank must securely manage access for:

- **Admin** — manages accounts, transactions, and customer data.
- **Teller** — processes transactions but cannot access full account histories.
- **Auditor** — reviews system logs and transactions without accessing sensitive customer details.
- **Customer** — views their own account details and transaction history.

---

## 🌟 What good looks like

- **Every role is justified.** Each role maps to a real responsibility in the workflow, and
  each permission is explained in terms of the task that requires it.
- **Least privilege is visible.** No role can reach data outside its remit — tellers can't
  open full account histories, warehouse staff can't read customer records, students can't
  grade, and guests see only public catalog data (deny-by-default).
- **Self-access is scoped to ownership.** End users see *their own* data only (patient
  records, personal orders, own grades); instructors edit *their own* courses; doctors act
  on *assigned* patients — never anyone else's.
- **Sensitive operations are separated.** Auditors get **read-only** access so they can't
  alter the records they review (audit integrity), keeping oversight independent from the
  operations it checks — a classic segregation-of-duties control.
- **Privacy regulations are respected.** Scoping students to their own grades echoes
  educational privacy rules (e.g., FERPA), and walling customer PII off from tellers and
  auditors echoes financial/data rules (e.g., PCI DSS, GDPR).
- **The table is the deliverable.** A clean role → permissions table makes the access model
  reviewable at a glance and easy to translate into policy.

---

## ☑️ Definition of done

- [ ] Roles for **each** scenario defined, with their responsibilities described
- [ ] Permissions assigned to every role, each with a justification
- [ ] A role/permission **table** summarizes each design
- [ ] Designs honor the **principle of least privilege** (no excess access)
- [ ] End-user roles are restricted to **their own** data only
- [ ] Sensitive data (test results, customer records, account histories) is walled off from roles that don't need it

---

## 🔑 Key concepts

- **Role-Based Access Control** — group permissions into roles and assign roles to users,
  instead of granting permissions to individuals one by one.
- **Principle of least privilege** — a role receives only the access its job requires;
  minimizing access reduces the attack surface and the chance of errors or breaches.
- **Permission ↔ responsibility** — every permission should trace back to a concrete task
  the role must perform; if no task needs it, don't grant it.
- **Data isolation** — restricting roles to their core duties keeps sensitive data (patient
  records, customer data, account histories) reachable only by those who genuinely need it.
- **Requirements-driven design** — roles and permissions are derived from analyzing the
  system's real-world workflow, not assumed up front.
