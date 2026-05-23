# Flavorly

## Members

- Jerick Service - Project Manager
- Lourdyn Neil Verdida - UI/UX
- Felix Vincent Ybanez - Frontend Programmer
- Russell Ray Gillera - Backend Programmer
- Euwen Aldrich Villarin - Lead Programmer

## Overview

Flavorly is a social recipe and cooking video platform where users can create, share, and discover recipes and instructional videos. The system supports user profiles, collections, favorites, reviews, step-by-step recipe instructions, and media uploads (photos and videos). Administrators can manage content and view analytics. Flavorly is designed to help home cooks and creators showcase recipes, follow other users, save favorites, and learn through video tutorials.

## API — Controllers & Routes

This section documents the main MVC controllers, their base routes, endpoints, HTTP methods, and important behaviors.

- **AccountController** (routes under `account`)
	- `GET  /account/login` — Show login form.
	- `POST /account/login` — Perform login (`ValidateAntiForgeryToken`).
	- `GET  /account/register` — Show registration form.
	- `POST /account/register` — Register new user (`ValidateAntiForgeryToken`).
	- `POST /account/logout` — Log out (`ValidateAntiForgeryToken`).

- **AdminController** (routes under `admin`, requires `Admin` role)
	- `GET  /admin` — Dashboard (admin-only analytics and summaries).
	- `POST /admin/delete-recipe/{id}` — Remove a recipe (`ValidateAntiForgeryToken`).
	- `POST /admin/toggle-featured/{id}` — Toggle featured flag on a video (`ValidateAntiForgeryToken`).
	- `GET  /admin/settings` — View system settings.
	- `POST /admin/settings` — Save system settings (`ValidateAntiForgeryToken`).
	- `GET  /admin/recipes` — Admin recipe listing.
	- `GET  /admin/videos` — Admin video listing.
	- `GET  /admin/users` — Admin user management.
	- `POST /admin/toggle-ban/{id}` — Toggle ban/lockout on a user (`ValidateAntiForgeryToken`).
	- `POST /admin/promote/{id}` — Promote/demote user to/from Admin (`ValidateAntiForgeryToken`).
	- `POST /admin/delete-user/{id}` — Permanently delete a user (`ValidateAntiForgeryToken`).
	- `GET  /admin/reviews` — View reviews & reports.
	- `POST /admin/dismiss-report/{id}` — Dismiss a report (`ValidateAntiForgeryToken`).
	- `POST /admin/resolve-report/{id}` — Resolve a report and optionally remove offending content (`ValidateAntiForgeryToken`).
	- `POST /admin/approve-review/{id}` — Approve a review and clear reports (`ValidateAntiForgeryToken`).
	- `POST /admin/delete-review/{id}` — Delete a review and resolve associated reports (`ValidateAntiForgeryToken`).
	- `GET  /admin/analytics` — Admin analytics dashboards.

- **HomeController** (site front page and search)
	- `GET  /` (Index) — Landing / trending feed (optional `tab` query param).
	- `GET  /search?q={query}` — Search recipes.
	- `GET  /error/404` — Custom 404 page.

- **ProfileController** (routes under `profile`)
	- `GET  /profile/{username}` — View profile (recipes, videos, followers).
	- `POST /profile/{username}/follow` — Follow/unfollow a user (requires auth, `ValidateAntiForgeryToken`).
	- `POST /profile/{username}/avatar` — Upload profile avatar (requires auth, `ValidateAntiForgeryToken`).

- **RecipeController** (routes under `recipes`)
	- `GET  /recipes` — Recipe feed (filter/query params: category, difficulty, maxTime, isVeg, isVegan, isGF, sortBy, page).
	- `GET  /recipes/{id}` — Recipe detail page.
	- `POST /recipes/{id}/favorite` — Toggle favorite/save for authenticated users (`ValidateAntiForgeryToken`). Returns JSON.
	- `GET  /recipes/create` — Show create recipe form (requires auth).
	- `POST /recipes/create` — Create a new recipe, accepts hero image and process media (requires auth, `ValidateAntiForgeryToken`).
	- `POST /recipes/{id}/review` — Add a review (requires auth, `ValidateAntiForgeryToken`).

- **VideoController** (routes under `videos`)
	- `GET  /videos` — Video feed (category filter).
	- `GET  /videos/{id}` — Video player page.

Notes:
- Many POST endpoints include `[ValidateAntiForgeryToken]` and require authenticated users for state-changing operations.
- `AdminController` is protected with `[Authorize(Roles = "Admin")]`.

## Data, Repositories & DB

- The application uses `FlavorlyDbContext` (EF Core) for persistence (models in `Models/`).
- Repositories: `IRecipeRepository`, `IVideoRepository`, and implementations are used to abstract data access for recipes and videos.

## Media Uploads and Limits

- Profile avatars and recipe media are saved to `wwwroot/uploads` (and subfolders).
- Allowed image extensions: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`.
- Allowed video extensions: `.mp4`, `.webm`, `.mov`, `.ogg`.
- Max sizes in `RecipeController` for process media: images up to 10 MB, videos up to 100 MB.

If you'd like, I can also add example request samples (curl) for key endpoints, or generate an OpenAPI/Swagger draft based on these controllers.

## Project Structure

Top-level layout of the workspace (key files and folders):

```
INTPROG_FINAL-main/
├─ FINALPROJECT.slnx
├─ .gitignore
├─ README.md
├─ FINALPROJECT/
│  ├─ Program.cs
│  ├─ FINALPROJECT.csproj
│  ├─ appsettings.json
│  ├─ appsettings.Development.json
│  ├─ Properties/
│  │  └─ launchSettings.json
│  ├─ Controllers/
│  │  ├─ AccountController.cs
│  │  ├─ AdminController.cs
│  │  ├─ HomeController.cs
│  │  ├─ ProfileController.cs
│  │  ├─ RecipeController.cs
│  │  └─ VideoController.cs
│  ├─ Data/
│  │  └─ FlavorlyDbContext.cs
│  ├─ Helpers/
│  │  └─ UserMediaHelper.cs
│  ├─ Migrations/
│  │  └─ (EF Core migration files)
│  ├─ Models/
│  │  ├─ User.cs
│  │  ├─ Recipe.cs
│  │  ├─ Video.cs
│  │  └─ (other model classes)
│  ├─ Repositories/
│  │  ├─ IRecipeRepository.cs
│  │  ├─ IVide oRepository.cs
│  │  └─ RecipeRepository.cs
│  ├─ ViewModels/
│  │  └─ (view model classes)
│  ├─ Views/
│  │  ├─ Account/
│  │  ├─ Admin/
│  │  ├─ Home/
│  │  ├─ Profile/
│  │  ├─ Recipe/
│  │  └─ Video/
│  └─ wwwroot/
│     ├─ css/
│     ├─ js/
│     ├─ images/
│     ├─ lib/    (third-party libs like bootstrap, jquery)
│     └─ uploads/ (user-uploaded media)
```

This is a condensed view — let me know if you want a full recursive listing or a Markdown table with file counts per folder.
