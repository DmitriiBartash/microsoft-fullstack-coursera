# Identifying Real-World Data Protection Breaches

## Task 1: Capital One Data Breach (2019)

### Scenario

In 2019, Capital One suffered a breach that exposed personal information of over 100 million customers, including credit card applications and Social Security numbers. Attackers exploited a misconfigured firewall to access data stored in the company's cloud environment (AWS).

### 1. CIA Triad Impact Analysis

- **Confidentiality** — severely impacted. Sensitive personal data of 100M+ customers was exposed: Social Security numbers, credit card application data, bank account numbers, and credit scores. This is a direct violation of data confidentiality.
- **Integrity** — potentially impacted. Once the attacker gained access through the misconfigured firewall, there was a possibility of modifying or tampering with customer records and credit application data, though no evidence of data alteration was publicly confirmed.
- **Availability** — not significantly impacted. Capital One's services remained operational; the attack was focused on data exfiltration rather than disrupting systems.

### 2. Impact on Users and Company

- **Users:** Personal and financial data was exposed, putting over 100 million customers at risk of identity theft, fraudulent credit applications, and financial fraud. Approximately 140,000 Social Security numbers and 80,000 bank account numbers were compromised.
- **Company:** Capital One faced an $80 million fine from the Office of the Comptroller of the Currency (OCC), class-action lawsuits, significant reputational damage, and loss of customer trust. The company also incurred substantial costs for breach remediation and credit monitoring services for affected customers.

### 3. Prevention Measures

- **Proper cloud configuration management** — implement automated tools (e.g., AWS Config, CloudFormation Guard) to detect and remediate firewall and security group misconfigurations before they are exploited.
- **Principle of least privilege** — restrict IAM roles and permissions so that no single role has broad access to sensitive data stores. Use fine-grained access controls.
- **Web Application Firewall (WAF) hardening** — regularly audit WAF rules and ensure SSRF (Server-Side Request Forgery) protection is in place, as the attacker used SSRF to retrieve IAM credentials from the metadata service.
- **Data encryption and tokenization** — encrypt sensitive data at rest and in transit, and tokenize PII so that even if accessed, the data is unusable without decryption keys.
- **Continuous security monitoring** — deploy cloud security posture management (CSPM) tools and intrusion detection systems to identify anomalous access patterns in real time.

---

## Task 2: Hospital System Ransomware Attack (2020)

### Scenario

In 2020, a ransomware attack on a major hospital system disrupted access to electronic medical records (EMRs) and delayed critical care for patients. The attackers encrypted data and demanded a ransom for decryption keys.

### 1. CIA Triad Impact Analysis

- **Confidentiality** — potentially impacted. Ransomware operators increasingly use double extortion tactics — encrypting data and threatening to publish it. Patient medical records, diagnoses, treatment histories, and personal identifiers may have been exfiltrated before encryption.
- **Integrity** — impacted. The encryption of EMRs altered the usable state of medical data. Healthcare providers could not verify whether patient records had been tampered with, undermining trust in the accuracy of medical information even after recovery.
- **Availability** — severely impacted. This was the primary target. Access to EMRs was completely blocked, forcing the hospital to revert to paper-based processes, divert emergency patients to other facilities, postpone surgeries, and delay critical treatments — directly endangering patient lives.

### 2. Impact on Patients and Healthcare Provider

- **Patients:** Critical care was delayed, potentially leading to worsened health outcomes or life-threatening situations. Patients had to be redirected to other hospitals. Sensitive medical data (diagnoses, medications, insurance info) was at risk of exposure. Loss of trust in the healthcare provider's ability to protect personal health information.
- **Healthcare Provider:** Operational paralysis — staff could not access patient histories, medication lists, or treatment plans. Financial losses from operational downtime, incident response costs, potential ransom payment, and regulatory fines (e.g., HIPAA violations). Reputational damage and loss of patient trust. Potential legal liability if delayed care resulted in patient harm.

### 3. Prevention Measures

- **Offline and immutable backups** — maintain regular, air-gapped backups of all critical systems (especially EMRs) and routinely test restoration procedures to ensure rapid recovery without paying ransom.
- **Network segmentation** — isolate clinical systems (EMR, medical devices) from administrative IT networks so that a compromise in one segment cannot spread to life-critical systems.
- **Multi-factor authentication (MFA)** — enforce MFA for all remote access, VPN connections, and privileged accounts to prevent unauthorized access via stolen credentials.
- **Endpoint detection and response (EDR)** — deploy advanced endpoint protection across all workstations and servers to detect and block ransomware before it can encrypt files.
- **Staff security training** — conduct regular phishing awareness and cybersecurity training for all hospital staff, as ransomware often enters through phishing emails.
- **Incident response plan** — develop and regularly drill a specific ransomware incident response plan that includes procedures for maintaining patient care during system outages.
