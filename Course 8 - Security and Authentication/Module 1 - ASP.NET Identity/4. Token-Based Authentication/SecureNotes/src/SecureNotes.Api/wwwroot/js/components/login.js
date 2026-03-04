import { saveTokens } from '../auth.js';

export function renderLogin() {
    const app = document.getElementById('app');
    app.innerHTML = `
        <div class="min-h-screen flex items-center justify-center px-4 fade-in">
            <div class="bg-gray-800 rounded-xl shadow-2xl p-8 w-full max-w-md border border-gray-700">
                <div class="text-center mb-8">
                    <svg class="w-12 h-12 text-blue-400 mx-auto mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                              d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"/>
                    </svg>
                    <h1 class="text-2xl font-bold">SecureNotes</h1>
                    <p class="text-gray-400 mt-1">Sign in to your account</p>
                </div>

                <div id="login-error" class="hidden bg-red-900/50 border border-red-700 text-red-300 px-4 py-3 rounded-lg mb-4"></div>

                <form id="login-form" class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-300 mb-1">Email</label>
                        <input type="email" id="login-email" required
                               class="w-full px-4 py-2.5 bg-gray-700 border border-gray-600 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                               placeholder="demo@notes.com">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-300 mb-1">Password</label>
                        <input type="password" id="login-password" required
                               class="w-full px-4 py-2.5 bg-gray-700 border border-gray-600 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                               placeholder="Demo123">
                    </div>
                    <button type="submit" id="login-btn"
                            class="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg transition-colors">
                        Sign In
                    </button>
                </form>

                <p class="text-center text-gray-400 mt-6 text-sm">
                    Don't have an account?
                    <a href="#/register" class="text-blue-400 hover:text-blue-300">Register</a>
                </p>

                <div class="mt-6 pt-4 border-t border-gray-700">
                    <p class="text-xs text-gray-500 text-center">Demo accounts: demo@notes.com / Demo123</p>
                </div>
            </div>
        </div>
    `;

    document.getElementById('login-form').addEventListener('submit', handleLogin);
}

async function handleLogin(e) {
    e.preventDefault();
    const errorEl = document.getElementById('login-error');
    const btn = document.getElementById('login-btn');
    errorEl.classList.add('hidden');
    btn.disabled = true;
    btn.textContent = 'Signing in...';

    try {
        const res = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                email: document.getElementById('login-email').value,
                password: document.getElementById('login-password').value
            })
        });

        if (!res.ok) {
            const err = await res.json();
            throw new Error(err.detail || 'Login failed');
        }

        const data = await res.json();
        saveTokens(data.accessToken, data.refreshToken, data.expiresAt);
        window.location.hash = '#/dashboard';
    } catch (err) {
        errorEl.textContent = err.message;
        errorEl.classList.remove('hidden');
    } finally {
        btn.disabled = false;
        btn.textContent = 'Sign In';
    }
}
