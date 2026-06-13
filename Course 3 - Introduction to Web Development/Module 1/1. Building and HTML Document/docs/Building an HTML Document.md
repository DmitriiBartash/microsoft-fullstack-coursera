# Building an HTML Document

**Course 3 — Introduction to Web Development** · Module 1 · Lesson 1 · `You Try It!`

> Build a structured **Personal Webpage** from scratch using core HTML elements —
> headings, paragraphs, ordered and unordered lists, and a feedback **form** — that
> introduces you, lists your interests and goals, and lets visitors leave feedback.
> One semantic document, assembled section by section.

---

## 🎯 Objective

Build a structured HTML document that includes headings, lists, and a form, following
web development best practices. By the end you will have a single, valid `.html` file —
a personal webpage that applies the foundational HTML elements you will reuse on every
project that follows.

---

## 📄 Scenario

You are tasked with creating a **Personal Webpage** that introduces yourself, displays
your interests, and includes a form for visitors to leave feedback. This activity helps
you apply HTML elements such as headings, lists, forms, and more in a realistic page layout.

---

## 🗂️ What you'll build

A single file named **`index.html`** with a standard document skeleton and five content sections:

| Section          | Elements                                    | Purpose                                  |
| ---------------- | ------------------------------------------- | ---------------------------------------- |
| Document head    | `<!DOCTYPE>`, `<head>`, `<meta>`, `<title>` | Declare the doc type, charset, viewport  |
| About Me         | `<h1>`, `<h2>`, `<p>`                        | Introduce yourself                       |
| Favorite Hobbies | `<h3>`, `<ul>`, `<li>`                       | An **unordered** list of interests       |
| Personal Goals   | `<h3>`, `<ol>`, `<li>`                       | An **ordered** list of goals             |
| Feedback Form    | `<form>`, `<label>`, `<input>`              | Collect a name, email, and a rating      |

**Flow:** `Document skeleton → headings & intro → unordered list → ordered list → form`

---

## ✅ Prerequisites

- A plain text editor — **Visual Studio Code** is recommended
- A modern web browser (Chrome, Edge, or Firefox) to open and preview the file
- No build tools, server, or extensions required — HTML runs straight from the file

---

## 🛠️ Steps

### Step 1 — Create the document skeleton

Create a new file named **`index.html`**. Start with the HTML5 doctype and the
`<head>` metadata that every page needs: the character set, a responsive viewport, and
a page title shown in the browser tab.

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>My Personal Webpage</title>
</head>
<body>
  <!-- page content goes here -->
</body>
</html>
```

- `<!DOCTYPE html>` tells the browser to use standards mode.
- `lang="en"` declares the document language for accessibility and search engines.
- `<meta charset="UTF-8">` ensures characters render correctly.
- The viewport `<meta>` makes the page scale properly on mobile devices.

### Step 2 — Add the headings and introduction

Inside `<body>`, add a top-level `<h1>` for the page, an `<h2>` for the "About Me"
section, and a `<p>` paragraph introducing yourself. Headings establish the page's
outline; use them in order (`h1` → `h2` → `h3`) rather than for visual size.

```html
  <h1>Welcome to My Personal Webpage</h1>
  <h2>About Me</h2>
  <p>
    Hello! My name is Dima, and I'm passionate about web development and technology.
    I enjoy learning new skills, building projects, and exploring creative solutions through code.
    This webpage is a place where I share more about myself and my interests.
  </p>
```

### Step 3 — Add an unordered list of hobbies

Use an `<h3>` subheading followed by an unordered list `<ul>`. Each item is a `<li>`.
Unordered lists render with bullets and are for items whose order doesn't matter.

```html
  <h3>My Favorite Hobbies</h3>
  <ul>
    <li>Coding</li>
    <li>Reading tech blogs</li>
    <li>Playing board games</li>
    <li>Playing video games</li>
    <li>Traveling</li>
  </ul>
```

### Step 4 — Add an ordered list of goals

For goals where sequence or priority matters, use an ordered list `<ol>`. Browsers
number the items automatically, so you never hard-code "1.", "2.", "3.".

```html
  <h3>My Personal Goals</h3>
  <ol>
    <li>Become a full-stack web developer</li>
    <li>Build my own personal portfolio website</li>
    <li>Contribute to open-source projects</li>
    <li>Learn advanced JavaScript and frameworks like React or Angular</li>
    <li>Land a job in a tech company</li>
  </ol>
