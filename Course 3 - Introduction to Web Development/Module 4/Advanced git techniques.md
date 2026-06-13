# Advanced Git Techniques

**Course 3 — Introduction to Web Development** · Module 4 · `You Try It!`

> Practice the everyday-but-advanced Git moves on a single throwaway repo: spin up a
> `feature-new-feature` branch, **merge it and resolve a conflict**, **tag** a release,
> **stash** work-in-progress, and **rewrite history with an interactive rebase** — then
> force-push the cleaned-up branch.

---

## 🎯 Objective

Gain hands-on confidence with five advanced Git workflows — **branching**, **merge
conflict resolution**, **tagging**, **stashing**, and **interactive rebase** — by running
each one end-to-end against a small local repository you create from scratch.

---

## 🗂️ What you will build

A disposable repo named **`lab-advanced-git`** that you walk through five techniques:

| Step | Technique               | Key commands                                             |
| ---- | ----------------------- | ------------------------------------------------------- |
| 1    | Create & manage branches | `git checkout -b`, `git add`, `git commit`             |
| 2    | Merge & resolve conflicts | `git merge`, manual edit, `git add`, `git commit`     |
| 3    | Tag important commits   | `git tag -a v1.0`, `git push origin v1.0`               |
| 4    | Stash changes           | `git stash`, `git stash list`, `git stash apply`        |
| 5    | Interactive rebase      | `git rebase -i HEAD~4`, `pick`/`squash`/`edit`, `--force` |

**Flow:** `branch  →  merge + resolve  →  tag  →  stash  →  rebase -i  →  force-push`

---

## ✅ Prerequisites

- Git installed — check with `git --version`
- A terminal (bash, PowerShell, or Git Bash)
- A text editor to resolve conflicts (VS Code, nano, vim — anything)
- Basic familiarity with `git add` / `git commit`
- *(Optional)* A remote repository if you want to push tags and force-push the branch

---

## 🛠️ Steps

The commands below build one continuous session. Run them in order — every step assumes the
previous step's state.

### Step 0 — Initialize the lab repository

Create the project, make an initial commit so there is history to work with.

```bash
# === Lab: Advanced Git Techniques ===
# Initialize project
mkdir lab-advanced-git
cd lab-advanced-git
git init

# Create initial file and first commit
echo "# Advanced Git Lab" > README.md
git add README.md
git commit -m "Initial commit with README"
```

### Step 1 — Create and manage branches

Create and switch to a new branch, change a file, then stage and commit with a meaningful
message.

```bash
git checkout -b feature-new-feature
echo "Feature: Add login form" >> README.md
git add README.md
git commit -m "Add login form section in README"
```

- `git checkout -b feature-new-feature` creates the branch **and** switches to it in one shot.
- The commit message states *what* changed, not just "update".

### Step 2 — Merge branches and resolve conflicts

Edit the **same** file on both `main` and the feature branch so the merge collides, then
fix it by hand.

```bash
# Diverge main from the feature branch
git checkout main
echo "Main: Add welcome message" >> README.md
git add README.md
git commit -m "Add welcome message section in README"

# Add one more change on the feature branch
git checkout feature-new-feature
echo "Feature: Add logout info" >> README.md
git add README.md
git commit -m "Add logout info to README"

# Merge feature into main — this will conflict
git checkout main
git merge feature-new-feature || echo "Merge conflict occurred"
```

When Git reports a conflict, open `README.md`. You will see conflict markers:

```text
<<<<<<< HEAD
Main: Add welcome message
=======
Feature: Add logout info
>>>>>>> feature-new-feature
```

Decide which content to keep (or combine both), delete the `<<<<<<<`, `=======`, and
`>>>>>>>` markers, then stage and commit to complete the merge.

```bash
# Simulate manual conflict resolution
echo "Final version: login, welcome, logout" > README.md
git add README.md
git commit -m "Resolve merge conflict between feature and main"
```

### Step 3 — Use tags to mark important commits

Create an **annotated** tag to mark the current state as a release, then push it to the
remote.

```bash
git tag -a v1.0 -m "Release version 1.0"

# Push the tag to the remote (requires an 'origin' remote)
git push origin v1.0
```

