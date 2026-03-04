import { registerRoute, startRouter } from './router.js';
import { scheduleRefresh, isAuthenticated } from './auth.js';
import { renderLogin } from './components/login.js';
import { renderRegister } from './components/register.js';
import { renderDashboard } from './components/dashboard.js';
import { renderNoteEditor } from './components/note-editor.js';

registerRoute('#/login', renderLogin);
registerRoute('#/register', renderRegister);
registerRoute('#/dashboard', renderDashboard);
registerRoute('#/notes/new', () => renderNoteEditor());

registerRoute('#/notes/edit', () => {
    const hash = window.location.hash;
    const match = hash.match(/#\/notes\/edit\/(.+)/);
    if (match) renderNoteEditor(match[1]);
    else window.location.hash = '#/dashboard';
});

if (isAuthenticated()) scheduleRefresh();

startRouter();
