# Activity - User Registration

**Course 8 — Security and Authentication** · Module 1 · Lesson 1 · `Activity`

> Build a simple **user registration form** in plain HTML and lean on **HTML5 built-in
> validation** (`required`, `type`, `minlength`, `pattern`, `title`) to guarantee input
> meets your rules *before* it is ever submitted — then add a touch of JavaScript to
> confirm the two password fields match.

---

## 🎯 Objective

Create a basic user registration form and use HTML5 validation features to ensure user
input meets specific criteria. You will start from a minimal form, expand it with extra
fields and constraints, and finish by completing two challenges on your own.

---

## 🗂️ What you will build

A single `index.html` page containing one `<form>` that collects and validates the
following fields:

| Field              | Input type        | Validation                                              |
| ------------------ | ----------------- | ------------------------------------------------------- |
| Name               | `text`            | `required`                                              |
| Email              | `email`           | `required` + valid email format (`@` and a domain)      |
| Password           | `password`        | `required`, `minlength="6"` (masked input)              |
| Confirm Password   | `password`        | `required`, `pattern=".{6,}"`, must match Password (JS) |
| Phone Number       | `tel`             | `required`, `pattern="\d{10}"` (exactly 10 digits)      |
| Username *(task)*  | `text`            | `required`, `pattern="[a-zA-Z0-9]{4,12}"` (alphanumeric, 4–12 chars) |

**Flow:** `User fills form → HTML5 constraints validate on submit → JS checks passwords match → submit`

---

## ✅ Prerequisites

- A text editor (Visual Studio Code recommended)
- A modern web browser (Chrome, Edge, or Firefox) to render and test the form
- Basic familiarity with HTML elements and attributes

---

## 🛠️ Walkthrough

### Step 1 — Create the basic registration form

Start with a minimal form that has **required** fields and basic type validation. Save it
as `index.html`.

```html
<form id="registrationForm">
    <label for="name">Name:</label>
    <input type="text" id="name" name="name" required><br><br>

    <label for="email">Email:</label>
    <input type="email" id="email" name="email" required><br><br>

    <label for="password">Password:</label>
    <input type="password" id="password" name="password" required minlength="6"><br><br>

    <button type="submit">Register</button>
</form>
```

**How it works**

- **Name field** — the `required` attribute stops the user from leaving the field blank.
- **Email field** — `type="email"` forces a valid email format, such as containing `@`
  and a domain.
- **Password field** — `type="password"` masks the input for privacy, and `minlength="6"`
  requires at least 6 characters.
- **Submit button** — triggers submission only when every field passes validation.

### Step 2 — Expand the form with new fields

Add **Confirm Password** and **Phone Number** fields, each with their own validation
rules.

```html
<form id="registrationForm">
    <label for="name">Name:</label>
    <input type="text" id="name" name="name" required><br><br>

    <label for="email">Email:</label>
    <input type="email" id="email" name="email" required><br><br>

    <label for="password">Password:</label>
    <input type="password" id="password" name="password" required minlength="6"><br><br>

    <label for="confirmPassword">Confirm Password:</label>
    <input type="password" id="confirmPassword" name="confirmPassword" required
           pattern=".{6,}" title="Must match the password"><br><br>

    <label for="phone">Phone Number:</label>
    <input type="tel" id="phone" name="phone" required
           pattern="\d{10}" title="Must be 10 digits"><br><br>

    <button type="submit" id="btnSubmit">Register</button>
</form>
```

**How it works**

- **Confirm Password field** — `required` makes it mandatory, `pattern=".{6,}"` enforces a
  minimum length of 6 characters, and `title` shows a helpful tooltip when the user hovers
  over the field or fails validation.
- **Phone Number field** — `type="tel"` accepts numeric input, `pattern="\d{10}"` requires
  exactly 10 numeric digits, and `title` explains the format requirement to the user.

> **Note:** a `pattern` only checks *shape*. It cannot compare two fields, so Confirm
> Password matching the Password still needs a little JavaScript — that is your first task.

---

## 🧩 Your turn

### Task 1 — Make Confirm Password actually match

Building on Example 2, ensure the **Confirm Password** field truly matches **Password**.

- Confirm the Confirm Password field has `required`, a `pattern` for the minimum length,
  and a `title` to explain the requirement.
- Add JavaScript that compares the `password` and `confirmPassword` values on submit and
  blocks the form (with a clear message) when they differ.

A simple, idiomatic way to do this is the Constraint Validation API — `setCustomValidity`:

```html
<script>
  const form = document.getElementById("registrationForm");
  const password = document.getElementById("password");
  const confirmPassword = document.getElementById("confirmPassword");

  function validatePasswordMatch() {
    if (confirmPassword.value !== password.value) {
      confirmPassword.setCustomValidity("Passwords do not match");
    } else {
      confirmPassword.setCustomValidity(""); // clears the error
    }
  }

  password.addEventListener("input", validatePasswordMatch);
  confirmPassword.addEventListener("input", validatePasswordMatch);

  form.addEventListener("submit", (event) => {
    validatePasswordMatch();
    if (!form.checkValidity()) {
      event.preventDefault(); // stops submission while invalid
    }
  });
</script>
```

### Task 2 — Add a Username field

Add a **Username** field with these validation rules:

- Alphanumeric characters only.
- Length between **4 and 12** characters.

Referring to Examples 1 and 2, add the field with:

- The `required` attribute to make it mandatory.
- The `pattern="[a-zA-Z0-9]{4,12}"` attribute to enforce alphanumeric characters and length.
- The `title` attribute to provide guidance.

```html
<label for="username">Username:</label>
<input type="text" id="username" name="username" required
       pattern="[a-zA-Z0-9]{4,12}"
       title="4–12 letters or numbers, no spaces or symbols"><br><br>
```

---

## ☑️ Definition of done

- [ ] `index.html` contains a `<form id="registrationForm">` that renders in the browser
- [ ] **Name**, **Email**, and **Password** fields enforce `required`, `type="email"`, and `minlength="6"`
- [ ] **Confirm Password** uses `required`, `pattern=".{6,}"`, and a `title` tooltip
- [ ] **Phone Number** uses `type="tel"` and `pattern="\d{10}"` for exactly 10 digits
- [ ] JavaScript blocks submission when Password and Confirm Password do not match (Task 1)
- [ ] A **Username** field enforces `pattern="[a-zA-Z0-9]{4,12}"` (Task 2)
- [ ] Submitting with invalid input shows the browser's validation messages and does not proceed

---

## 🔑 Key concepts

- **HTML5 constraint validation is the first line of defense** — `required`, `type`,
  `minlength`, and `pattern` let the browser block bad input with zero JavaScript.
- **`pattern` validates shape, not relationships** — it can enforce "10 digits" or
  "alphanumeric 4–12", but comparing two fields (password vs. confirm) needs script.
- **`title` doubles as a hint and an error message** — it surfaces as a tooltip and in the
  browser's validation bubble, so write it for the user.
- **`setCustomValidity` integrates JS checks into native validation** — set a message to
  mark a field invalid, set `""` to clear it, and the form behaves consistently.
- **Client-side validation is for UX, not security** — it improves the experience, but the
  server must still re-validate every field, since users can bypass the browser.
