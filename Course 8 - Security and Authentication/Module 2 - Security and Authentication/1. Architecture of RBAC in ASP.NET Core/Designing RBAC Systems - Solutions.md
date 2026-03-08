# Designing Role-Based Access Control (RBAC) Systems — Solutions

---

## Task 1: RBAC System for a Learning Management System (LMS)

### Scenario

An LMS must securely manage access for:
1. **Admin** — Manages users, courses, and settings.
2. **Instructor** — Manages their own courses, grades assignments, and views enrolled students.
3. **Student** — Views their enrolled courses, assignments, and grades.
4. **Guest** — Views general course information but cannot enroll or access private data.

---

### 1. Identify the Roles and Their Responsibilities

- **Admin:**
  - Oversees the entire LMS platform: manages user accounts, creates and removes courses, configures system settings (e.g., enrollment periods, grading policies). Requires unrestricted access to ensure the platform operates smoothly and securely.

- **Instructor:**
  - Responsible for teaching and course management. Creates course content, uploads materials, grades student assignments, and views the list of enrolled students for their own courses. Access is scoped to their own courses to prevent unauthorized interference with other instructors' content.
  - *Why limited to own courses?* Following the principle of least privilege — instructors should only modify content they are responsible for, preventing accidental or intentional changes to another instructor's course.

- **Student:**
  - Consumes educational content. Views enrolled courses, submits assignments, and checks their own grades. Cannot access other students' grades or unenrolled courses' private materials.
  - *Why limited to own data?* Ensures academic privacy — grades and submissions are personal data protected under educational privacy regulations (e.g., FERPA).

- **Guest:**
  - A prospective user who can browse the course catalog and view general course descriptions (title, syllabus overview, instructor name). Cannot enroll, submit work, or view any private content such as grades, assignments, or student lists.
  - *Why so restricted?* Guests are unauthenticated or unverified users. Granting minimal read-only access to public data encourages enrollment while protecting the platform's private resources.

---

### 2. Assign Permissions to Each Role

- **Admin:** Full access to all system resources — user management (CRUD), course management (CRUD), system settings, enrollment management, view all grades and reports.
  - *Why?* Admins maintain the platform's functionality, resolve issues, and ensure compliance. They need unrestricted access to fulfill these responsibilities.

- **Instructor:**
  - Create, update, and delete content for their own courses.
  - View the list of enrolled students in their own courses.
  - Grade assignments and provide feedback for their own courses.
  - Upload and manage course materials (lectures, files, quizzes).
  - *Why?* These permissions enable instructors to deliver education effectively. Scoping to own courses enforces separation of duties and protects other instructors' intellectual property.

- **Student:**
  - View enrolled courses and their content (lectures, materials).
  - Submit assignments for enrolled courses.
  - View their own grades and feedback.
  - Enroll in available courses (within enrollment period).
  - *Why?* Students need access to consume content and track their academic progress. Restricting to enrolled courses and own grades maintains privacy and prevents unauthorized access.

- **Guest:**
  - View the course catalog (titles, descriptions, instructor names).
  - View general platform information (about page, FAQ).
  - *Why?* Read-only access to public information allows prospective users to evaluate the platform before committing to registration, without exposing any private data.

---

### 3. RBAC Design Summary Table

| Role | Permissions |
|---|---|
| **Admin** | Full access: user management (CRUD), course management (CRUD), system settings, enrollment management, view all grades and reports. |
| **Instructor** | Create/update/delete own course content, upload materials, view enrolled students (own courses), grade assignments, provide feedback. |
| **Student** | View enrolled courses and materials, submit assignments, view own grades and feedback, enroll in available courses. |
| **Guest** | View course catalog (titles, descriptions, instructor names), view general platform information. |

---

### Explanation of the Design Process

- **How Were Roles Defined?** The roles correspond to the natural hierarchy in an educational environment: administrators manage the platform, instructors deliver content, students consume it, and guests explore before committing. Each role has clearly distinct responsibilities with no overlap in authority.

- **Why Are Permissions Assigned This Way?** Each permission directly maps to a task the role must perform. The principle of least privilege is applied throughout — instructors cannot access other instructors' courses, students cannot see other students' grades, and guests cannot access any private resources. This minimizes the attack surface and limits the blast radius of a compromised account.

- **How Does This System Protect Data?** By scoping access to ownership (own courses for instructors, own grades for students) and applying deny-by-default for guests, the system ensures that sensitive academic data — grades, submissions, student lists — is only visible to authorized parties. This aligns with educational privacy standards.

---

---