- Annotated tags (`-a`) store a tagger, date, and message — use them for releases.
- `git push origin v1.0` publishes the tag; tags are **not** pushed by `git push` alone.

### Step 4 — Stash changes temporarily

Make uncommitted changes, stash them away, switch branches, inspect the stash, then bring
the work back.

```bash
git checkout feature-new-feature
echo "Uncommitted test notes" >> README.md

git stash               # save the dirty working tree, return to a clean state
git checkout main
git stash list          # show saved stashes
git stash apply         # re-apply the most recent stash onto the working tree
```

- `git stash` shelves uncommitted changes so you can switch context with a clean tree.
- `git stash apply` keeps the stash in the list; use `git stash pop` if you want to apply
  **and** drop it in one move.

### Step 5 — Perform an interactive rebase

Create four throwaway commits, then rewrite them — squash some together and reword others.

```bash
git checkout feature-new-feature
git commit --allow-empty -m "Temp commit 1"
git commit --allow-empty -m "Temp commit 2"
git commit --allow-empty -m "Temp commit 3"
git commit --allow-empty -m "Temp commit 4"

# Start an interactive rebase over the last 4 commits
git rebase -i HEAD~4
```

The rebase editor lists the four commits. Change the verb at the start of each line:

| Action   | Effect                                                |
| -------- | ----------------------------------------------------- |
| `pick`   | Keep the commit as-is                                 |
| `squash` | Merge this commit into the previous one               |
| `edit`   | Stop here so you can amend the commit                 |

```text
pick   <hash> Temp commit 1
squash <hash> Temp commit 2
edit   <hash> Temp commit 3
pick   <hash> Temp commit 4
```

Save and close the editor to continue. If you marked a commit `edit`, amend it and resume:

```bash
# If "edit" is used:
git commit --amend
git rebase --continue
```

If the rewrite collides with later changes, resolve the conflict the same way as in Step 2,
stage the file, and continue:

```bash
# Simulate conflict resolution during rebase
echo "Rebase conflict resolved" > README.md
git add README.md
git rebase --continue
```

Because rebasing **rewrites history**, the remote branch must be overwritten with a
force-push:

```bash
# Final step: force push (if remote exists)
git push origin feature-new-feature --force
```

> ⚠️ Force-pushing rewrites the remote branch. Only do it on branches **you** own. Prefer
> `--force-with-lease` over `--force` so you don't clobber someone else's pushed work.

---

## ▶️ Expected result

By the end you have a single repo where:

- `git branch` lists both `main` and `feature-new-feature`.
- `git log --oneline` on `main` shows the merge-conflict resolution commit.
- `git tag` lists `v1.0`, and it exists on the remote.
- `git stash list` is empty after the apply (or shows your stash if you skipped `pop`).
- The four `Temp commit` entries have been **collapsed/reworded** by the rebase, and the
  remote `feature-new-feature` reflects the rewritten history.

---

## ☑️ Definition of done

- [ ] `lab-advanced-git` repo initialized with an initial README commit
- [ ] `feature-new-feature` branch created, changed, and committed (Step 1)
- [ ] A real merge conflict was produced **and** resolved with a follow-up commit (Step 2)
- [ ] Annotated tag `v1.0` created and pushed to the remote (Step 3)
- [ ] Changes stashed, listed, and re-applied across branches (Step 4)
- [ ] `git rebase -i HEAD~4` completed using `pick` / `squash` / `edit`, then force-pushed (Step 5)

---

## 🔑 Key concepts

- **Branches are cheap, isolated lines of work** — `git checkout -b` lets you develop a
  feature without touching `main` until you choose to integrate it.
- **A conflict is a question, not an error** — Git pauses at the conflict markers and asks
  *you* which version wins; resolving is just editing the file, then `add` + `commit`.
- **Annotated tags are release markers** — unlike lightweight tags they carry a message and
  tagger, and they must be pushed explicitly with `git push origin <tag>`.
- **Stashing buys a clean tree on demand** — shelve unfinished work to switch context, then
  `apply` (keep) or `pop` (apply + drop) it back later.
- **Interactive rebase rewrites history** — `pick`/`squash`/`edit` let you curate commits
  before sharing, but the new hashes mean any pushed branch needs a (careful) force-push.
