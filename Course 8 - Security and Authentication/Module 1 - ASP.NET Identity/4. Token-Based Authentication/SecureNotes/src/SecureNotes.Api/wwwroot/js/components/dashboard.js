import { apiFetch } from '../api.js';
import { navigate } from '../router.js';
import { renderJwtInspector } from './jwt-inspector.js';
import { renderTokenStatus } from './token-status.js';

export function renderDashboard() {
    const app = document.getElementById('app');
    app.innerHTML = `
        <div class="max-w-7xl mx-auto px-4 py-6 fade-in">
            <div class="flex flex-col lg:flex-row gap-6">
                <!-- Notes panel -->
                <div class="flex-1">
                    <div class="flex items-center justify-between mb-6">
                        <h2 class="text-2xl font-bold">My Notes</h2>
                        <button id="new-note-btn"
                                class="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors flex items-center gap-2">
                            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
                            </svg>
                            New Note
                        </button>
                    </div>
                    <div id="notes-grid" class="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div class="text-gray-500">Loading notes...</div>
                    </div>
                </div>

                <!-- JWT Inspector sidebar -->
                <div class="lg:w-80 xl:w-96">
                    <div class="bg-gray-800 rounded-xl border border-gray-700 p-4 sticky top-4">
                        <h3 class="text-lg font-bold mb-4 flex items-center gap-2">
                            <svg class="w-5 h-5 text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                      d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                      d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                            </svg>
                            JWT Inspector
                        </h3>
                        <div id="jwt-inspector-panel" class="max-h-[calc(100vh-160px)] overflow-y-auto"></div>
                    </div>
                </div>
            </div>
        </div>
    `;

    renderTokenStatus();
    renderJwtInspector(document.getElementById('jwt-inspector-panel'));
    loadNotes();

    document.getElementById('new-note-btn').addEventListener('click', () => navigate('#/notes/new'));
}

async function loadNotes() {
    const grid = document.getElementById('notes-grid');

    try {
        const res = await apiFetch('/api/notes');
        if (!res.ok) throw new Error('Failed to load notes');
        const notes = await res.json();

        if (notes.length === 0) {
            grid.innerHTML = `
                <div class="col-span-full text-center py-12">
                    <svg class="w-16 h-16 text-gray-600 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"
                              d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                    </svg>
                    <p class="text-gray-500">No notes yet. Create your first note!</p>
                </div>
            `;
            return;
        }

        grid.innerHTML = notes.map(note => `
            <div class="note-card bg-gray-800 rounded-xl border border-gray-700 p-4 cursor-pointer relative"
                 data-id="${note.id}">
                ${note.isPinned ? `
                    <div class="absolute top-3 right-3 text-yellow-400" title="Pinned">
                        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                            <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
                        </svg>
                    </div>
                ` : ''}
                <h3 class="font-semibold text-white mb-2 pr-6">${escapeHtml(note.title)}</h3>
                <p class="text-gray-400 text-sm line-clamp-3 mb-3">${escapeHtml(note.content)}</p>
                <div class="flex items-center justify-between">
                    <span class="text-xs text-gray-500">${new Date(note.updatedAt).toLocaleDateString()}</span>
                    <div class="flex gap-2">
                        <button class="edit-note text-gray-400 hover:text-blue-400 transition-colors p-1" data-id="${note.id}" title="Edit">
                            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                      d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                            </svg>
                        </button>
                        <button class="delete-note text-gray-400 hover:text-red-400 transition-colors p-1" data-id="${note.id}" title="Delete">
                            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                      d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                            </svg>
                        </button>
                    </div>
                </div>
            </div>
        `).join('');

        grid.querySelectorAll('.edit-note').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                navigate(`#/notes/edit/${btn.dataset.id}`);
            });
        });

        grid.querySelectorAll('.delete-note').forEach(btn => {
            btn.addEventListener('click', async (e) => {
                e.stopPropagation();
                if (!confirm('Delete this note?')) return;
                const res = await apiFetch(`/api/notes/${btn.dataset.id}`, { method: 'DELETE' });
                if (res.ok) loadNotes();
            });
        });

    } catch (err) {
        grid.innerHTML = `<div class="text-red-400">${err.message}</div>`;
    }
}

function escapeHtml(str) {
    const div = document.createElement('div');
    div.textContent = str;
    return div.innerHTML;
}
