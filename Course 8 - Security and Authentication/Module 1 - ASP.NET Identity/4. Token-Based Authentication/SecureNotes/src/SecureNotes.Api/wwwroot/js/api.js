import { getAccessToken, doRefresh, clearTokens } from './auth.js';

export async function apiFetch(url, options = {}) {
    const token = getAccessToken();

    const headers = {
        'Content-Type': 'application/json',
        ...options.headers
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    let res = await fetch(url, { ...options, headers });

    if (res.status === 401 && token) {
        const refreshed = await doRefresh();
        if (refreshed) {
            headers['Authorization'] = `Bearer ${getAccessToken()}`;
            res = await fetch(url, { ...options, headers });
        } else {
            clearTokens();
            window.location.hash = '#/login';
            throw new Error('Session expired');
        }
    }

    return res;
}
