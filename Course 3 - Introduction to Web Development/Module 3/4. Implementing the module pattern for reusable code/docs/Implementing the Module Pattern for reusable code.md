# Implementing the Module Pattern for reusable code

**Course 3 — Introduction to Web Development** · Module 3 · Lesson 4 · `You Try It!`

> Build a small, structured JavaScript app that puts **three classic design patterns** to
> work together: the **Module** pattern encapsulates a calculator, the **Observer** pattern
> lets components react to events, and the **Singleton** pattern keeps one shared settings
> object. Each pattern lives in `main.js` and is wired up from a single `index.html`.

---

## 🎯 Objective

Apply the **Module**, **Observer**, and **Singleton** patterns to build a structured
JavaScript application. Together these patterns let you **encapsulate** state and behaviour,
**decouple** communication between components, and keep **one consistent** source of truth for
app-wide settings.

---

## 🗂️ What you will build

A static web page driven by one script file, demonstrating each pattern with its own buttons
and output area.

| File         | Responsibility                                                            |
| ------------ | ------------------------------------------------------------------------- |
| `index.html` | Page structure + buttons that trigger each demo                           |
| `styles.css` | Visual styling for the calculator, buttons, and output boxes              |
| `main.js`    | `CalculatorModule` (Module), `Subject`/`Observer` (Observer), `Settings` (Singleton) |

**Flow:** `Button click → CalculatorModule.add/subtract → result → displayResult() → DOM`
and `Subject.notify() → each Observer.update() → output box`.

---

## ✅ Prerequisites

- Visual Studio Code with the **Live Server** extension (the **Go Live** button)
- Basic familiarity with JavaScript functions, classes, and the DOM
- A modern browser with the developer **Console** open (to watch the logging)

---

## 🛠️ Steps

### Step 1 — Create a new HTML file

In the lab editor:

- Select **File**.
- Select **New File…**.
- Name the file `index.html`.
- Press **Enter**, then select **OK**.

### Step 2 — Build the HTML structure

Add the following to `index.html`. The lab gives you a skeleton with blanks to fill in —
each `___` is replaced by the value shown in the completed version below.

**Template (fill in the blanks):**

```html
<!DOCTYPE html>
<html lang="___">
<head>
  <meta charset="___">
  <meta name="viewport" content="___">
  <title>___</title>
  <link rel="stylesheet" href="styles.css">
</head>
<body>
  <h1>___</h1>
  <div id="___"></div>
  <script src="main.js"></script>
</body>
</html>
```

**Completed structure** (with demo buttons and output boxes wired up for all three patterns):

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Module, Observer, Singleton</title>
  <link rel="stylesheet" href="styles.css">
</head>
<body>
  <h1>Calculator</h1>
  <div id="result">Result: 0</div>
  <button onclick="CalculatorModule.add(5)">Add 5</button>
  <button onclick="CalculatorModule.subtract(2)">Subtract 2</button>

  <h2>Observer Demo</h2>
  <button onclick="runObserverDemo()">Run Observer Demo</button>
  <button onclick="clearObserverOutput()">Clear Observer Output</button>
  <div id="observer-output" class="output-box">
    <div class="placeholder">No observer output yet.</div>
  </div>

  <h2>Singleton Demo</h2>
  <button onclick="runSingletonDemo()">Run Singleton Demo</button>
  <button onclick="clearSingletonOutput()">Clear Singleton Output</button>
  <div id="singleton-output" class="output-box">
    <div class="placeholder">No singleton output yet.</div>
  </div>

  <script src="main.js"></script>
</body>
</html>
```

### Step 3 — Create a new JavaScript file

- Select **File**.
- Select **New File…**.
- Name the file `main.js`.
- Press **Enter**, then select **OK**.

### Step 4 — Implement the Module Pattern (`main.js`)

The Module pattern wraps state and helper functions inside an **IIFE** (immediately-invoked
function expression) and returns only the public API. Here `result` and `displayResult()` stay
private; only `add` and `subtract` are exposed.

**Template (fill in the blanks):**

```javascript
const CalculatorModule = (function () {
  let result = ___;  // Initialize the result to 0
  function add(value) {
    result ___ value;
    displayResult();
  }
  function subtract(value) {
    result ___ value;
    displayResult();
  }
  function displayResult() {
    document.getElementById('___').textContent = `Result: ${___}`;  // Update the UI
  }
  return {
    ___,  // Expose the add function
    ___   // Expose the subtract function
  };
})();
```

**Completed implementation** (assigned to `window` so the inline `onclick` handlers can reach it):

```javascript
// --- CalculatorModule ---
window.CalculatorModule = (function () {
  let result = 0;
  function add(value) {
    result += value;
    console.log(`Added ${value}, new result: ${result}`);
    displayResult();
  }
  function subtract(value) {
    result -= value;
    console.log(`Subtracted ${value}, new result: ${result}`);
    displayResult();
  }
  function displayResult() {
    const resultElement = document.getElementById('result');
    if (resultElement) {
      resultElement.textContent = `Result: ${result}`;
    } else {
      console.warn("Element with ID 'result' not found.");
    }
  }
  return {
    add,
    subtract
  };
})();
```

### Step 5 — Implement the Observer Pattern (`main.js`)

The Observer pattern lets a `Subject` keep a list of `Observer`s and broadcast to all of them
at once. Subscribers can be added or removed at runtime, and each one decides what to do in its
own `update()` method.

**Template (fill in the blanks):**

```javascript
class Subject {
  constructor() {
    this.observers = ___;  // Initialize the observers list
  }
  subscribe(observer) {
    this.observers.___(observer);  // Add an observer
  }
  unsubscribe(observer) {
    this.observers = this.observers.filter(obs => obs !== ___);  // Remove an observer
  }
  notify() {
    this.observers.forEach(observer => observer.___());  // Notify all observers
  }
}

