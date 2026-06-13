# Build a web page with asynchronous JavaScript

**Course 3 — Introduction to Web Development** · Module 3 · Lesson 3 · `You Try It!`

> Build a small demo page to understand **asynchronous programming** in JavaScript.
> Instead of hitting a real API, you'll *simulate* a delayed fetch with `setTimeout`
> and surface the same mock data three ways — a **callback**, a **Promise**, and
> **`async`/`await`** — then render the result into the DOM.

---

## 🎯 Objective

Learn the three core models of asynchronous JavaScript — **callbacks**, **Promises**, and
**`async`/`await`** (with `try`/`catch` error handling) — by wiring a button that "fetches"
local mock data after a delay and displays it on the page.

---

## 🗂️ What you will build

A tiny static site with two files served by VS Code Live Server:

| File         | Responsibility                                                        |
| ------------ | -------------------------------------------------------------------- |
| `index.html` | Page shell — heading, a **Fetch Data** button, an empty results `div` |
| `script.js`  | The async logic — callback, Promise, `async`/`await`, and DOM update  |

**Flow:** `click #fetch-data  →  fetchAndDisplayData()  →  await fetchDataWithPromise()  →  setTimeout (1s)  →  resolve(mockData)  →  #data-container.innerHTML`

---

## ✅ Prerequisites

- Visual Studio Code
- The **Live Server** extension (provides the **Go Live** button)
- A modern browser — no build tools, no `npm`, no framework

---

## 🛠️ Steps

### Step 1 — Create a new HTML file

- Select **File > New File**.
- Press **Enter** and select **OK**.
- Name it **`index.html`**.

### Step 2 — Build the HTML structure

In `index.html`, set up a basic HTML5 document. Inside the `<body>`, add:

- An `<h1>` heading with the text **"Async JavaScript Lab"**.
- A `<button>` with the ID `fetch-data` and the text **"Fetch Data"**.
- A `<div>` with the ID `data-container` to display results.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Async JavaScript Lab</title>
</head>
<body>
    <h1>Async JavaScript Lab</h1>
    <button id="fetch-data">Fetch Data</button>
    <div id="data-container"></div>

    <script src="script.js"></script>
</body>
</html>
```

> The `<script>` tag sits just before `</body>` so the button and container exist in the
> DOM by the time `script.js` runs.

### Step 3 — Create a new JavaScript file

- Select **File > New File**.
- Name it **`script.js`**.
- Press **Enter** and select **OK**.

### Step 4 — Write the asynchronous JavaScript code

You will simulate data fetching and use callbacks, Promises, and `async`/`await` with local
data and a delay via `setTimeout`.

1. **Callback function** — takes a callback, uses `setTimeout` to simulate a delay, then runs
   the callback with mock data (an array of names).
2. **Promises** — convert the callback function into one that returns a `Promise`, resolved
   with the same mock data.
3. **`async`/`await`** — use `async` and `await` to handle the Promise, then update the DOM
   with string concatenation and `.innerHTML`.
4. **Error handling** — wrap the `async`/`await` logic in `try`/`catch` and log a console
   message if an error occurs.

```javascript
// --- Mock Data ---
const mockData = ['Alice', 'Bob', 'Charlie'];

// --- 1. Callback-based Fetch ---
function fetchDataWithCallback(callback) {
    setTimeout(() => {
        callback(mockData);
    }, 1000);
}

// --- 2. Promise-based Fetch ---
function fetchDataWithPromise() {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            // Simulate success
            resolve(mockData);
            // To simulate error, uncomment the line below:
            // reject('Failed to fetch data');
        }, 1000);
    });
}

// --- 3 & 4. Async/Await + Error Handling ---
async function fetchAndDisplayData() {
    try {
        const data = await fetchDataWithPromise();
        const container = document.getElementById('data-container');
        container.innerHTML = '';
        let html = '<ul>';
        for (let name of data) {
            html += '<li>' + name + '</li>';
        }
        html += '</ul>';
        container.innerHTML = html;
    } catch (error) {
        console.error('Error fetching data:', error);
    }
}

// --- Button Click Handler ---
document.getElementById('fetch-data').addEventListener('click', () => {
    fetchAndDisplayData();
});

// Uncomment to test callback version
/*
fetchDataWithCallback((data) => {
  console.log('Callback Data:', data);
});
*/
```

### Step 5 — Run your code

- Click **Go Live** in the lower-right corner of Visual Studio Code.
- A new browser tab should open, displaying your page.
- Click the **Fetch Data** button to test your asynchronous logic.

---

## ▶️ Expected result

After clicking **Fetch Data**, the page pauses for about one second (the simulated network
delay), then `#data-container` fills with a bulleted list of the mock names —
**Alice**, **Bob**, **Charlie**. Uncomment the `reject(...)` line to watch the `catch` block
log `Error fetching data:` to the console instead.

---

## ☑️ Definition of done

- [ ] `index.html` contains the `<h1>`, a `#fetch-data` button, and an empty `#data-container`
- [ ] `script.js` defines `fetchDataWithCallback`, `fetchDataWithPromise`, and `fetchAndDisplayData`
- [ ] The Promise version resolves with `mockData` after a `setTimeout` delay
- [ ] `async`/`await` reads the Promise inside a `try`/`catch` block
- [ ] Clicking **Fetch Data** renders the names as a `<ul>` inside `#data-container`
- [ ] Uncommenting `reject(...)` triggers the `catch` and logs the error

---

## 🔑 Key concepts

- **`setTimeout` simulates async work** — it schedules the callback to run after the delay
  without blocking the page, standing in for a real network request.
- **Callbacks → Promises → `async`/`await`** — the same task expressed three ways; each step
  up flattens nesting and makes the "do this *after* that" intent read top-to-bottom.
- **`await` pauses, it doesn't block** — it suspends only the `async` function until the
  Promise settles; the rest of the page (and event loop) keeps running.
- **`try`/`catch` is how `async`/`await` handles failure** — a rejected Promise surfaces as a
  thrown error, so wrapping the `await` lets one block handle both success and failure paths.
- **Update the DOM once** — build the markup string in a variable, then assign `.innerHTML`
  a single time to render the whole list efficiently.
