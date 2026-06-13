# Create a Responsive Web Page Using CSS

**Course 3 — Introduction to Web Development** · Module 2 · `You Try It!`

> Build a **responsive personal portfolio webpage** from scratch. You will lay out a header,
> nav bar, main content, sidebar, and footer with semantic HTML, then make it adapt to any
> screen using **CSS Flexbox** and **media queries** — so the layout reflows and stacks
> vertically on phones (under 768px).

---

## 🎯 Objective

Create a single-page portfolio that uses semantic HTML5 structure and modern CSS layout
(**Flexbox**, with **Grid** available for more complex sections) plus **media queries**, so the
page looks polished on desktop and gracefully **stacks vertically** on small screens.

---

## 🗂️ What you will build

A two-file static site:

| File          | Responsibility                                                        |
| ------------- | -------------------------------------------------------------------- |
| `index.html`  | Semantic structure — `header`, `nav`, `main` (`section` + `aside`), `footer` |
| `styles.css`  | Reset, typography, Flexbox layout, and a `max-width: 768px` media query |

**Flow:** `index.html  →  links  →  styles.css  →  Flexbox layout  →  media query  →  responsive page`

---

## ✅ Prerequisites

- A code editor (Visual Studio Code recommended)
- A modern web browser (Chrome, Edge, or Firefox)
- Basic familiarity with HTML tags and CSS selectors
- No build tools or installs — these are plain `.html` and `.css` files

---

## 🛠️ Steps

### Step 1 — Create a new HTML file

- Select **New File**.
- Name the file `index.html`.
- Press **Enter**, then select **OK**.

### Step 2 — Build the HTML structure

Create a structured HTML document with these semantic elements:

- **Header** — a `<header>` section with your name and tagline.
- **Navigation bar** — a `<nav>` element with three links (About, Projects, Contact).
- **Main content** — a `<main>` section containing:
  - A `<section>` with a heading and a paragraph introducing yourself.
  - An `<aside>` with your top skills.
- **Footer** — a `<footer>` containing copyright text.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Dima's Portfolio</title>
    <link rel="stylesheet" href="styles.css" />
</head>
<body>
    <header>
        <div class="header-container">
            <h1>Dima</h1>
            <p>Aspiring Web Developer &amp; Tech Enthusiast</p>
        </div>
    </header>
    <nav>
        <a href="#about">About</a>
        <a href="#projects">Projects</a>
        <a href="#contact">Contact</a>
    </nav>
    <main>
        <section>
            <h2 id="about">About Me</h2>
            <p>
                Hello! I'm Dima, a passionate learner with a growing interest in full-stack development.
                I enjoy exploring modern web technologies and continuously improving my skills by building fun and
                useful projects.
            </p>
        </section>
        <aside>
            <h3>Top Skills</h3>
            <ul>
                <li>HTML &amp; CSS</li>
                <li>JavaScript</li>
                <li>C# / ASP.NET Core</li>
                <li>Git &amp; GitHub</li>
                <li>Problem Solving</li>
            </ul>
        </aside>
    </main>
    <footer>
        <p>&copy; 2025 Dima. All rights reserved.</p>
    </footer>
</body>
</html>
```

### Step 3 — Apply CSS for layout and responsiveness

Create a `styles.css` file and link it to your HTML document (the `<link>` in `<head>` already
does this). Use the following CSS concepts:

- **Basic styling** — reset margins and padding with the `*` selector and set a consistent font
  for the whole page using `box-sizing: border-box`.
- **Layout styling** — use **Flexbox** to align and space elements in the header, navigation, and
  main sections; reach for **Grid** if a section needs a more complex layout.
- **Media queries** — ensure the layout adapts for smaller screens (less than `768px`) and **stack
  elements vertically** (like the sidebar) when the screen is small.

```css
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
}
body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background-color: #f5f7fa;
    color: #333;
    line-height: 1.6;
}
header {
    background-color: #2c3e50;
    color: #ffffff;
    padding: 40px 20px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}
.header-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
}
nav {
    background-color: #1f1f1f;
    display: flex;
    justify-content: center;
    padding: 12px 0;
    gap: 30px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, 0.05);
}
nav a {
    color: white;
    text-decoration: none;
    font-weight: bold;
    transition: color 0.3s ease;
    padding: 6px 10px;
}
nav a:hover {
    color: #4a90e2;
}
main {
    display: flex;
    gap: 24px;
    padding: 40px 20px;
    max-width: 1200px;
    margin: auto;
}
section {
    flex: 3;
    background-color: #ffffff;
    padding: 24px;
    border-radius: 12px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.03);
}
aside {
    flex: 1;
    background-color: #f0f4ff;
    padding: 24px;
    border-radius: 12px;
    border: 1px solid #d3e0f0;
}
footer {
    background-color: #1f1f1f;
    color: white;
    text-align: center;
    padding: 20px 10px;
    font-size: 0.9rem;
    margin-top: 60px;
}
ul {
    padding-left: 15px;
}
@media (max-width: 768px) {
    .header-container {
        text-align: center;
    }
    nav {
        flex-direction: column;
        gap: 12px;
        padding: 10px 16px;
    }
    nav a {
        padding: 10px 0;
    }
    main {
        flex-direction: column;
        padding: 30px 16px;
    }
    section,
    aside {
        width: 100%;
    }
}
```

### Step 4 — Test your page

- Open your HTML document in the browser (double-click `index.html`, or use a live-server extension).
- Resize the browser window to test the responsiveness.
- Ensure the content **stacks vertically** and remains readable on small screens.

---

## ▶️ Expected result

On a wide screen the `section` and `aside` sit **side by side** (a 3:1 Flexbox split) under a
centered header and a horizontal nav bar. Narrow the window below **768px** and the nav switches to
a vertical stack, the main area collapses to a single column, and the sidebar drops beneath the
intro — the page stays readable at any width.

---

## ☑️ Definition of done

- [ ] `index.html` contains semantic `header`, `nav`, `main` (`section` + `aside`), and `footer`
- [ ] The viewport `<meta>` tag is present and `styles.css` is linked from `<head>`
- [ ] `styles.css` resets margins/padding with `*` and sets `box-sizing: border-box`
- [ ] Header, nav, and main use **Flexbox** for alignment and spacing
- [ ] A `@media (max-width: 768px)` query stacks nav and main **vertically**
- [ ] Resizing the browser confirms the layout reflows and stays readable on mobile

---

## 🔑 Key concepts

- **Semantic HTML** — `header`, `nav`, `main`, `section`, `aside`, and `footer` describe the page's
  meaning, improving accessibility and SEO over generic `<div>`s.
- **Flexbox for layout** — `display: flex` with `flex: 3` / `flex: 1` gives a fluid two-column split,
  while `justify-content` and `gap` handle alignment and spacing without floats or margins hacks.
- **Mobile-first responsiveness** — the `viewport` meta tag plus a `max-width: 768px` media query let
  the same markup reflow from desktop to phone by switching `flex-direction` to `column`.
- **CSS reset + `box-sizing`** — zeroing default margins/padding and using `border-box` makes sizing
  predictable so padding and borders never blow out an element's width.
- **Separation of concerns** — structure lives in `index.html` and presentation in `styles.css`,
  linked via `<link rel="stylesheet">`, keeping content and styling independently maintainable.
