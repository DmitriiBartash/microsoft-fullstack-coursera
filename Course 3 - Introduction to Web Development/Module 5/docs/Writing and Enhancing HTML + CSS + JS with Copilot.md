# Writing and Enhancing HTML + CSS + JS with Copilot

**Course 3 — Introduction to Web Development** · Module 5 · `You Try It!`

> Build a complete, responsive **portfolio website** with vanilla HTML, CSS, and
> JavaScript — using **Microsoft Copilot** as a coding assistant to generate
> semantic markup, accessible styling, and interactive behavior. You'll move
> through four activities (HTML → CSS → JavaScript → integrate & test) and finish
> with a single deployable site: `index.html`, `style.css`, and `script.js`.

---

## 🎯 Objective

Use Microsoft Copilot to **write and enhance** the three front-end layers of a portfolio
site — semantic and accessible **HTML**, responsive **CSS**, and interactive **JavaScript** —
then integrate, test, and debug them into one working project that meets accessibility (WCAG),
SEO, and cross-browser best practices.

---

## 🗂️ What you will build

A self-contained portfolio website made of three linked files plus an `images/` folder:

| File          | Layer        | Responsibility                                                            |
| ------------- | ------------ | ------------------------------------------------------------------------- |
| `index.html`  | Structure    | Semantic sections (header/nav, about, projects, skills, contact, footer)  |
| `style.css`   | Presentation | Layout, color, typography, project cards, responsive `@media` rules       |
| `script.js`   | Behavior     | Menu toggle, smooth scroll, project filter, lightbox, form validation     |

**Flow:** `index.html  →  <link> style.css  →  <script> script.js  →  interactive portfolio`

> Copilot is the assistant here: you type a tag, comment, or function name and accept
> its suggestions, then review every line for correctness, accessibility, and SEO.

---

## ✅ Prerequisites

