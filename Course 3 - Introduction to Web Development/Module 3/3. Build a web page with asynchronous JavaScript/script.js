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
