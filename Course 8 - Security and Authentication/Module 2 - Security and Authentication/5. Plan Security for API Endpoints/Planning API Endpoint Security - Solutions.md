# Planning Security for API Endpoints with JWT Validation — Solutions

> Each route is planned on three axes: **JWT validation** (authentication — *who you are*),
> **roles** (coarse authorization — *what type of thing you may do*), and **claims** (fine
> authorization — *which specific record you may touch*). A note on reading the claim column:
> "must include the user ID" means the caller's own identity from the token is compared
> against the resource's owner — the token does not carry a list of every record the caller
> may reach.

---

## Task 1: Social Media Platform

### Scenario

A social media platform exposes:
1. `GET /users/{id}` — Returns profile information for a specific user.
2. `POST /posts` — Creates a new post.
3. `DELETE /posts/{id}` — Deletes a specific post.

---

### 1. Identify Which Routes Require JWT Validation

- **`GET /users/{id}` — JWT required.** A profile contains personal information and is subject
  to privacy settings and block lists, so the platform must know *who is asking* before
  deciding what to return. Requiring a valid token also prevents anonymous scraping of the
  user base. *(Design note: if the platform intentionally publishes fully public profiles, a
  trimmed version of this route could be `AllowAnonymous`; the secure default is to
  authenticate and then filter fields by the viewer's relationship to the target.)*

- **`POST /posts` — JWT required.** Creating content is a write operation that must be tied to
  a real, authenticated author. Anonymous posting invites spam and abuse and makes the author
  un-attributable.

- **`DELETE /posts/{id}` — JWT required.** Deletion is a destructive write on a specific
  record; only an authenticated user may attempt it, and only the owner or a moderator may
  succeed.

> **All three routes require JWT validation.** Unlike an e-commerce product catalog, none of
> these operations is safely anonymous.

---

### 2. Specify Roles or Claims for Each Route

- **`GET /users/{id}`**
  - **Roles:** `user` (any authenticated user). Viewing profiles is the core of a social
    network, so this route is intentionally **not** owner-restricted — you view *other
    people's* profiles, not only your own.
  - **Claims:** the caller's user ID, used to apply relationship rules (privacy settings,
    blocks, friends-only fields) — not to restrict to the owner. This is the key contrast with
    `GET /orders/{id}` in the worked example, which *is* owner-only.

- **`POST /posts`**
  - **Roles:** `user`.
  - **Claims:** must include the caller's user ID, which is used to set the new post's author.
    The client must never be trusted to send an arbitrary `authorId` — it is taken from the
    token so a post can only be created on the caller's own behalf.

