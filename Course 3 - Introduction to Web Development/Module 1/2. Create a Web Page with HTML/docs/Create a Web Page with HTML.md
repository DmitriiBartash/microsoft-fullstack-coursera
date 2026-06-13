# Create a Web Page with HTML

**Course 3 — Introduction to Web Development** · Module 1 · Lesson 2 · `You Try It!`

> Build a simple **Personal Web Page** from scratch using only HTML. You'll combine
> headings, paragraphs, ordered and unordered **lists**, an external **link**, and embedded
> **media** (image, video, and audio) into one well-structured document that opens in any
> browser — no frameworks, no build tools, just `.html`.

---

## 🎯 Objective

Practice the core building blocks of HTML by authoring a single, valid web page that
demonstrates document structure, text content, lists, hyperlinks, and embedded media —
the foundation every web developer builds on.

---

## 🧱 What you will build

A single file named **`index.html`** describing a personal homepage with these sections:

| Section            | HTML used                                  | Purpose                                   |
| ------------------ | ------------------------------------------ | ----------------------------------------- |
| Page shell         | `<!DOCTYPE html>`, `<html>`, `<head>`, `<body>` | Valid document skeleton + metadata   |
| About Me           | `<h1>`, `<h2>`, `<p>`                       | Headings and a paragraph of text          |
| My Hobbies         | `<ul>` / `<li>`                            | **Unordered** (bulleted) list             |
| My Goals           | `<ol>` / `<li>`                            | **Ordered** (numbered) list               |
| Learn More         | `<a href>`                                 | External hyperlink (opens in a new tab)   |
| Favorite Board Game| `<img>`                                    | Embedded image                            |
| Demo Video         | `<video>` + `<source>`                     | Embedded video with controls              |
| Favorite Track     | `<audio>` + `<source>`                     | Embedded audio with controls              |

---

## ✅ Prerequisites

- A plain-text or code editor (Visual Studio Code recommended)
- Any modern web browser (Chrome, Edge, Firefox, Safari)
- A project folder to hold your files, with two subfolders for assets:
  - `images/` — for your picture (e.g. `boardgame.jpg`)
  - `media/` — for your video and audio (e.g. `demo.mp4`, `music.mp3`)

> No internet connection or installation is required — HTML runs directly in the browser.

---

## 🛠️ Steps

### Step 1 — Set up the document skeleton

Create a file called `index.html` and start with the standard HTML5 boilerplate. The
`<!DOCTYPE html>` declaration tells the browser to use modern (standards) rendering, and the
two `<meta>` tags set the character encoding and make the page responsive on mobile devices.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Dima's Webpage</title>
</head>
<body>
    <!-- Page content goes here -->
</body>
</html>
```

### Step 2 — Add headings and an introductory paragraph

Inside `<body>`, add a top-level heading (`<h1>`) for the page title, a section heading
(`<h2>`), and a `<p>` paragraph introducing yourself. Use exactly one `<h1>` per page and
keep the heading levels in order (`h1` → `h2` → `h3`).

```html
    <!-- Headings and Paragraph -->
    <h1>Dima's Webpage</h1>
    <h2>About Me</h2>
    <p>
        Hello! I'm Dima, a passionate learner and aspiring web developer.
        I enjoy building websites, learning new technologies, and exploring the world of programming.
    </p>
```

### Step 3 — Add an unordered list of hobbies

An **unordered list** (`<ul>`) renders bullet points. Each item is a list item (`<li>`).

```html
    <!-- Unordered List: Hobbies -->
    <h3>My Hobbies</h3>
    <ul>
        <li>Programming</li>
        <li>Reading tech articles</li>
        <li>Playing video games</li>
        <li>Traveling</li>
        <li>Playing board games</li>
    </ul>
```

### Step 4 — Add an ordered list of goals

An **ordered list** (`<ol>`) numbers its items automatically — perfect for ranked or
sequential content.

```html
    <!-- Ordered List: Goals -->
    <h3>My Goals</h3>
    <ol>
        <li>Become a full-stack developer</li>
        <li>Create a personal portfolio website</li>
        <li>Contribute to open-source projects</li>
        <li>Work at a leading tech company</li>
    </ol>
```

### Step 5 — Add an external link

The anchor element (`<a>`) creates a hyperlink. The `href` attribute is the destination, and
`target="_blank"` opens it in a new browser tab.

```html
    <!-- External Link -->
    <h3>Learn More</h3>
    <p>
        Visit <a href="https://developer.mozilla.org/" target="_blank">MDN Web Docs</a> to learn more about HTML and web
        development.
    </p>
