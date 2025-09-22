let allUsers = [];
let currentPage = 1;
let pageSize = 10;

const AUTH_TOKEN = 'dev-token';
async function apiFetch(url, options = {}) {
    const headers = { Authorization: `Bearer ${AUTH_TOKEN}`, ...(options.headers || {}) };
    return fetch(url, { ...options, headers });
}

async function loadUsers() {
    try {
        const response = await apiFetch('/api/users');
        if (!response.ok) throw new Error("Failed to fetch users");
        allUsers = await response.json();
        renderUsers();
    } catch (error) {
        alert("Error loading users");
        console.error(error);
    }
}

function renderUsers() {
    const tbody = document.querySelector('#userTable tbody');
    tbody.innerHTML = '';

    const total = allUsers.length;
    const totalPages = Math.max(1, Math.ceil(total / pageSize));
    if (currentPage > totalPages) currentPage = totalPages;

    if (total === 0) {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="5" class="text-center text-muted">No users found</td>`;
        tbody.appendChild(row);
        document.getElementById('pageInfo').textContent = '0–0 of 0';
        buildPager(1);
        return;
    }

    const start = (currentPage - 1) * pageSize;
    const end = Math.min(start + pageSize, total);
    const pageUsers = allUsers.slice(start, end);

    pageUsers.forEach(u => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>
                <a href="#" onclick="viewUser(${u.id});return false;" class="badge bg-primary text-decoration-none">${u.id}</a>
            </td>
            <td>${u.firstName}</td>
            <td>${u.lastName}</td>
            <td>${u.email}</td>
            <td>
                <div class="d-inline-flex">
                    <button class="btn btn-sm btn-outline-primary me-1" onclick="editUser(${u.id})" title="Edit">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-danger" onclick="deleteUser(${u.id})" title="Delete">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </td>
        `;
        tbody.appendChild(row);
    });

    document.getElementById('pageInfo').textContent = `${start + 1}–${end} of ${total}`;
    buildPager(totalPages);
    const ps = document.getElementById('pageSize');
    if (ps && parseInt(ps.value, 10) !== pageSize) ps.value = String(pageSize);
}

function buildPager(totalPages) {
    const pager = document.getElementById('pager');
    pager.innerHTML = '';

    addPageItem('«', currentPage === 1, false, currentPage - 1);

    const windowSize = 5;
    let startPage = Math.max(1, currentPage - Math.floor(windowSize / 2));
    let endPage = Math.min(totalPages, startPage + windowSize - 1);
    if (endPage - startPage + 1 < windowSize) startPage = Math.max(1, endPage - windowSize + 1);

    if (startPage > 1) {
        addPageItem('1', false, currentPage === 1, 1);
        if (startPage > 2) addEllipsis();
    }

    for (let p = startPage; p <= endPage; p++) {
        addPageItem(String(p), false, currentPage === p, p);
    }

    if (endPage < totalPages) {
        if (endPage < totalPages - 1) addEllipsis();
        addPageItem(String(totalPages), false, currentPage === totalPages, totalPages);
    }

    addPageItem('»', currentPage === totalPages, false, currentPage + 1);

    function addPageItem(text, disabled, active, targetPage) {
        const li = document.createElement('li');
        li.className = 'page-item' + (disabled ? ' disabled' : '') + (active ? ' active' : '');
        const a = document.createElement('a');
        a.className = 'page-link';
        a.href = '#';
        a.textContent = text;
        a.onclick = (e) => {
            e.preventDefault();
            if (disabled || active) return;
            goToPage(targetPage);
        };
        li.appendChild(a);
        pager.appendChild(li);
    }

    function addEllipsis() {
        const li = document.createElement('li');
        li.className = 'page-item disabled';
        const span = document.createElement('span');
        span.className = 'page-link';
        span.textContent = '…';
        li.appendChild(span);
        pager.appendChild(li);
    }
}

function goToPage(p) {
    const totalPages = Math.max(1, Math.ceil(allUsers.length / pageSize));
    currentPage = Math.min(Math.max(1, p), totalPages);
    renderUsers();
}

function changePageSize(val) {
    pageSize = parseInt(val, 10) || 10;
    currentPage = 1;
    renderUsers();
}