- An IDE with Copilot enabled (e.g. **Visual Studio Code** + GitHub Copilot, or Microsoft Copilot)
- A modern browser for testing — **Edge, Chrome, and Firefox**
- The **WebAIM Contrast Checker** (or your browser's contrast tooling) for verifying WCAG color contrast
- Basic familiarity with HTML5, CSS3, and JavaScript

---

## 🛠️ Steps

This project spans four activities. Steps 1–6 build each layer; Step 7 integrates and tests the whole site.

### Step 1 — Create and structure `index.html`

Create a new file named `index.html`. Type `<!DOCTYPE html>` and let Copilot auto-complete the
boilerplate, then ask it to fill in the `<head>` (title, meta, stylesheet link) and a **semantic**
`<body>`. Use meaningful tags — `<header>`, `<nav>`, `<section>`, `<article>`, `<figure>`,
`<footer>` — which help both accessibility and SEO.

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <meta name="description"
    content="Portfolio website of a Junior Full-Stack Developer specializing in .NET & ASP.NET Core." />
  <title>Dmitrii Bartash | Junior Full-Stack Developer</title>
  <link rel="stylesheet" href="style.css" />
</head>
<body>
  <!-- Header + Navigation -->
  <header role="banner">
    <button id="menu-toggle" aria-label="Toggle navigation">
      <span class="bar"></span>
      <span class="bar"></span>
      <span class="bar"></span>
    </button>
    <nav role="navigation" aria-label="Main navigation">
      <ul>
        <li><a href="#about-me">About</a></li>
        <li><a href="#experience">Experience</a></li>
        <li><a href="#education">Education</a></li>
        <li><a href="#skills">Skills</a></li>
        <li><a href="#certifications">Certifications</a></li>
        <li><a href="#projects">Projects</a></li>
        <li><a href="#contact">Contact</a></li>
      </ul>
    </nav>
  </header>
  <!-- Sections are added in Step 2 -->
</body>
</html>
```

### Step 2 — Build the portfolio sections

Generate each `<section>` with Copilot, keeping the structure semantic. Use an `<h2>` per section,
`<article>` cards for entries, `<ul>`/`<li>` for skills, and a `<form>` for contact.

```html
  <!-- About Me -->
  <section id="about-me">
    <h2>About Me</h2>
    <p><strong>Dmitrii Bartash</strong> – Junior Full-Stack Developer based in Chișinău, Moldova.</p>
    <p>Aspiring software developer with hands-on experience designing and building full-stack web
      applications and distributed backend systems. Skilled in clean code, system architecture,
      and cloud integration.</p>
    <p><strong>Email:</strong> <span class="email-obfuscated">moc.liamg@hsatrab.iirtimd</span></p>
    <p><strong>GitHub:</strong>
      <a href="https://github.com/DmitriiBartash" target="_blank" rel="noopener noreferrer">github.com/DmitriiBartash</a> |
      <strong>LinkedIn:</strong>
      <a href="https://www.linkedin.com/in/dmitrii-bartash" target="_blank" rel="noopener noreferrer">linkedin.com/in/dmitrii-bartash</a>
    </p>
  </section>

  <!-- Experience (repeat <article> per role) -->
  <section id="experience">
    <h2>Experience</h2>
    <article>
      <h3>Biocond S.R.L – Intern Full-Stack Developer (Graduation Project)</h3>
      <p><em>01/2024 – 06/2025 | Chișinău, Moldova</em></p>
      <ul>
        <li>Designed a microservice-based management system for climate equipment installation</li>
        <li>Built backend with ASP.NET Core and FastAPI</li>
        <li>Integrated PostgreSQL, MongoDB, Redis</li>
        <li>Enabled REST, WebSocket, and RabbitMQ communication</li>
        <li>Containerized services with Docker for consistent deployment</li>
      </ul>
    </article>
  </section>

  <!-- Skills (styled as pills via CSS) -->
  <section id="skills">
    <h2>Skills</h2>
    <ul>
      <li><strong>Programming Languages:</strong> C#, Python, JavaScript, SQL</li>
      <li><strong>.NET Frameworks & Tools:</strong> ASP.NET Core, ASP.NET Web API, ASP.NET MVC,
        Razor Pages, Blazor, Entity Framework Core, LINQ</li>
      <li><strong>Databases & Storage:</strong> PostgreSQL, Microsoft SQL Server, SQLite, MongoDB, Redis</li>
      <li><strong>DevOps & Infrastructure:</strong> Docker, Docker Compose, CI/CD, Git</li>
    </ul>
  </section>

  <!-- Projects: filter buttons + cards + a reusable lightbox modal -->
  <section id="projects">
    <h2>Projects</h2>
    <div id="project-filters">
      <button data-category="all" class="active">All</button>
      <button data-category="web">Web</button>
      <button data-category="api">API</button>
      <button data-category="tool">Tool</button>
    </div>
    <div class="project-card" data-category="web">
      <h3>Personal Portfolio</h3>
      <p>A responsive portfolio built with HTML, CSS, and JavaScript.</p>
      <img src="images/portfolio.png" alt="Portfolio screenshot" />
    </div>
    <div class="project-card" data-category="api">
      <h3>Weather API</h3>
      <p>REST API built with ASP.NET Core and OpenWeather integration.</p>
      <img src="images/weather.png" alt="Weather API screenshot" />
    </div>
    <div class="project-card" data-category="tool">
      <h3>BTU Calculator</h3>
      <p>Thermal load estimation tool built with FastAPI and a JS frontend.</p>
      <img src="images/btu.png" alt="BTU Calculator screenshot" />
    </div>
  </section>

  <!-- Lightbox modal (hidden until an image is clicked) -->
  <div id="lightbox" class="lightbox" style="display: none;">
    <span id="lightbox-close">×</span>
    <img id="lightbox-img" src="" alt="Project Preview" />
  </div>

  <!-- Contact: labelled form for accessibility -->
  <section id="contact">
    <h2>Contact Me</h2>
    <form class="contact-form" action="#" method="post">
      <label for="name">Your Name</label>
      <input type="text" id="name" name="name" placeholder="Enter your name" required />
      <label for="email">Your Email</label>
      <input type="email" id="email" name="email" placeholder="Enter your email" required />
      <label for="message">Your Message</label>
      <textarea id="message" name="message" rows="5" placeholder="Type your message here..." required></textarea>
      <button type="submit">Send Message</button>
    </form>
  </section>

  <footer>
    <p>&copy; 2025 Dmitrii Bartash. All rights reserved.</p>
  </footer>
  <script src="script.js"></script>
```

### Step 3 — Add accessibility and SEO

With the structure in place, harden it for users and search engines.

- Add **`alt`** text to every `<img>`, especially in the Projects section.
- Add **ARIA roles** where they clarify intent — `role="banner"` on `<header>`, `role="navigation"` on `<nav>`.
- Ensure interactive elements (buttons, links) are reachable by **keyboard**.
- Use **WebAIM Contrast Checker** to confirm text/background contrast meets **WCAG**.
- Keep a meaningful `<title>` and `<meta name="description">`, plus a sensible heading hierarchy, for **SEO**.

### Step 4 — Create and link `style.css`

Create a new file named `style.css` and confirm it is linked from the `<head>` of `index.html`
(`<link rel="stylesheet" href="style.css" />`). Define base styles and design tokens with CSS variables.

```css
/* ========= Root Variables & Base ========= */
:root {
  --bg-color: #f9fafb;
  --text-color: #1f2937;
  --accent: #2563eb;
  --accent-dark: #1e40af;
  --card-bg: #ffffff;
  --card-border: #e5e7eb;
  --pill-bg: #eef2ff;
  --pill-text: #3730a3;
  --font-main: 'Segoe UI', Roboto, sans-serif;
  --shadow-sm: 0 1px 4px rgba(0, 0, 0, 0.06);
  --shadow-md: 0 4px 12px rgba(0, 0, 0, 0.1);
}
* { margin: 0; padding: 0; box-sizing: border-box; }
body {
  font-family: var(--font-main);
  background-color: var(--bg-color);
  color: var(--text-color);
  line-height: 1.7;
  font-size: 16px;
  padding: 0 1rem;
  scroll-behavior: smooth;
}
img { max-width: 100%; display: block; border-radius: 8px; }

header, section, footer { max-width: 1000px; margin: auto; padding: 3rem 1rem; }
```

### Step 5 — Style the sections (cards, skills, contact)

Let Copilot suggest layout for each section: a sticky gradient header, card-like project tiles using
`display: flex`, and skill "pills".

```css
/* Sticky header with gradient */
header {
  background: linear-gradient(90deg, #1f2937, #111827);
  color: #fff;
  position: sticky;
  top: 0;
  z-index: 100;
  box-shadow: var(--shadow-md);
  border-radius: 0 0 16px 16px;
}

/* Card layout for articles and project cards */
section > article,
section > div.project-card {
  background: var(--card-bg);
  border: 1px solid var(--card-border);
  border-radius: 12px;
  padding: 2rem;
  margin-bottom: 2rem;
  box-shadow: var(--shadow-sm);
  transition: transform 0.2s ease, box-shadow 0.3s ease;
}
section > article:hover,
section > div.project-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-md);
}