```

### Step 5 — Add the feedback form

Build a `<form>` that collects a name, an email, and a rating. Pair every input with a
`<label>` via matching `for`/`id` attributes for accessibility, mark required fields with
the `required` attribute, and use the right `type` for each control (`text`, `email`,
`radio`, `submit`).

```html
  <h3>Feedback Form</h3>
  <form action="#" method="post">
    <label for="name">Name:</label><br>
    <input type="text" id="name" name="name" required><br><br>

    <label for="email">Email:</label><br>
    <input type="email" id="email" name="email" required><br><br>

    <p>How would you rate this site?</p>
    <input type="radio" id="excellent" name="rating" value="Excellent" required>
    <label for="excellent">Excellent</label><br>
    <input type="radio" id="good" name="rating" value="Good">
    <label for="good">Good</label><br>
    <input type="radio" id="average" name="rating" value="Average">
    <label for="average">Average</label><br><br>

    <input type="submit" value="Submit Feedback">
  </form>
```

- All three radio buttons share `name="rating"`, so only one can be selected at a time.
- `action="#"` is a placeholder target; `method="post"` sends the data in the request body.

### Step 6 — Assemble and preview

Put it all together into one file, then **open `index.html` in your browser** (double-click
it, or drag it onto a browser tab). You should see your headings, both lists, and a working
feedback form.

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>My Personal Webpage</title>
</head>
<body>
  <h1>Welcome to My Personal Webpage</h1>
  <h2>About Me</h2>
  <p>
    Hello! My name is Dima, and I'm passionate about web development and technology.
    I enjoy learning new skills, building projects, and exploring creative solutions through code.
    This webpage is a place where I share more about myself and my interests.
  </p>

  <h3>My Favorite Hobbies</h3>
  <ul>
    <li>Coding</li>
    <li>Reading tech blogs</li>
    <li>Playing board games</li>
    <li>Playing video games</li>
    <li>Traveling</li>
  </ul>

  <h3>My Personal Goals</h3>
  <ol>
    <li>Become a full-stack web developer</li>
    <li>Build my own personal portfolio website</li>
    <li>Contribute to open-source projects</li>
    <li>Learn advanced JavaScript and frameworks like React or Angular</li>
    <li>Land a job in a tech company</li>
  </ol>

  <h3>Feedback Form</h3>
  <form action="#" method="post">
    <label for="name">Name:</label><br>
    <input type="text" id="name" name="name" required><br><br>

    <label for="email">Email:</label><br>
    <input type="email" id="email" name="email" required><br><br>

    <p>How would you rate this site?</p>
    <input type="radio" id="excellent" name="rating" value="Excellent" required>
    <label for="excellent">Excellent</label><br>
    <input type="radio" id="good" name="rating" value="Good">
    <label for="good">Good</label><br>
    <input type="radio" id="average" name="rating" value="Average">
    <label for="average">Average</label><br><br>

    <input type="submit" value="Submit Feedback">
  </form>
</body>
</html>
```

---

## ▶️ Expected result

Opening the file in a browser shows a personal webpage with: a main heading, an "About Me"
paragraph, a **bulleted** hobbies list, a **numbered** goals list, and a feedback form with
name, email, and rating fields. Submitting with a required field empty triggers the browser's
built-in validation message — proving the `required` attributes work.

---

## ☑️ Definition of done

- [ ] An `index.html` file exists with a valid `<!DOCTYPE html>` and `<head>` (charset, viewport, title)
- [ ] An `<h1>` and an "About Me" `<h2>` with an introductory `<p>`
- [ ] An **unordered** list (`<ul>`) of hobbies and an **ordered** list (`<ol>`) of goals
- [ ] A `<form>` with labelled `name`, `email`, and `rating` inputs plus a submit button
- [ ] Every `<input>` is paired with a `<label>` via matching `for`/`id`, and required fields use `required`
- [ ] The page opens in a browser and the form's required-field validation works

---

## 🔑 Key concepts

- **Document structure** — every HTML page needs a doctype, an `<html lang>` root, a `<head>`
  with metadata, and a `<body>` for visible content.
- **Headings are an outline, not styling** — use `<h1>`–`<h3>` in order to describe the page's
  hierarchy; size is a CSS concern, not a reason to pick a heading level.
- **Lists communicate meaning** — `<ul>` for unordered items (bullets), `<ol>` for sequence or
  priority (auto-numbered); never fake numbering by hand.
- **Forms and accessibility** — associate each `<input>` with a `<label>` (matching `for`/`id`),
  pick the correct `type`, group radios by a shared `name`, and use `required` for built-in validation.
- **Semantic HTML first** — choosing the right element for the job makes pages accessible,
  search-friendly, and easy to style and maintain later.