class Observer {
  constructor(name) {
    this.name = ___;  // Store the observer's name
  }
  update() {
    console.log(`${___} received notification!`);  // Log a notification
  }
}
```

**Completed implementation** (each observer also writes a styled message into its output box):

```javascript
// --- Observer Pattern ---
class Subject {
  constructor() {
    this.observers = [];
    console.log('Subject created.');
  }
  subscribe(observer) {
    this.observers.push(observer);
    console.log(`Subscribed: ${observer.name}`);
  }
  unsubscribe(observer) {
    this.observers = this.observers.filter(obs => obs !== observer);
    console.log(`Unsubscribed: ${observer.name}`);
  }
  notify() {
    console.log('Notifying observers...');
    this.observers.forEach(observer => observer.update());
  }
}

class Observer {
  constructor(name, outputElementId) {
    this.name = name;
    this.outputElementId = outputElementId;
    console.log(`Observer created: ${this.name}`);
  }
  update() {
    const output = document.getElementById(this.outputElementId);
    if (output) {
      const message = document.createElement('div');
      message.className = 'observer-message';
      message.textContent = `${this.name} received notification!`;
      output.appendChild(message);
      console.log(`${this.name} updated.`);
    } else {
      console.warn(`Element with ID '${this.outputElementId}' not found.`);
    }
  }
}
```

### Step 6 — Implement the Singleton Pattern (`main.js`)

The Singleton pattern guarantees exactly **one** instance of a class. The constructor checks
for an existing instance and, if found, returns it instead of creating a new one — so every
`new Settings()` call shares the same configuration object.

**Template (fill in the blanks):**

```javascript
class Settings {
  constructor() {
    if (Settings.___) {
      return ___;  // Return the existing instance
    }
    this.configuration = ___;  // Initialize the configuration object
    Settings.___ = this;  // Store the instance
  }
  set(key, value) {
    this.configuration[___] = ___;  // Set a configuration value
  }
  get(key) {
    return this.configuration[___];  // Retrieve a configuration value
  }
}
```

**Completed implementation:**

```javascript
// --- Singleton Pattern ---
class Settings {
  constructor() {
    if (Settings.instance) {
      console.log('Settings instance reused.');
      return Settings.instance;
    }
    console.log('Settings instance created.');
    this.configuration = {};
    Settings.instance = this;
  }
  set(key, value) {
    this.configuration[key] = value;
    console.log(`Setting set: ${key} = ${value}`);
  }
  get(key) {
    const value = this.configuration[key];
    console.log(`Setting get: ${key} = ${value}`);
    return value;
  }
}
```

### Step 7 — Add the demo drivers and helpers (`main.js`)

These functions are called by the buttons in `index.html`. They run each demo and manage the
placeholder text in the output boxes. Add them to `main.js` alongside the patterns above.

```javascript
// --- Helper Functions ---
function setPlaceholder(containerId, message) {
  const container = document.getElementById(containerId);
  if (container) {
    container.innerHTML = `<div class="placeholder">${message}</div>`;
    console.log(`Placeholder set in #${containerId}: "${message}"`);
  }
}

function clearPlaceholder(containerId) {
  const container = document.getElementById(containerId);
  if (container) {
    const placeholder = container.querySelector('.placeholder');
    if (placeholder) {
      placeholder.remove();
      console.log(`Placeholder cleared in #${containerId}`);
    }
  }
}

// --- Observer demo drivers ---
window.runObserverDemo = function () {
  const output = document.getElementById('observer-output');
  if (output) output.innerHTML = '';
  clearPlaceholder('observer-output');
  const subject = new Subject();
  const observerA = new Observer("Observer A", "observer-output");
  const observerB = new Observer("Observer B", "observer-output");
  subject.subscribe(observerA);
  subject.subscribe(observerB);
  subject.notify();
};

