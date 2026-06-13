# Build a JavaScript-Enhanced Web Page

**Course 3 — Introduction to Web Development** · Module 3 · Lesson 1 · `You Try It!`

> Build a single, self-contained HTML page that **embeds JavaScript** with a `<script>`
> tag and exercises the core building blocks of the language: declaring variables
> (`let` / `const` / `var`), a control structure (`if`), and a reusable function
> (`squareNumber`). All output goes to the browser **console**.

---

## 🎯 Objective

Reinforce basic JavaScript syntax and how it lives inside a web page by creating
`index.html` that prints messages, declares variables, branches on a condition, and
defines and calls a function — observing each result in the browser's developer console.

---

## 🗂️ What you will build

A single file, **`index.html`**, that combines markup and an inline script:

| Piece                  | What it does                                                              |
| ---------------------- | ------------------------------------------------------------------------ |
| HTML structure         | `<!DOCTYPE html>` page with `<head>` (title + charset) and `<body>`       |
| Heading                | An `<h1>` reading `Hello, JavaScript!`                                    |
| `<script>` block       | Inline JavaScript executed when the browser loads the page               |
| `console.log` calls    | Print a greeting, the variable values, and computed results              |
| Variables              | `name` (`let`), `age` (`const`), `city` (`var`)                          |
| Control structure      | An `if` that checks whether `age > 18`                                    |
| `squareNumber(number)` | A function that returns `number * number`, called with `4`               |

**Flow:** `index.html loads  →  <script> runs  →  console.log → variables → if check → squareNumber(4)  →  console output`

---

## ✅ Prerequisites

- A code editor (Visual Studio Code recommended)
- Any modern web browser (Chrome, Edge, Firefox)
- Knowing how to open the browser **developer console** (`F12` → **Console** tab)

---

## 🛠️ Steps

### Step 1 — Create a new HTML file

In your editor, create the file that will hold the page.

- Select **File** → **New File…**
- Name it **`index.html`**
- Press **Enter**, then **OK** to confirm the location.

### Step 2 — Create the HTML structure

Set up a valid HTML5 skeleton.

- Use `<!DOCTYPE html>` to declare HTML5.
- Add `<html>`, `<head>`, and `<body>` tags to structure the content.
- Inside `<head>`, include a `<meta charset="UTF-8">` tag and a `<title>` of `JavaScript Lab`.
- Inside `<body>`, add an `<h1>` heading that reads `Hello, JavaScript!`.

```html
<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <title>JavaScript Lab</title>
</head>
<body>
  <h1>Hello, JavaScript!</h1>
</body>
</html>
```

### Step 3 — Write the JavaScript code

Inside `<body>`, add a `<script>` tag and write the logic step by step.

- **Embed JavaScript** — use `console.log` to print `"Hello, World!"` to the browser console.
- **Declare variables** — a `name` with `let` (`"Alice"`), a constant `age` with `const` (`30`),
  and a `city` with `var` (`"New York"`); print all three with `console.log`.
- **Control structure** — use an `if` statement to check whether `age` is greater than `18`,
  and print `"You are an adult"` when it is true.
- **Write a function** — define `squareNumber(number)` that returns `number * number`,
  call it with `4`, and print the result.

```html
<script>
  // Print message to the console
  console.log("Hello, World!");

  // Declare variables
  let name = "Alice";
  const age = 30;
  var city = "New York";

  // Print variables
  console.log("Name:", name);
  console.log("Age:", age);
  console.log("City:", city);

  // Control structure
  if (age > 18) {
    console.log("You are an adult");
  }

  // Function to square a number
  function squareNumber(number) {
    return number * number;
  }

  // Call the function and print result
  let result = squareNumber(4);
  console.log("Square of 4 is:", result);
</script>
```

### Step 4 — Run your code

Assemble the full page, then open it in the browser and watch the console.

- Place the `<script>` block inside `<body>` (after the `<h1>`).
- Open `index.html` in your browser (double-click it, or use a Live Server extension).
- Open the developer console with `F12` and select the **Console** tab.

```html
<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <title>JavaScript Lab</title>
</head>
<body>
  <h1>Hello, JavaScript!</h1>
  <script>
    // Print message to the console
    console.log("Hello, World!");

    // Declare variables
    let name = "Alice";
    const age = 30;
    var city = "New York";

    // Print variables
    console.log("Name:", name);
    console.log("Age:", age);
    console.log("City:", city);

    // Control structure
    if (age > 18) {
      console.log("You are an adult");
    }

    // Function to square a number
    function squareNumber(number) {
      return number * number;
    }

    // Call the function and print result
    let result = squareNumber(4);
    console.log("Square of 4 is:", result);
  </script>
</body>
</html>
```

---

## ▶️ Expected result

The page shows the `Hello, JavaScript!` heading, and the **Console** tab prints:

```text
Hello, World!
Name: Alice
Age: 30
City: New York
You are an adult
Square of 4 is: 16
```

---

## ☑️ Definition of done

- [ ] `index.html` exists with a valid HTML5 structure (`<!DOCTYPE html>`, `<head>`, `<body>`)
- [ ] `<head>` includes `<meta charset="UTF-8">` and the title `JavaScript Lab`
- [ ] The `<body>` shows an `<h1>` reading `Hello, JavaScript!`
- [ ] A `<script>` block prints `"Hello, World!"` and the values of `name`, `age`, and `city`
- [ ] The `if (age > 18)` check prints `"You are an adult"`
- [ ] `squareNumber(4)` returns `16` and the result is printed to the console

---

## 🔑 Key concepts

- **Embedding JavaScript** — a `<script>` tag inside the HTML lets the browser run JS as it
  loads the page; `console.log` is your first window into what the code is doing.
- **`let` vs `const` vs `var`** — `let` is a reassignable block-scoped variable, `const` is a
  block-scoped binding that cannot be reassigned, and `var` is the older function-scoped form.
- **Control flow with `if`** — conditions evaluate to a boolean; the block runs only when the
  expression (here `age > 18`) is `true`.
- **Functions and return values** — `squareNumber` takes a parameter, computes
  `number * number`, and hands the result back to the caller for reuse.
- **The developer console** — pressing `F12` and opening the **Console** tab is the standard
  way to inspect output and debug JavaScript directly in the browser.
