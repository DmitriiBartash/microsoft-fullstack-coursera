// DOM element selection

const usersContainer = document.getElementById('users-container');
const localUsersContainer = document.getElementById('local-users');
const charlieContainer = document.getElementById('charlie-data');
const settingsContainer = document.getElementById('settings-data');

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
