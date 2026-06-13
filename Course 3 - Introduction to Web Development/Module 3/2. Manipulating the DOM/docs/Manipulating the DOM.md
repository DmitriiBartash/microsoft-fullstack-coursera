# Manipulating the DOM

**Course 3 — Introduction to Web Development** · Module 3 · Lesson 2 · `You Try It!`

> Build a **Quote of the Day** feature with vanilla JavaScript. Each time the user clicks
> a button, you **select** the right elements, pick a random quote, and **update the page
> live** — no reload. The whole point is the DOM trio: *select → respond to events →
> change content*.

---

## 🎯 Objective

Use JavaScript to manipulate the DOM by **selecting** HTML elements, **updating content
dynamically**, and **responding to user events** — so that clicking a button swaps the text
of a `<p>` element to a new quote without refreshing the page.

---

## 🗂️ What you will build

A tiny two-file web page: a static HTML document plus a script that brings it to life.

| File         | Responsibility                                                        |
| ------------ | --------------------------------------------------------------------- |
| `index.html` | Markup: a heading, a `<p id="quoteDisplay">`, and a `<button>`        |
| `script.js`  | Behaviour: select elements, hold the quotes, handle the click event   |

**Flow:** `click button → addEventListener fires → Math.random() picks a quote → quoteDisplay.textContent updated`

---

## ✅ Prerequisites

- A code editor (Visual Studio Code)
- A modern web browser (Chrome, Edge, or Firefox) to open the page
- Recommended: the VS Code **Live Server** extension for instant reload (optional — you can also double-click the file)
- Basic familiarity with HTML elements and the `id` attribute

---

## 🛠️ Steps

### Step 1 — Set up your environment

Create a fresh folder for this activity and open it in Visual Studio Code. You will add two
files side by side, `index.html` and `script.js`.

> The two files must live in the **same folder** so that `<script src="script.js">`
> resolves correctly.

### Step 2 — HTML setup

Create `index.html` with a simple structure: a heading, a paragraph to hold the quote, and a
button to request a new one. Give the paragraph and the button **`id` attributes** so the
script can find them, and load the script at the end of `<body>`.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Quote of the Day</title>
</head>
<body>
    <h1 id="quoteHeader">Quote of the Day</h1>
    <p id="quoteDisplay">Click the button to see today's quote!</p>
    <button id="newQuoteButton">Get New Quote</button>
    <script src="script.js"></script>
</body>
</html>
```

### Step 3 — JavaScript setup

Create `script.js` and complete three tasks:

1. **Select HTML elements** — use `getElementById` to grab the paragraph and the button.
2. **Create an array of quotes** — define at least three quotes.
3. **Add an event listener** — on each button click, pick a random quote and write it into
   the paragraph with `textContent`.

```javascript
// Select HTML Elements
const quoteDisplay = document.getElementById("quoteDisplay");
const newQuoteButton = document.getElementById("newQuoteButton");

// Create an Array of Quotes
const quotes = [
  "The only limit to our realization of tomorrow is our doubts of today. – Franklin D. Roosevelt",
  "In the middle of every difficulty lies opportunity. – Albert Einstein",
  "Success usually comes to those who are too busy to be looking for it. – Henry David Thoreau"
];

// Add an Event Listener
newQuoteButton.addEventListener("click", () => {
  const randomIndex = Math.floor(Math.random() * quotes.length);
  quoteDisplay.textContent = quotes[randomIndex];
});
```

### Step 4 — Run it

Open `index.html` in your browser (double-click it, or right-click in VS Code and choose
**Open with Live Server**). Click **Get New Quote** a few times.

---

## ▶️ Expected result

The page loads showing *"Click the button to see today's quote!"*. Each click on **Get New
Quote** replaces the paragraph text with a randomly chosen quote from the array — the content
updates instantly, with no page reload.

---

## ☑️ Definition of done

- [ ] `index.html` and `script.js` exist in the **same folder**
- [ ] The paragraph has `id="quoteDisplay"` and the button has `id="newQuoteButton"`
- [ ] `script.js` selects both elements with `getElementById`
- [ ] An array holds **at least three** quotes
- [ ] A `click` event listener updates `quoteDisplay.textContent` with a random quote
- [ ] Clicking the button in the browser shows a new quote each time, without reloading

---

## 🔑 Key concepts

- **Selecting elements** — `document.getElementById("...")` returns a live reference to a DOM
  node, the entry point for reading or changing it from JavaScript.
- **Event-driven UI** — `addEventListener("click", handler)` runs your callback every time the
  user clicks, decoupling *what happens* from *when it happens*.
- **Updating content with `textContent`** — assigning to `textContent` safely replaces the
  text of an element (it does not parse HTML), which is exactly what you want for plain quotes.
- **Randomized selection** — `Math.floor(Math.random() * quotes.length)` yields a valid index
  from `0` to `length − 1`, picking any quote in the array uniformly.
- **Script placement** — loading `<script>` at the end of `<body>` guarantees the elements
  exist before the script queries them, so `getElementById` never returns `null`.
