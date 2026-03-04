const TOKEN_KEY = 'securenotes_access_token';
const REFRESH_KEY = 'securenotes_refresh_token';
const EXPIRES_KEY = 'securenotes_expires_at';

let refreshTimer = null;
let countdownTimer = null;
let countdownCallback = null;

export function getAccessToken() {
    return localStorage.getItem(TOKEN_KEY);
}

export function getRefreshToken() {
    return localStorage.getItem(REFRESH_KEY);
}

export function getExpiresAt() {
    const val = localStorage.getItem(EXPIRES_KEY);
    return val ? new Date(val) : null;
}

export function saveTokens(accessToken, refreshToken, expiresAt) {
    localStorage.setItem(TOKEN_KEY, accessToken);
    localStorage.setItem(REFRESH_KEY, refreshToken);
    localStorage.setItem(EXPIRES_KEY, expiresAt);
    scheduleRefresh();
}

export function clearTokens() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_KEY);
    localStorage.removeItem(EXPIRES_KEY);
    if (refreshTimer) clearTimeout(refreshTimer);
    if (countdownTimer) clearInterval(countdownTimer);
    refreshTimer = null;
    countdownTimer = null;
}

export function isAuthenticated() {
    return !!getAccessToken();
}

export function decodeJwt(token) {
    if (!token) return null;
    try {
        const parts = token.split('.');
        if (parts.length !== 3) return null;

        const decodeBase64Url = (str) => {
            let base64 = str.replace(/-/g, '+').replace(/_/g, '/');
            while (base64.length % 4) base64 += '=';
            return JSON.parse(atob(base64));
        };

        return {
            header: decodeBase64Url(parts[0]),
            payload: decodeBase64Url(parts[1]),
            signature: parts[2],
            raw: { header: parts[0], payload: parts[1], signature: parts[2] }
        };
    } catch {
        return null;
    }
}

export function getSecondsUntilExpiry() {
    const expiresAt = getExpiresAt();
    if (!expiresAt) return 0;
    return Math.max(0, Math.floor((expiresAt.getTime() - Date.now()) / 1000));
}

export function formatCountdown(seconds) {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
}

export function onCountdownTick(callback) {
    countdownCallback = callback;
}

function startCountdown() {
    if (countdownTimer) clearInterval(countdownTimer);
    countdownTimer = setInterval(() => {
        if (countdownCallback) countdownCallback(getSecondsUntilExpiry());
    }, 1000);
}

export function scheduleRefresh() {
    if (refreshTimer) clearTimeout(refreshTimer);

    const seconds = getSecondsUntilExpiry();
    if (seconds <= 0) return;

    startCountdown();

    const refreshAt = Math.max(0, (seconds - 30) * 1000);
    refreshTimer = setTimeout(() => doRefresh(), refreshAt);
}

export async function doRefresh() {
    const accessToken = getAccessToken();
    const refreshToken = getRefreshToken();
    if (!accessToken || !refreshToken) return false;

    try {
        const res = await fetch('/api/auth/refresh', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ accessToken, refreshToken })
        });

        if (!res.ok) {
            clearTokens();
            window.location.hash = '#/login';
            return false;
        }

        const data = await res.json();
        saveTokens(data.accessToken, data.refreshToken, data.expiresAt);
        return true;
    } catch {
        return false;
    }
}