window.clearObserverOutput = function () {
  setPlaceholder('observer-output', 'No observer output yet.');
};

// --- Singleton demo drivers ---
window.runSingletonDemo = function () {
  clearPlaceholder('singleton-output');
  const settings1 = new Settings();
  settings1.set("theme", "dark");
  const settings2 = new Settings();
  const output = document.getElementById('singleton-output');
  if (output) {
    output.innerHTML = '';
    const message = document.createElement('div');
    message.textContent = `Theme from settings2: ${settings2.get("theme")}`;
    output.appendChild(message);
  } else {
    console.warn("Element with ID 'singleton-output' not found.");
  }
};

window.clearSingletonOutput = function () {
  setPlaceholder('singleton-output', 'No singleton output yet.');
};
```

### Step 8 — Add the styling (`styles.css`)

Create a `styles.css` file next to `index.html` and paste in the styles for the calculator,
buttons, and output boxes.

```css
body {
  font-family: Arial, sans-serif;
  margin: 40px;
  background-color: #f4f4f4;
  color: #333;
  line-height: 1.5;
}
h1, h2 {
  color: #333;
  margin-bottom: 15px;
}
h2 {
  margin-top: 40px;
}
#result {
  font-size: 24px;
  margin-bottom: 20px;
  padding: 12px 18px;
  background-color: #fff;
  border: 2px solid #ccc;
  border-radius: 6px;
  display: inline-block;
}
button {
  margin: 5px 10px 10px 0;
  padding: 10px 20px;
  font-size: 16px;
  cursor: pointer;
  border: none;
  border-radius: 6px;
  background-color: #3498db;
  color: white;
  transition: background-color 0.3s ease;
}
button:hover {
  background-color: #2980b9;
}
.output-box {
  margin-top: 10px;
  padding: 15px;
  background-color: #eef;
  border: 1px solid #bbb;
  border-radius: 6px;
  min-height: 40px;
  width: fit-content;
  max-width: 100%;
  box-shadow: 2px 2px 5px rgba(0, 0, 0, 0.05);
}
.observer-message {
  margin-bottom: 8px;
  padding: 8px 12px;
  background-color: #dff0d8;
  border-left: 4px solid #3c763d;
  border-radius: 4px;
  font-size: 15px;
  color: #2b542c;
}
input[type="text"] {
  padding: 8px;
  font-size: 16px;
  border: 1px solid #ccc;
  border-radius: 5px;
  margin-right: 10px;
}
input[type="text"]:focus {
  border-color: #3498db;
  outline: none;
}
.placeholder {
  color: #888;
  font-style: italic;
  font-size: 14px;
  padding: 4px;
}
```

### Step 9 — Run your code

- Click **Go Live** (in the lower-right of the lab).
- A new tab opens and displays your webpage.

If your code is not running as expected, compare it against the completed code in the steps
above (or the next item in the lab) to find the missing piece.

---

## ▶️ Expected result

The page loads showing **Result: 0**.

- **Add 5 / Subtract 2** update the result in place — the calculator's internal `result` is
  private, mutated only through the exposed `add` / `subtract` methods.
- **Run Observer Demo** subscribes *Observer A* and *Observer B*, then `notify()` makes both
  print `Observer X received notification!` into the observer output box.
- **Run Singleton Demo** sets `theme = "dark"` on the first `Settings`, then a *second*
  `new Settings()` reads the same value back — proving both variables point at one shared
  instance (`Theme from settings2: dark`).

Open the browser Console to watch the matching log lines for every action.

---

## ☑️ Definition of done

- [ ] `index.html`, `main.js`, and `styles.css` created in the same folder
- [ ] `CalculatorModule` keeps `result` private and exposes only `add` and `subtract`
- [ ] **Add 5** / **Subtract 2** update the on-screen `#result`
- [ ] `Subject.notify()` calls `update()` on every subscribed `Observer`, writing to the output box
- [ ] A second `new Settings()` returns the **same** instance and reads back `theme = "dark"`
- [ ] **Go Live** serves the page and all three demos run without console errors

---

## 🔑 Key concepts

- **Module pattern = encapsulation via closure** — an IIFE keeps `result` and `displayResult()`
  private and returns a small public API (`add`, `subtract`), preventing outside code from
  corrupting internal state.
- **Observer pattern = loose coupling** — the `Subject` knows only that its observers implement
  `update()`; it can broadcast to any number of them without knowing what each one does.
- **Singleton pattern = one shared instance** — guarding the constructor with
  `Settings.instance` ensures every `new Settings()` returns the same object, giving the whole
  app a single source of truth for configuration.
- **Patterns compose** — each pattern solves one structural problem (state hiding, event
  fan-out, shared state), and combining them yields code that is reusable, decoupled, and
  easier to reason about.
- **Defensive DOM access** — null-checking `getElementById` and logging to the console makes the
  demo resilient and easy to debug when an element id is missing.
