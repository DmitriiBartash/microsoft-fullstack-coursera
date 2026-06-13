# Managing Data with JSON

**Course 3 — Introduction to Web Development** · Module 3 · Lesson 5 · `You Try It!`

> Build a small web page that **fetches, parses, manipulates, and stores** JSON data with
> plain JavaScript. You will pull users from a public API, hand-parse a JSON string with
> `JSON.parse()`, serialize an object with `JSON.stringify()`, and persist settings in
> `localStorage` — the everyday JSON skills behind dynamic, data-driven web apps.

---

## 🎯 Objective

Learn to work with JSON end-to-end in the browser: fetch data from a REST API, convert
between JSON strings and JavaScript objects, render the results into the DOM, and use
`localStorage` for persistence across page loads.

---

## 🗂️ What you will build

A single static page (`index.html` + `styles.css` + `script.js`) with four sections, each
populated by JavaScript:

| File          | Responsibility                                                            |
| ------------- | ------------------------------------------------------------------------- |
| `index.html`  | Page structure: a heading and four `<div>` containers by ID               |
| `styles.css`  | Dark-theme card styling for the sections                                  |
| `script.js`   | All JSON logic: parse, stringify, `fetch`, `localStorage`, DOM rendering  |

**Flow:** `JSON string → JSON.parse() → objects → DOM` · `object → JSON.stringify() → localStorage` · `fetch(API) → response.json() → DOM`

---

## ✅ Prerequisites

- Visual Studio Code with the **Live Server** extension (the "Go Live" button)
- A modern browser (the Fetch API and `localStorage` are built in)
- Basic HTML/CSS/JavaScript familiarity — no packages to install

---

## 🛠️ Steps

### Step 1 — Create a new HTML file

In the VS Code Explorer:

- Select **File**
- Select **New File...**
- Name it `index.html`
- Press **Enter**, then select **OK**

### Step 2 — Create the HTML structure

Use `<!DOCTYPE html>` for HTML5 and add `<html>`, `<head>`, and `<body>`. Inside `<head>`
include a `<title>` and a `<meta charset="UTF-8">`. Inside `<body>` add an `<h1>` titled
**Users List**, four section `<div>`s (including one with the ID `users-container`), and a
`<script>` linking `script.js`.

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Users Page</title>
  <link rel="stylesheet" href="styles.css">
</head>
<body>
  <h1>Users List</h1>

  <div class="section">
    <h2>Local Users (Alice &amp; Bob)</h2>
    <div id="local-users"></div>
  </div>

  <div class="section">
    <h2>Charlie (from Object)</h2>
    <div id="charlie-data"></div>
  </div>

  <div class="section">
    <h2>Settings</h2>
    <div id="settings-data"></div>
  </div>

  <div class="section">
    <h2>Fetched Users</h2>
    <div id="users-container"></div>
  </div>

  <script src="script.js"></script>
</body>
</html>
```

### Step 3 — Add the styles (optional)

Create `styles.css` to give the page a dark theme with rounded section cards.

```css
body {
  font-family: Arial, sans-serif;
  background-color: #1e1e2f;
  color: #f0f0f0;
  margin: 0;
  padding: 20px;
}
h1 {
  color: #61dafb;
  text-align: center;
  margin-bottom: 30px;
}
.section {
  background-color: #2a2a40;
  padding: 20px;
  border-radius: 10px;
  margin-bottom: 20px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
}
.section h2 {
  color: #ffd700;
  margin-top: 0;
}
p {
  margin: 6px 0;
}
#users-container p,
#local-users p,
#charlie-data p,
#settings-data p {
  padding-left: 10px;
}
```

### Step 4 — Create a new JavaScript file

- Select **File**
- Select **New File...**
- Name it `script.js`
- Press **Enter**, then select **OK**

### Step 5 — Select the DOM containers

At the top of `script.js`, grab the four `<div>`s by ID so the rest of the code can render
into them.

```js
// DOM element selection
const usersContainer = document.getElementById('users-container');
const localUsersContainer = document.getElementById('local-users');
const charlieContainer = document.getElementById('charlie-data');
const settingsContainer = document.getElementById('settings-data');
```

### Step 6 — Simulate and parse JSON data

Create a JSON **string** with two users — `Alice` (age 25) and `Bob` (age 30) — then turn it
into an array of objects with `JSON.parse()`. Log Alice's name and Bob's age, then render
both users into the page.

```js
// Local JSON string and parsing
const jsonString = `
[
  { "name": "Alice", "age": 25 },
  { "name": "Bob", "age": 30 }
]
`;

