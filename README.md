# Analytics Dashboard

A full-stack dashboard for managing articles and tracking their performance: views, time on
page, and bounce rate. Includes full CRUD content management, tags/authors/campaigns as
many-to-many relationships, and role-based access control (Viewer / Editor / Admin).

**Stack**

- Frontend: React 19, TypeScript, Vite, React Router, Recharts, Axios
- Backend: ASP.NET Core 8 Web API, EF Core with Npgsql, JWT authentication
- Database: PostgreSQL

## Prerequisites

- .NET 8 SDK
- Node.js 18 or newer (includes npm)
- Docker Desktop (used to run PostgreSQL) or a local PostgreSQL instance
- EF Core CLI tool, installed once with `dotnet tool install --global dotnet-ef`

## Getting started

### 1. Database

```bash
docker compose up -d
```

This starts a PostgreSQL 16 container with the database `analytics_dashboard`, exposed on host
port 5433. The port is mapped to 5433 rather than the default 5432 to avoid colliding with a
local PostgreSQL installation that may already be running on this machine; it does not affect
anything running inside the container.

If you're using your own PostgreSQL instance instead of Docker, update
`ConnectionStrings:DefaultConnection` in `backend/AnalyticsDashboard.Api/appsettings.json`
to match.

### 2. Backend

```bash
cd backend/AnalyticsDashboard.Api
dotnet restore
dotnet run
```

On startup, the API applies any pending EF Core migrations and seeds the database if it's
empty. Seeding is idempotent, so restarting the API won't create duplicate data.

The API runs at `http://localhost:5080`, with Swagger UI available at
`http://localhost:5080/swagger` in development.

### 3. Frontend

```bash
cd frontend
npm install
npm run dev
```

Runs at `http://localhost:5173` and talks to the API at `http://localhost:5080/api` by
default. To point it elsewhere, copy `.env.example` to `.env` and set `VITE_API_URL`.

### Demo accounts

The seeded database includes one account per role:

| Username | Password    | Role   |
|----------|-------------|--------|
| admin    | Admin@123   | Admin  |
| editor   | Editor@123  | Editor |
| viewer   | Viewer@123  | Viewer |

Seeded data also includes 26 articles across three categories, 10 tags, 5 authors, 3
campaigns, and 30,000-60,000 pageviews spread over the last 90 days. Seeding can be turned off
by setting `Seed:EnabledOnStartup` to `false` in `appsettings.json`.

## Project structure

```
backend/AnalyticsDashboard.Api/
  Models/         EF Core entities (Article, Pageview, Tag, Author, Campaign, User, ...)
  Data/           DbContext, per-entity Fluent API configurations, DbSeeder
  DTOs/           Request/response contracts, grouped by resource
  Services/       Business logic (Interfaces/ + Implementations/), one per resource
  Controllers/    REST controllers handling auth, validation, and RBAC
  Common/         Roles constants, ApiException, global exception-handling middleware

frontend/src/
  api/            Axios client plus one module per resource
  context/        AuthContext: JWT storage, current user, login/register/logout
  components/     Shared UI: Navbar, ProtectedRoute, DateRangeFilter, UsersPanel, ...
  pages/          One component per route (Dashboard, Insights, Articles, Manage, ...)
  types/          Shared TypeScript interfaces mirroring the backend DTOs
```

## Data model

`articles` has a one-to-one relationship with `article_details` and a one-to-many
relationship with `pageviews`. Three junction tables handle many-to-many relationships:
`article_tags`, `article_authors`, and `article_campaigns`.

## Roles

- **Viewer** - read-only access to articles, pageviews, and analytics.
- **Editor** - everything a Viewer can do, plus creating and updating articles, details,
  tags, authors, campaigns, and their associations.
- **Admin** - everything an Editor can do, plus delete operations and user role management.

The first account ever registered is automatically promoted to Admin. Every account after
that starts as Viewer; an Admin can change roles from Manage > Users & Roles.

## API reference

| Area | Endpoints |
|---|---|
| Auth | `POST /api/auth/register`, `POST /api/auth/login` |
| Users (Admin) | `GET /api/users`, `PUT /api/users/{id}/role` |
| Articles | `GET/POST /api/articles`, `GET/PUT/DELETE /api/articles/{id}`, `PUT /api/articles/{id}/details`, `PUT /api/articles/{id}/tags\|authors\|campaigns` |
| Pageviews | `GET/POST /api/pageviews`, `DELETE /api/pageviews/{id}` |
| Tags / Authors / Campaigns | `GET/POST /api/{resource}`, `DELETE /api/{resource}/{id}` |
| Analytics | `GET /api/analytics/kpis\|daily-views\|top-articles\|top-tags\|author-performance\|campaign-impact` |

Full interactive documentation, including an authenticated "try it out" console, is available
through Swagger UI once the API is running.
