import { getAccessToken, decodeJwt, getSecondsUntilExpiry, formatCountdown, clearTokens, onCountdownTick } from '../auth.js';
import { apiFetch } from '../api.js';

export function renderTokenStatus() {
    const container = document.getElementById('token-status-bar');
    if (!container) return;

    const token = getAccessToken();
    const decoded = decodeJwt(token);
    const displayName = decoded?.payload?.displayName || 'User';
    const seconds = getSecondsUntilExpiry();

    container.innerHTML = `
        <div class="flex items-center gap-4 text-sm">
            <div class="flex items-center gap-2">
                <span id="header-status-dot" class="status-dot ${getColor(seconds)}"></span>
                <span class="text-gray-400">Expires in</span>
                <span id="header-countdown" class="countdown font-mono text-gray-200">${formatCountdown(seconds)}</span>
            </div>
            <div class="text-gray-400">|</div>
            <span class="text-gray-300">${escapeHtml(displayName)}</span>
            <button id="logout-btn" class="text-gray-400 hover:text-red-400 transition-colors" title="Logout">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/>
                </svg>
            </button>
        </div>
    `;

    onCountdownTick((sec) => {
        const el = document.getElementById('header-countdown');
        const dot = document.getElementById('header-status-dot');
        if (el) el.textContent = formatCountdown(sec);
        if (dot) dot.className = 'status-dot ' + getColor(sec);
    });

    document.getElementById('logout-btn')?.addEventListener('click', handleLogout);
}

function getColor(seconds) {
    if (seconds > 120) return 'green';
    if (seconds > 30) return 'yellow';
    return 'red';
}

async function handleLogout() {
    try {
        await apiFetch('/api/auth/logout', { method: 'POST' });
    } catch { /* ignore */ }
    clearTokens();
    window.location.hash = '#/login';
}

function escapeHtml(str) {
    const div = document.createElement('div');
    div.textContent = str;
    return div.innerHTML;
}