const localUsers = JSON.parse(jsonString);
console.log("Alice's name:", localUsers[0].name);
console.log("Bob's age:", localUsers[1].age);

// Display Alice and Bob
localUsers.forEach(user => {
  const p = document.createElement('p');
  p.textContent = `${user.name}, Age: ${user.age}`;
  localUsersContainer.appendChild(p);
});
```

### Step 7 — Convert a JavaScript object to JSON

Create an object for `Charlie` (age 28, `isActive: true`), serialize it with
`JSON.stringify()`, log the JSON string, then parse it back and render it.

```js
// Object creation and conversion to JSON
const userCharlie = {
  name: "Charlie",
  age: 28,
  isActive: true
};

const charlieJSON = JSON.stringify(userCharlie);
console.log("Charlie as JSON:", charlieJSON);

// Display Charlie
const charlieData = JSON.parse(charlieJSON);
const charlieInfo = document.createElement('p');
charlieInfo.textContent = `${charlieData.name}, Age: ${charlieData.age}, Active: ${charlieData.isActive}`;
charlieContainer.appendChild(charlieInfo);
```

### Step 8 — Fetch and display data from an API

Call `fetch` on `https://jsonplaceholder.typicode.com/users`. Check `response.ok`, parse the
body with `response.json()`, then render each user's name and email into `users-container`.
If the request fails, log the error to the console.

```js
// Fetching and displaying external API data
fetch('https://jsonplaceholder.typicode.com/users')
  .then(response => {
    if (!response.ok) {
      throw new Error('Network response was not OK');
    }
    return response.json();
  })
  .then(users => {
    users.forEach(user => {
      const userElement = document.createElement('p');
      userElement.textContent = `${user.name} (${user.email})`;
      usersContainer.appendChild(userElement);
    });
  })
  .catch(error => {
    console.error('Failed to fetch users:', error);
  });
```

### Step 9 — Store and retrieve data with localStorage

Create a `settings` object (`theme: "dark"`, `language: "en"`), store it as a JSON string
with `localStorage.setItem`, then read it back with `getItem` + `JSON.parse()` and render it.

```js
// LocalStorage: storing and retrieving settings
const settings = {
  theme: "dark",
  language: "en"
};

localStorage.setItem("settings", JSON.stringify(settings));
const savedSettings = JSON.parse(localStorage.getItem("settings"));
console.log("Theme:", savedSettings.theme);
console.log("Language:", savedSettings.language);

// Display settings
const themePara = document.createElement('p');
themePara.textContent = `Theme: ${savedSettings.theme}`;
settingsContainer.appendChild(themePara);

const languagePara = document.createElement('p');
languagePara.textContent = `Language: ${savedSettings.language}`;
settingsContainer.appendChild(languagePara);
```

### Step 10 — Run your code

- Click **Go Live** (lower-right of the lab) to start Live Server.
- A new tab opens and displays your webpage.
- If the page does not behave as expected, compare against the complete code above.

---

## ▶️ Expected result

The page shows four populated sections: **Local Users** (Alice and Bob), **Charlie** with his
active flag, **Settings** (theme and language read back from `localStorage`), and **Fetched
Users** — ten name/email lines pulled live from the placeholder API. The browser console also
logs Alice's name, Bob's age, Charlie's JSON, and the saved theme/language.

---

## ☑️ Definition of done

- [ ] `index.html` has an `<h1>` and four section `<div>`s, including `users-container`, plus a `<script>` to `script.js`
- [ ] `JSON.parse()` turns the Alice/Bob JSON string into objects, and both render in **Local Users**
- [ ] `JSON.stringify()` serializes the Charlie object, and the round-tripped data renders
- [ ] `fetch` loads the API, checks `response.ok`, and renders each user's name and email
- [ ] A failed fetch logs an error via `.catch`
- [ ] `settings` is saved to `localStorage` as JSON, read back, parsed, and rendered
- [ ] Clicking **Go Live** shows all four sections populated

---

## 🔑 Key concepts

- **JSON is text, objects are live** — `JSON.parse()` deserializes a string into JavaScript
  values; `JSON.stringify()` does the reverse. The network and `localStorage` only move strings.
- **`fetch` returns promises** — chain `.then(response => response.json())` to read the body,
  guard with `response.ok`, and always add `.catch` so network errors surface instead of failing silently.
- **`localStorage` stores strings only** — stringify before `setItem`, parse after `getItem`;
  the data survives page reloads, making it ideal for user settings.
- **Render from data, not by hand** — building DOM nodes in a `forEach` over parsed data keeps
  the UI in sync with the underlying objects and scales to any list length.
