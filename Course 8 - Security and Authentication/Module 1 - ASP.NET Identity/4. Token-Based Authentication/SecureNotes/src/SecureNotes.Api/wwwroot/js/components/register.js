import { saveTokens } from '../auth.js';

export function renderRegister() {
    const app = document.getElementById('app');
    app.innerHTML = `
        <div class="min-h-screen flex items-center justify-center px-4 fade-in">
            <div class="bg-gray-800 rounded-xl shadow-2xl p-8 w-full max-w-md border border-gray-700">
                <div class="text-center mb-8">
                    <h1 class="text-2xl font-bold">Create Account</h1>
                    <p class="text-gray-400 mt-1">Join SecureNotes</p>
                </div>

                <div id="register-error" class="hidden bg-red-900/50 border border-red-700 text-red-300 px-4 py-3 rounded-lg mb-4"></div>

                <form id="register-form" class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-300 mb-1">Display Name</label>
                        <input type="text" id="reg-name" required minlength="2"
                               class="w-full px-4 py-2.5 bg-gray-700 border border-gray-600 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                               placeholder="Your name">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-300 mb-1">Email</label>
                        <input type="email" id="reg-email" required
                               class="w-full px-4 py-2.5 bg-gray-700 border border-gray-600 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                               placeholder="you@example.com">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-300 mb-1">Password</label>
                        <input type="password" id="reg-password" required minlength="6"
                               class="w-full px-4 py-2.5 bg-gray-700 border border-gray-600 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                               placeholder="Min 6 characters">
                    </div>
                    <button type="submit" id="reg-btn"
                            class="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg transition-colors">
                        Create Account
                    </button>
                </form>

                <p class="text-center text-gray-400 mt-6 text-sm">
                    Already have an account?
                    <a href="#/login" class="text-blue-400 hover:text-blue-300">Sign in</a>
                </p>
            </div>
        </div>
    `;

    document.getElementById('register-form').addEventListener('submit', handleRegister);
}

async function handleRegister(e) {
    e.preventDefault();
    const errorEl = document.getElementById('register-error');
    const btn = document.getElementById('reg-btn');
    errorEl.classList.add('hidden');
    btn.disabled = true;
    btn.textContent = 'Creating account...';

    try {
        const res = await fetch('/api/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                email: document.getElementById('reg-email').value,
                password: document.getElementById('reg-password').value,
                displayName: document.getElementById('reg-name').value
            })
        });

        if (!res.ok) {
            const err = await res.json();
            throw new Error(err.detail || 'Registration failed');
        }

        const data = await res.json();
        saveTokens(data.accessToken, data.refreshToken, data.expiresAt);
        window.location.hash = '#/dashboard';
    } catch (err) {
        errorEl.textContent = err.message;
        errorEl.classList.remove('hidden');
    } finally {
        btn.disabled = false;
        btn.textContent = 'Create Account';
    }
}
