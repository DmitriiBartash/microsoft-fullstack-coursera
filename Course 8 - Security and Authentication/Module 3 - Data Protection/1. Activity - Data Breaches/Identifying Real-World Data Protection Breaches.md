# Identifying Real-World Data Protection Breaches

**Course 8 — Security and Authentication** · Module 3 · Lesson 1 · `You Try It!`

> Analyze real-world data protection breaches through the lens of the **CIA Triad**
> — **C**onfidentiality, **I**ntegrity, **A**vailability. For each incident you decide
> *which* security properties were violated, *who* was harmed, and *what* controls
> could have prevented or contained it.

---

## 🎯 Objective

Learn to identify and analyze real-world examples of data protection breaches and
understand their impact on **confidentiality, integrity, or availability**.

---

## 📦 What you will produce

For each breach, a short written analysis with three parts:

| Part | What you answer |
| ---------------------- | ------------------------------------------------------------------- |
| **CIA impact** | Which of Confidentiality, Integrity, Availability were affected — and which were not |
| **Impact on people** | How users / customers / patients *and* the organization were harmed |
| **Prevention measures**| Concrete controls that could have prevented or mitigated the breach |

**Lens:** `Incident → CIA impact → Impact on people → Prevention measures`

---

## 🧭 The CIA Triad — quick reference

| Property | Question it answers | A breach of it looks like… |
| ----------------- | --------------------------------------- | ----------------------------------------------- |
| **Confidentiality** | Is data kept secret from the unauthorized? | Sensitive records (SSNs, card data) are exposed or stolen |
| **Integrity** | Is data accurate and untampered? | Records are altered, corrupted, or encrypted by an attacker |
| **Availability** | Can authorized users access it when needed? | Systems or data go offline (outage, ransomware lockout) |

---

## 🔍 Walkthrough — worked examples

Two solved incidents that model the expected analysis.

### Example 1 — Equifax Data Breach (2017)

**Scenario.** In 2017, Equifax, one of the largest credit reporting agencies, experienced
a massive data breach. Attackers exploited a vulnerability in a web application, exposing
sensitive personal data of **147 million people**, including Social Security numbers,
birthdates, and addresses.

**CIA impact analysis:**

| Property | Impacted? | Why |
| ----------------- | --------- | ------------------------------------------------------------------ |
| **Confidentiality** | ✅ Yes | Exposed sensitive user data (Social Security numbers, birthdates) |
| **Integrity** | ⚠️ Potential | Risk that attackers could alter credit records |
| **Availability** | ❌ No | No significant disruption to Equifax's services |

**Impact on users and company:**

- **Users** — personal data was exposed, leading to risks of identity theft and financial fraud.
- **Company** — Equifax faced lawsuits, regulatory fines, and significant reputational damage.

**Prevention measures:**

- Apply security patches promptly to prevent exploitation of known vulnerabilities.
- Conduct regular security audits and penetration testing.
- Encrypt sensitive data to minimize the impact of a breach.

### Example 2 — Colonial Pipeline Ransomware Attack (2021)

**Scenario.** In 2021, a ransomware attack on Colonial Pipeline disrupted fuel supply across
the Eastern United States. Attackers infiltrated the company's IT systems, encrypting critical
data and demanding a ransom to restore operations.

**CIA impact analysis:**

| Property | Impacted? | Why |
| ----------------- | --------- | ------------------------------------------------------------ |
| **Confidentiality** | ❌ No | No significant data exposure was reported |
| **Integrity** | ⚠️ Potential | Possible tampering with operational systems |
| **Availability** | ✅ Yes | Pipeline operations were halted for days, severely impacting fuel availability |

**Impact on users and company:**

- **Users** — fuel shortages caused widespread disruptions to businesses and consumers.
- **Company** — paid a **$4.4 million** ransom, suffered reputational damage, and faced regulatory scrutiny.

**Prevention measures:**

- Implement network segmentation to isolate critical systems.
- Regularly back up data and test recovery plans.
- Use multi-factor authentication to prevent unauthorized access.

---

## ✍️ Your turn

Apply the same three-part analysis (CIA impact → impact on people → prevention measures) to
each scenario below.

### Task 1 — Analyze a real-world data breach

**Scenario.** In 2019, Capital One suffered a breach that exposed personal information of over
**100 million customers**, including credit card applications and Social Security numbers.
Attackers exploited a **misconfigured firewall** to access data stored in the company's cloud
environment.

Your task:

- Identify which aspect(s) of the CIA Triad were impacted.
- Explain how the breach affected users and the company.
- Describe potential measures that could have prevented this breach.

### Task 2 — Analyze a data protection incident in healthcare

**Scenario.** In 2020, a ransomware attack on a major hospital system disrupted access to
**electronic medical records (EMRs)** and delayed critical care for patients. The attackers
encrypted data and demanded a ransom for decryption keys.

Your task:

- Identify which aspect(s) of the CIA Triad were impacted.
- Explain how the breach affected patients and the healthcare provider.
- Describe potential measures that could have mitigated this breach.

---

## ☑️ Definition of done

- [ ] Each breach is mapped to the CIA Triad, stating which properties were affected **and** which were not
- [ ] The human and organizational impact is described for every scenario
- [ ] At least two concrete prevention or mitigation measures are listed per scenario
- [ ] Task 1 (Capital One) is analyzed — note the **misconfigured firewall** / cloud root cause
- [ ] Task 2 (healthcare ransomware) is analyzed — note the **availability** impact on patient care

---

## 🔑 Key concepts

- **The CIA Triad is the analysis frame** — Confidentiality, Integrity, and Availability give a
  repeatable structure for reasoning about *any* security incident.
- **A breach rarely hits all three equally** — data-theft breaches (Equifax, Capital One) attack
  confidentiality; ransomware (Colonial Pipeline, healthcare) primarily attacks availability.
- **Root cause points to the control** — unpatched software, a misconfigured firewall, or missing
  MFA each map directly to a prevention measure (patching, secure cloud config, MFA, segmentation).
- **Impact is dual** — breaches harm both individuals (identity theft, delayed care) and the
  organization (fines, ransom payments, reputational damage, regulatory scrutiny).
- **Defense is layered** — patching, encryption, segmentation, MFA, audits, and tested backups
  combine so that no single failure becomes a catastrophic breach.