/* Skills rendered as pills */
#skills ul { display: flex; flex-wrap: wrap; gap: 0.6rem; list-style: none; padding-left: 0; }
#skills li {
  background-color: var(--pill-bg);
  color: var(--pill-text);
  padding: 0.5rem 0.9rem;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: 500;
}
```

### Step 6 — Make it responsive and cross-browser

Add a **media query at 768px** that collapses the nav into a **hamburger menu**, keep media fluid
with `max-width: 100%; height: auto;`, then test in Edge, Chrome, and Firefox and add vendor prefixes
where needed.

```css
/* Hamburger button is hidden on desktop, shown on mobile */
#menu-toggle { display: none; background: none; border: none; cursor: pointer; }

@media (max-width: 768px) {
  #menu-toggle { display: block; margin: 0 auto; }
  nav ul {
    display: none;            /* hidden until toggled open */
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }
  nav ul.open { display: flex; }
  h2 { font-size: 1.8rem; }
}
```

### Step 7 — Add interactivity in `script.js`, then integrate & test

Create `script.js` and link it at the **bottom** of `index.html` (`<script src="script.js"></script>`).
With Copilot, implement the menu toggle (`toggleMenu`), smooth scrolling, the project filter
(`filterProjects(category)`), an image lightbox, and contact-form validation with real-time feedback.
Then review all three files together, test on desktop/tablet/mobile across browsers, and use Copilot's
suggestions (and the browser dev-tools console) to debug.

```javascript
// ===== Toggle navigation menu (hamburger) =====
const menuToggle = document.getElementById('menu-toggle');
const navList = document.querySelector('nav ul');
menuToggle?.addEventListener('click', () => {
  navList.classList.toggle('open');
  menuToggle.classList.toggle('active');
});