async function addUser() {
    const firstNameInput = document.getElementById('firstName');
    const lastNameInput = document.getElementById('lastName');
    const emailInput = document.getElementById('email');

    const firstName = firstNameInput.value.trim();
    const lastName = lastNameInput.value.trim();
    const email = emailInput.value.trim();

    let isValid = true;

    if (!firstName) { firstNameInput.classList.add("is-invalid"); isValid = false; } else { firstNameInput.classList.remove("is-invalid"); }
    if (!lastName) { lastNameInput.classList.add("is-invalid"); isValid = false; } else { lastNameInput.classList.remove("is-invalid"); }
    if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) { emailInput.classList.add("is-invalid"); isValid = false; } else { emailInput.classList.remove("is-invalid"); }

    if (!isValid) return;

    try {
        const response = await apiFetch('/api/users', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ firstName, lastName, email })
        });
        if (!response.ok) throw new Error("Failed to add user");
        clearForm();
        await loadUsers();
        const totalPages = Math.ceil(allUsers.length / pageSize);
        currentPage = totalPages;
        renderUsers();
    } catch (error) {
        alert("Error adding user");
        console.error(error);
    }
}

function editUser(id) {
    const row = [...document.querySelectorAll('#userTable tbody tr')]
        .find(r => r.querySelector('td a')?.textContent.trim() == String(id));

    const firstName = row.children[1].textContent;
    const lastName = row.children[2].textContent;
    const email = row.children[3].textContent;

    row.children[1].innerHTML = `<input type="text" class="form-control form-control-sm" value="${firstName}" required>`;
    row.children[2].innerHTML = `<input type="text" class="form-control form-control-sm" value="${lastName}" required>`;
    row.children[3].innerHTML = `<input type="email" class="form-control form-control-sm" value="${email}" required>`;

    row.children[4].innerHTML = `
        <div class="d-inline-flex">
            <button class="btn btn-sm btn-outline-success me-1" onclick="saveUser(${id}, this)" title="Save">
                <i class="bi bi-check-lg"></i>
            </button>
            <button class="btn btn-sm btn-outline-secondary" onclick="cancelEdit()" title="Cancel">
                <i class="bi bi-x-lg"></i>
            </button>
        </div>
    `;
}

async function saveUser(id, btn) {
    const row = btn.closest('tr');
    const firstName = row.children[1].querySelector('input').value.trim();
    const lastName = row.children[2].querySelector('input').value.trim();
    const email = row.children[3].querySelector('input').value.trim();

    if (!firstName || !lastName || !email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
        alert("Fill all fields with valid values");
        return;
    }

    try {
        const response = await apiFetch(`/api/users/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id, firstName, lastName, email })
        });
        if (!response.ok) throw new Error("Failed to update user");
        await loadUsers();
        renderUsers();
    } catch (error) {
        alert("Error updating user");
        console.error(error);
    }
}

function cancelEdit() {
    renderUsers();
}

async function deleteUser(id) {
    if (!confirm("Are you sure you want to delete this user?")) return;
    try {
        const response = await apiFetch(`/api/users/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error("Failed to delete user");
        const prevTotalPages = Math.max(1, Math.ceil((allUsers.length - 1) / pageSize));
        await loadUsers();
        if (currentPage > prevTotalPages) currentPage = prevTotalPages;
        renderUsers();
    } catch (error) {
        alert("Error deleting user");
        console.error(error);
    }
}

async function viewUser(id) {
    try {
        const response = await apiFetch(`/api/users/${id}`);
        if (!response.ok) { alert("User not found"); return; }
        const user = await response.json();
        document.getElementById('modalUserId').textContent = user.id;
        document.getElementById('modalFirstName').textContent = user.firstName;
        document.getElementById('modalLastName').textContent = user.lastName;
        document.getElementById('modalEmail').textContent = user.email;
        const modal = new bootstrap.Modal(document.getElementById('userModal'));
        modal.show();
    } catch (error) {
        alert("Error loading user details");
        console.error(error);
    }
}

function clearForm() {
    document.getElementById('firstName').value = '';
    document.getElementById('lastName').value = '';
    document.getElementById('email').value = '';
    document.getElementById('firstName').classList.remove("is-invalid");
    document.getElementById('lastName').classList.remove("is-invalid");
    document.getElementById('email').classList.remove("is-invalid");
}

window.onload = loadUsers;