- **`DELETE /posts/{id}`**
  - **Roles:** `user` (as the post's **owner**) **or** `moderator` / `admin`.
  - **Claims:** the caller's user ID must match the post's `authorId` **or** the caller must
    hold a moderation role. This is the "owner *or* elevated role" pattern: authors manage
    their own content, while moderators remove anyone's content for policy enforcement.

---

### 3. Security Plan Summary Table

| Route               | JWT validation | Roles                              | Claims                                                            |
| ------------------- | -------------- | ---------------------------------- | ---------------------------------------------------------------- |
| `GET /users/{id}`    | Required       | `user` (any authenticated)         | Caller's user ID applies privacy/block rules. **Not** owner-restricted — users view others' profiles. |
| `POST /posts`        | Required       | `user`                             | Must include the caller's user ID to set the post's author.      |
| `DELETE /posts/{id}` | Required       | `user` (owner) **or** `moderator`/`admin` | Caller's ID must match the post's author, **or** caller holds a moderation role. |

---

### Explanation of the Design Process

- **Why these JWT decisions?** Every route either exposes personal data or performs a write,
  so none is a candidate for anonymous access — the secure default here is authenticate-first,
  the opposite of a public product catalog.
- **Why these roles and claims?** `GET /users/{id}` deliberately stays open to any
  authenticated user because a social graph exists to view *others* — owner-scoping it would
  break the product. `POST` and `DELETE`, being writes, tighten access: the author is taken
  from the token (never the request body), and deletion adds the "owner or moderator" rule so
  content moderation is possible without letting users delete each other's posts.
- **How does this protect data?** The author claim prevents impersonation (you can't post or
  delete as someone else), the ownership check on delete prevents tampering with others'
  content, and JWT validation keeps the entire surface off-limits to anonymous abuse —
  defense in depth across all three routes.

---

---

## Task 2: Learning Management System (LMS)

### Scenario

An LMS exposes:
1. `GET /courses` — Lists all available courses.
2. `POST /assignments` — Creates a new assignment.
3. `GET /grades/{id}` — Returns grades for a specific student.

---

### 1. Identify Which Routes Require JWT Validation

- **`GET /courses` — JWT not required (public).** A catalog of available courses is marketing
  surface meant to be browsable by prospective students. Like `GET /products` in the worked
  example, it carries no sensitive data and can be served anonymously. *(Design note: if
  courses are private to an enrolled institution, this becomes `[Authorize]` and lists only
  the caller's available courses — but the default LMS catalog is public.)*

- **`POST /assignments` — JWT required.** Authoring an assignment is a privileged write; it
  must be restricted to authenticated staff who own the relevant course.

- **`GET /grades/{id}` — JWT required.** Grades are highly sensitive academic records
  (protected under privacy regulations such as FERPA). Access must be authenticated and then
  tightly scoped.

> **Two of three routes require JWT validation;** the public catalog mirrors the e-commerce
> products example.

---

### 2. Specify Roles or Claims for Each Route

- **`GET /courses`**
  - **Roles:** none — public.
  - **Claims:** none.

- **`POST /assignments`**
  - **Roles:** `instructor`, `admin`. Students must never create assignments.
  - **Claims:** the instructor must own/teach the target course — the course's owner is
    checked against the caller's ID (or their teaching assignments). An `admin` may create for
    any course. This is a *resource-ownership* check on the course the assignment belongs to.

- **`GET /grades/{id}`**
  - **Roles:** `student` (the owner of the record), `instructor`, `admin`.
  - **Claims:**
    - `student` — the caller's user ID must match `{id}`; a student sees **only their own**
      grades.
    - `instructor` — limited to students enrolled in courses they teach (relationship check),
      not the entire student body.
    - `admin` — full access for administrative purposes.

---

### 3. Security Plan Summary Table

| Route                | JWT validation | Roles                          | Claims                                                                 |
| -------------------- | -------------- | ------------------------------ | --------------------------------------------------------------------- |
| `GET /courses`        | Not required   | None (public)                  | None.                                                                 |
| `POST /assignments`   | Required       | `instructor`, `admin`          | Instructor must own the target course; admin unrestricted.            |
| `GET /grades/{id}`    | Required       | `student`, `instructor`, `admin` | Student: ID must match `{id}`. Instructor: own students only. Admin: full. |

---

### Explanation of the Design Process

- **Why these JWT decisions?** The catalog is public to encourage discovery and enrollment,
  exactly like a product list. Everything that authors content or exposes grades is gated,
  because those operations involve privileged writes or regulated personal data.
- **Why these roles and claims?** `POST /assignments` shows **role + resource ownership**: the
  `instructor` role gets you to the route, but a course-ownership check stops one instructor
  from adding assignments to another's course. `GET /grades/{id}` shows **one resource, three
  scopes**: the same route serves students (own grades only), instructors (their own students
  only), and admins (all) — each role narrowed by a different claim/relationship rule.
- **How does this protect data?** Grades — the most sensitive asset — are reachable only by
  the student they belong to, the instructor responsible for them, or an admin, satisfying
  least privilege and privacy regulations (FERPA). Assignment authoring is walled off from
  students and scoped to course ownership, preventing both privilege escalation and
  cross-course tampering, while the public catalog deliberately exposes nothing sensitive.

---

## Enforcement at a glance (ASP.NET Core)

```csharp
// GET /courses  — public
[AllowAnonymous]
[HttpGet("/courses")]
public IActionResult ListCourses() => Ok(_catalog.All());

// POST /assignments — instructors or admins; ownership checked imperatively
[Authorize(Roles = "instructor,admin")]
[HttpPost("/assignments")]
public IActionResult Create(CreateAssignment dto)
{
    var callerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    if (!User.IsInRole("admin") && !_courses.IsOwnedBy(dto.CourseId, callerId))
        return Forbid();                       // axis 3: resource ownership
    return Ok(_assignments.Create(dto));
}

// GET /grades/{id} — self, instructor-of-record, or admin
[Authorize(Roles = "student,instructor,admin")]
[HttpGet("/grades/{id}")]
public IActionResult Grades(string id)
{
    var callerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    if (User.IsInRole("student") && callerId != id) return Forbid();  // own grades only
    if (User.IsInRole("instructor") && !_enroll.Teaches(callerId, id)) return Forbid();
    return Ok(_grades.For(id));
}
```

The **role gate** is declarative (`[Authorize(Roles = …)]`); the **ownership/claim check**
(axis 3) is imperative because the framework can't know who owns record `{id}` until it loads
it. For reuse, the same ownership logic can move into a resource-based `IAuthorizationHandler`
behind a named policy.
