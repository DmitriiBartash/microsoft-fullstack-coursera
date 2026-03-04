import { isAuthenticated } from './auth.js';

const routes = {};

export function registerRoute(path, handler) {
    routes[path] = handler;
}

export function navigate(hash) {
    window.location.hash = hash;
}

export function startRouter() {
    window.addEventListener('hashchange', handleRoute);
    handleRoute();
}

function handleRoute() {
    const hash = window.location.hash || '#/login';
    const path = hash.split('?')[0];

    const publicRoutes = ['#/login', '#/register'];
    const header = document.getElementById('app-header');
    const isPublic = publicRoutes.includes(path);

    if (!isPublic && !isAuthenticated()) {
        window.location.hash = '#/login';
        return;
    }

    if (isPublic && isAuthenticated()) {
        window.location.hash = '#/dashboard';
        return;
    }

    header.classList.toggle('hidden', isPublic);

    const handler = routes[path] || findPrefixRoute(path) || routes['#/login'];
    if (handler) handler();
}

function findPrefixRoute(path) {
    const prefixes = Object.keys(routes).sort((a, b) => b.length - a.length);
    for (const prefix of prefixes) {
        if (path.startsWith(prefix + '/')) return routes[prefix];
    }
    return null;
}