## Task 2: RBAC System for a Retail Bank

### Scenario

A retail bank must securely manage access for:
1. **Admin** — Manages accounts, transactions, and customer data.
2. **Teller** — Processes transactions but cannot access full account histories.
3. **Auditor** — Reviews system logs and transactions without accessing sensitive customer details.
4. **Customer** — Views their own account details and transaction history.

---

### 1. Identify the Roles and Their Responsibilities

- **Admin:**
  - Oversees the entire banking system: manages customer accounts (open, close, modify), configures system settings, manages user roles, and has full access to transaction records and customer data. This role ensures the bank's operations run securely and in compliance with regulations.

- **Teller:**
  - A front-line employee who processes day-to-day transactions: deposits, withdrawals, and transfers on behalf of customers. Does not need access to full account histories or sensitive personal customer data beyond what is required for the current transaction.
  - *Why limited access?* Tellers handle high-volume, routine operations. Restricting them to current transaction processing reduces the risk of insider threats and data exfiltration. Full account histories contain patterns that could be exploited.

- **Auditor:**
  - Reviews system logs, transaction records, and compliance reports to ensure regulatory adherence and detect anomalies (e.g., fraud, money laundering). Cannot access sensitive customer personal details (SSN, address, phone) or modify any data.
  - *Why read-only and no PII?* Auditors need to verify that operations are legitimate, not to identify individual customers. Separating audit access from personal data follows the principle of least privilege and satisfies regulatory requirements for segregation of duties.

- **Customer:**
  - Views their own account balances, transaction history, and personal profile. Can initiate transfers and payments from their own accounts. Cannot access any other customer's data or system-level resources.
  - *Why limited to own data?* Banking data is highly sensitive. Each customer must only see and interact with their own financial information, ensuring privacy and compliance with financial regulations (e.g., PCI DSS, GDPR).

---

### 2. Assign Permissions to Each Role

- **Admin:** Full access — account management (open/close/modify), user and role management, transaction oversight, customer data management, system configuration, compliance reports.
  - *Why?* Admins are responsible for the entire banking infrastructure. They must be able to resolve escalated issues, manage access, and ensure regulatory compliance.

- **Teller:**
  - Process deposits, withdrawals, and transfers for customers.
  - View basic customer account information (name, account number, current balance) required for the transaction.
  - *Why?* Tellers need just enough information to process a transaction correctly. Denying access to full history and sensitive PII limits exposure in case of a compromised or malicious teller account.

- **Auditor:**
  - View system logs and audit trails (read-only).
  - View transaction records (read-only, anonymized or with limited PII).
  - Generate and view compliance reports.
  - *Why?* Auditors must verify the integrity of operations without the ability to alter records (which would undermine the audit itself). Limited PII access ensures they can trace transactions without unnecessarily exposing customer identities.

- **Customer:**
  - View own account balances and transaction history.
  - View and update own personal profile (address, phone, email).
  - Initiate transfers and payments from own accounts.
  - *Why?* Customers interact exclusively with their own financial data. Self-service capabilities (transfers, profile updates) reduce operational load while strict scoping to own accounts prevents unauthorized access.

---

### 3. RBAC Design Summary Table

| Role | Permissions |
|---|---|
| **Admin** | Full access: account management (open/close/modify), user and role management, transaction oversight, customer data management, system configuration, compliance reports. |
| **Teller** | Process deposits/withdrawals/transfers, view basic customer info (name, account number, current balance) for active transactions. |
| **Auditor** | View system logs (read-only), view transaction records (read-only, limited PII), generate compliance reports. |
| **Customer** | View own account balances and transaction history, update own profile, initiate transfers and payments from own accounts. |

---

### Explanation of the Design Process

- **How Were Roles Defined?** The roles reflect the segregation of duties critical in the banking sector: administrators control the system, tellers handle front-line operations, auditors provide independent oversight, and customers manage their personal finances. This separation is a regulatory requirement in financial institutions.

- **Why Are Permissions Assigned This Way?** Each role receives the minimum permissions needed to perform its function. Tellers cannot see full histories (reducing insider threat risk), auditors cannot modify data (preserving audit integrity), and customers are scoped to their own accounts (ensuring privacy). This follows the principle of least privilege and defense in depth.

- **How Does This System Protect Data?** The layered approach ensures that no single role has unchecked access. Sensitive customer data (PII, full transaction history) is compartmentalized — only admins can access it fully, tellers see minimal info per transaction, auditors see anonymized records, and customers see only their own data. This minimizes the impact of a breach at any level and satisfies regulatory compliance requirements.
