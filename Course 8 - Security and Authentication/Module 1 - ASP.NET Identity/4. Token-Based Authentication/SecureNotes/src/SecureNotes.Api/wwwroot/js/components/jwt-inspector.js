import { getAccessToken, decodeJwt, getSecondsUntilExpiry, formatCountdown, onCountdownTick, doRefresh } from '../auth.js';

let inspectorInterval = null;

export function renderJwtInspector(container) {
    updateInspector(container);

    onCountdownTick(() => {
        const countdownEl = document.getElementById('jwt-countdown');
        const statusEl = document.getElementById('jwt-status-dot');
        if (!countdownEl || !statusEl) return;

        const seconds = getSecondsUntilExpiry();
        countdownEl.textContent = formatCountdown(seconds);

        statusEl.className = 'status-dot ' + getStatusColor(seconds);
    });
}

export function refreshInspector() {
    const container = document.getElementById('jwt-inspector-panel');
    if (container) updateInspector(container);
}

function updateInspector(container) {
    const token = getAccessToken();
    const decoded = decodeJwt(token);
    const seconds = getSecondsUntilExpiry();

    if (!decoded) {
        container.innerHTML = `<p class="text-gray-500 text-sm">No token available</p>`;
        return;
    }

    container.innerHTML = `
        <div class="space-y-4 slide-in">
            <!-- Status -->
            <div class="flex items-center justify-between">
                <div class="flex items-center gap-2">
                    <span id="jwt-status-dot" class="status-dot ${getStatusColor(seconds)}"></span>
                    <span class="text-sm font-medium">Token Status</span>
                </div>
                <span id="jwt-countdown" class="countdown text-sm font-mono ${seconds < 60 ? 'text-red-400' : 'text-green-400'}">
                    ${formatCountdown(seconds)}
                </span>
            </div>

            <!-- Force Refresh -->
            <button id="force-refresh-btn"
                    class="w-full text-sm bg-gray-700 hover:bg-gray-600 text-gray-300 py-2 rounded-lg transition-colors flex items-center justify-center gap-2">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
                </svg>
                Force Refresh
            </button>

            <!-- Raw Token -->
            <div>
                <h4 class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-2">Raw Token</h4>
                <div class="bg-gray-900 rounded-lg p-3 jwt-token text-xs max-h-24 overflow-auto">
                    <span class="jwt-header">${decoded.raw.header}</span><span class="jwt-dot">.</span><span class="jwt-payload">${decoded.raw.payload}</span><span class="jwt-dot">.</span><span class="jwt-signature">${decoded.raw.signature}</span>
                </div>
            </div>

            <!-- Header -->
            <div>
                <h4 class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-2">
                    <span class="text-blue-400">Header</span>
                </h4>
                <div class="bg-gray-900 rounded-lg p-3">
                    ${renderJson(decoded.header)}
                </div>
            </div>

            <!-- Payload / Claims -->
            <div>
                <h4 class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-2">
                    <span class="text-green-400">Payload</span> (Claims)
                </h4>
                <div class="bg-gray-900 rounded-lg p-3">
                    <table class="w-full text-xs">
                        <tbody>
                            ${renderClaimsTable(decoded.payload)}
                        </tbody>
                    </table>
                </div>
            </div>

            <!-- Signature -->
            <div>
                <h4 class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-2">
                    <span class="text-red-400">Signature</span>
                </h4>
                <p class="text-xs text-gray-500 bg-gray-900 rounded-lg p-3 font-mono break-all">
                    ${decoded.raw.signature}
                </p>
            </div>
        </div>
    `;

    document.getElementById('force-refresh-btn')?.addEventListener('click', async () => {
        const btn = document.getElementById('force-refresh-btn');
        btn.disabled = true;
        btn.textContent = 'Refreshing...';
        await doRefresh();
        refreshInspector();
    });
}

function getStatusColor(seconds) {
    if (seconds > 120) return 'green';
    if (seconds > 30) return 'yellow';
    return 'red';
}

function renderJson(obj) {
    const lines = JSON.stringify(obj, null, 2).split('\n');
    return `<pre class="json-viewer text-xs">${lines.map(line =>
        line.replace(/"([^"]+)":/g, '<span class="json-key">"$1"</span>:')
            .replace(/: "([^"]+)"/g, ': <span class="json-string">"$1"</span>')
            .replace(/: (\d+)/g, ': <span class="json-number">$1</span>')
            .replace(/: (true|false)/g, ': <span class="json-boolean">$1</span>')
    ).join('\n')}</pre>`;
}

const CLAIM_LABELS = {
    sub: 'Subject (User ID)',
    email: 'Email',
    displayName: 'Display Name',
    jti: 'JWT ID (unique)',
    iat: 'Issued At',
    exp: 'Expires',
    iss: 'Issuer',
    aud: 'Audience',
    nbf: 'Not Before'
};

function renderClaimsTable(payload) {
    return Object.entries(payload).map(([key, value]) => {
        const label = CLAIM_LABELS[key] || key;
        let displayValue = value;

        if (['exp', 'iat', 'nbf'].includes(key) && typeof value === 'number') {
            displayValue = new Date(value * 1000).toLocaleString();
        }

        return `
            <tr class="border-b border-gray-800 last:border-0">
                <td class="py-1.5 pr-3 text-gray-400 whitespace-nowrap">${label}</td>
                <td class="py-1.5 text-gray-200 font-mono break-all">${displayValue}</td>
            </tr>
        `;
    }).join('');
}
