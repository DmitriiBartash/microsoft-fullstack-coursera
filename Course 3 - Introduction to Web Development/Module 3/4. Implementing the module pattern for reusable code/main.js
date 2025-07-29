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
