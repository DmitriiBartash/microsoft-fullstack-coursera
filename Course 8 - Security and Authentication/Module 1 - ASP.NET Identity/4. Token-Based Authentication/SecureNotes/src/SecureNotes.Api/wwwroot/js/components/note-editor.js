import { apiFetch } from '../api.js';
import { navigate } from '../router.js';

export function renderNoteEditor(noteId = null) {
    const app = document.getElementById('app');
    app.innerHTML = `
        <div class="max-w-2xl mx-auto px-4 py-8 fade-in">
            <div class="bg-gray-800 rounded-xl p-6 border border-gray-700">
                <h2 class="text-xl font-bold mb-6">${noteId ? 'Edit Note' : 'New Note'}</h2>

                <div id="editor-error" class="hidden bg-red-900/50 border border-red-700 text-red-300 px-4 py-3 rounded-lg mb-4"></div>

                <form id="note-form" class="space-y-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-300 mb-1">Title</label>
                        <input type="text" id="note-title" required maxlength="200"
                               class="w-full px-4 py-2.5 bg-gray-700 border border-gray-600 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                               placeholder="Note title">
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-300 mb-1">Content</label>
                        <textarea id="note-content" required maxlength="10000" rows="8"
                                  class="w-full px-4 py-2.5 bg-gray-700 border border-gray-600 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 resize-y"
                                  placeholder="Write your note..."></textarea>
                    </div>
                    <div class="flex items-center gap-2">
                        <input type="checkbox" id="note-pinned" class="w-4 h-4 rounded bg-gray-700 border-gray-600 text-blue-500 focus:ring-blue-500">
                        <label for="note-pinned" class="text-sm text-gray-300">Pin this note</label>
                    </div>
                    <div class="flex gap-3 pt-2">
                        <button type="submit" id="save-btn"
                                class="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 px-6 rounded-lg transition-colors">
                            ${noteId ? 'Update' : 'Create'}
                        </button>
                        <button type="button" id="cancel-btn"
                                class="bg-gray-700 hover:bg-gray-600 text-gray-300 font-medium py-2.5 px-6 rounded-lg transition-colors">
                            Cancel
                        </button>
                    </div>
                </form>
            </div>
        </div>
    `;

    if (noteId) loadNote(noteId);

    document.getElementById('note-form').addEventListener('submit', (e) => handleSave(e, noteId));
    document.getElementById('cancel-btn').addEventListener('click', () => navigate('#/dashboard'));
}

async function loadNote(id) {
    try {
        const res = await apiFetch(`/api/notes/${id}`);
        if (!res.ok) throw new Error('Failed to load note');
        const note = await res.json();
        document.getElementById('note-title').value = note.title;
        document.getElementById('note-content').value = note.content;
        document.getElementById('note-pinned').checked = note.isPinned;
    } catch (err) {
        document.getElementById('editor-error').textContent = err.message;
        document.getElementById('editor-error').classList.remove('hidden');
    }
}

async function handleSave(e, noteId) {
    e.preventDefault();
    const errorEl = document.getElementById('editor-error');
    const btn = document.getElementById('save-btn');
    errorEl.classList.add('hidden');
    btn.disabled = true;

    const body = {
        title: document.getElementById('note-title').value,
        content: document.getElementById('note-content').value,
        isPinned: document.getElementById('note-pinned').checked
    };

    try {
        const url = noteId ? `/api/notes/${noteId}` : '/api/notes';
        const method = noteId ? 'PUT' : 'POST';
        const res = await apiFetch(url, { method, body: JSON.stringify(body) });

        if (!res.ok) {
            const err = await res.json();
            throw new Error(err.detail || 'Failed to save note');
        }

        navigate('#/dashboard');
    } catch (err) {
        errorEl.textContent = err.message;
        errorEl.classList.remove('hidden');
    } finally {
        btn.disabled = false;
    }
}
