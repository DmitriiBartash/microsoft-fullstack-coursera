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