```

### Step 6 — Embed an image

The `<img>` element embeds a picture. `src` points to the image file, `alt` provides
descriptive text for screen readers and broken-image fallback, and `width` sets the display
size in pixels. Place your image in the `images/` folder.

```html
    <!-- Image -->
    <h3>My Favorite Board Game</h3>
    <img src="images/boardgame.jpg" alt="A fun board game setup on a table" width="400">
```

### Step 7 — Embed a video and an audio clip

The `<video>` and `<audio>` elements play media inline. The `controls` attribute shows the
play/pause UI, and the nested `<source>` element declares the file and its MIME `type`. The
text inside each element is the fallback shown if the browser can't play that format.

```html
    <!-- Video -->
    <h3>Watch My Demo Video</h3>
    <video width="400" controls>
        <source src="media/demo.mp4" type="video/mp4">
        Your browser does not support the video tag.
    </video>

    <!-- Audio -->
    <h3>Listen to My Favorite Track</h3>
    <audio controls>
        <source src="media/music.mp3" type="audio/mpeg">
        Your browser does not support the audio element.
    </audio>
```

### Step 8 — Save and open in the browser

Save `index.html`, then double-click it (or right-click → **Open with** your browser). The
page loads instantly — there's nothing to compile.

---

## 📄 Full page

Putting every section together inside the skeleton gives you the complete `index.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Dima's Webpage</title>
</head>
<body>
    <!-- Headings and Paragraph -->
    <h1>Dima's Webpage</h1>
    <h2>About Me</h2>
    <p>
        Hello! I'm Dima, a passionate learner and aspiring web developer.
        I enjoy building websites, learning new technologies, and exploring the world of programming.
    </p>
    <!-- Unordered List: Hobbies -->
    <h3>My Hobbies</h3>
    <ul>
        <li>Programming</li>
        <li>Reading tech articles</li>
        <li>Playing video games</li>
        <li>Traveling</li>
        <li>Playing board games</li>
    </ul>
    <!-- Ordered List: Goals -->
    <h3>My Goals</h3>
    <ol>
        <li>Become a full-stack developer</li>
        <li>Create a personal portfolio website</li>
        <li>Contribute to open-source projects</li>
        <li>Work at a leading tech company</li>
    </ol>
    <!-- External Link -->
    <h3>Learn More</h3>
    <p>
        Visit <a href="https://developer.mozilla.org/" target="_blank">MDN Web Docs</a> to learn more about HTML and web
        development.
    </p>
    <!-- Image -->
    <h3>My Favorite Board Game</h3>
    <img src="images/boardgame.jpg" alt="A fun board game setup on a table" width="400">
    <!-- Video -->
    <h3>Watch My Demo Video</h3>
    <video width="400" controls>
        <source src="media/demo.mp4" type="video/mp4">
        Your browser does not support the video tag.
    </video>
    <!-- Audio -->
    <h3>Listen to My Favorite Track</h3>
    <audio controls>
        <source src="media/music.mp3" type="audio/mpeg">
        Your browser does not support the audio element.
    </audio>
</body>
</html>
```

---

## ▶️ Expected result

The browser shows a single page titled **"Dima's Webpage"** with: a large heading and an
"About Me" paragraph, a bulleted **Hobbies** list, a numbered **Goals** list, a clickable
**MDN Web Docs** link that opens in a new tab, and three media blocks — an image, a video
player, and an audio player, each with playback controls. The browser tab reads
*Dima's Webpage* (from the `<title>`).

---

## ☑️ Definition of done

- [ ] `index.html` opens in the browser with no errors and the tab title shows *Dima's Webpage*
- [ ] Page has one `<h1>` plus `<h2>`/`<h3>` section headings and an intro paragraph
- [ ] Hobbies render as a bulleted `<ul>` and Goals as a numbered `<ol>`
- [ ] The MDN link works and opens in a new tab
- [ ] The `<img>` displays (or shows its `alt` text if the file is missing)
- [ ] The `<video>` and `<audio>` players appear with working controls

---

## 🔑 Key concepts

- **Document structure** — every page needs the `<!DOCTYPE html>` declaration plus
  `<html>`, `<head>`, and `<body>`; the `<head>` holds metadata (charset, viewport, title)
  while the `<body>` holds visible content.
- **Semantic headings** — use a single `<h1>` and nest heading levels in order to give the
  page a clear, accessible outline.
- **Lists** — `<ul>` produces bullets for unordered items, `<ol>` produces numbers for
  ordered/sequential items; both wrap individual `<li>` elements.
- **Links and the `target` attribute** — `<a href="…">` navigates; `target="_blank"` opens
  the destination in a new tab.
- **Embedded media with fallbacks** — `<img>` needs `alt` text for accessibility, and
  `<video>`/`<audio>` use a nested `<source>` (with a MIME `type`) plus fallback text for
  browsers that can't play the format.