// ===== Smooth scroll for internal links =====
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
  anchor.addEventListener('click', function (e) {
    e.preventDefault();
    const target = document.querySelector(this.getAttribute('href'));
    navList.classList.remove('open');
    menuToggle.classList.remove('active');
    target?.scrollIntoView({ behavior: 'smooth' });
  });
});

// ===== Filter projects by category =====
const filterButtons = document.querySelectorAll('#project-filters button');
const projectCards = document.querySelectorAll('.project-card');
filterButtons.forEach(button => {
  button.addEventListener('click', () => {
    filterProjects(button.getAttribute('data-category'));
    filterButtons.forEach(btn => btn.classList.remove('active'));
    button.classList.add('active');
  });
});
function filterProjects(category) {
  projectCards.forEach(card => {
    const match = category === 'all' || card.dataset.category === category;
    card.style.display = match ? 'block' : 'none';
  });
}

// ===== Lightbox effect for project images =====
const lightbox = document.getElementById('lightbox');
const lightboxImg = document.getElementById('lightbox-img');
const lightboxClose = document.getElementById('lightbox-close');
document.querySelectorAll('.project-card img').forEach(img => {
  img.addEventListener('click', () => {
    lightboxImg.src = img.src;
    lightbox.style.display = 'flex';
  });
});
lightboxClose.addEventListener('click', () => { lightbox.style.display = 'none'; });
lightbox.addEventListener('click', (e) => {
  if (e.target === lightbox) lightbox.style.display = 'none';
});

// ===== Contact form validation =====
const form = document.querySelector('.contact-form');
form?.addEventListener('submit', function (e) {
  e.preventDefault();
  const fields = [form.name, form.email, form.message];
  let valid = true;
  fields.forEach(input => {
    input.style.borderColor = '';
    if (!input.value.trim()) { input.style.borderColor = 'red'; valid = false; }
  });
  if (valid) { alert('Message sent successfully!'); form.reset(); }
  else { alert('Please fill in all required fields.'); }
});

// ===== Real-time input feedback =====
['name', 'email', 'message'].forEach(id => {
  const input = document.getElementById(id);
  input?.addEventListener('input', () => {
    input.style.borderColor = input.value.trim() ? 'green' : 'red';
  });
});
```

---

## ▶️ Expected result

Opening `index.html` in a browser shows a styled, single-page portfolio. The navigation collapses to
a hamburger menu below **768px**; nav links **smooth-scroll** to their sections; the Projects filter
buttons show/hide cards by category; clicking a project image opens it in a **lightbox**; and the
Contact form blocks empty submissions while giving **green/red real-time** feedback. The same behavior
holds consistently across Edge, Chrome, and Firefox.

---

## ☑️ Definition of done

- [ ] `index.html` uses semantic tags (`header`, `nav`, `section`, `article`, `footer`) with `alt` text and ARIA roles
- [ ] `style.css` is linked in the `<head>` and styles header, cards, skill pills, and the contact form
- [ ] A `@media (max-width: 768px)` query turns the nav into a working hamburger menu
- [ ] `script.js` is linked at the bottom and implements toggle, smooth scroll, `filterProjects()`, lightbox, and form validation
- [ ] Color contrast verified against WCAG (e.g. WebAIM Contrast Checker)
- [ ] The site is tested and behaves consistently in Edge, Chrome, and Firefox
- [ ] All three files are saved together for use in the final integrated project

---

## 🔑 Key concepts

- **Copilot as a pair-programmer, not an oracle** — you prompt with tags, comments, and function names,
  but you review every suggestion for correctness, accessibility, and SEO before accepting it.
- **Separation of concerns across three layers** — HTML carries *structure*, CSS carries *presentation*,
  and JavaScript carries *behavior*; keeping them in `index.html`, `style.css`, and `script.js` keeps the
  project maintainable.
- **Semantic, accessible markup is the foundation** — meaningful tags, `alt` text, ARIA roles, keyboard
  access, and WCAG-compliant contrast benefit both users and search engines (SEO).
- **Responsive design is intentional** — a `768px` breakpoint, a collapsible hamburger nav, and fluid
  media (`max-width: 100%`) make the single layout work from desktop to mobile.
- **Integrate, then test and debug** — the payoff only appears when the three files are wired together
  and exercised across devices and browsers, using dev-tools and Copilot to resolve issues.
