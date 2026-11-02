# BuildWorkflow Connect

BuildWorkflow Connect is a lightweight, self-hosted HTTP API client and tester built with ASP.NET Core.
It provides a Postman-like web UI for organizing, saving, and sending HTTP requests, with support for
environments, variables, collections, authentication, and request bodies. Responses are proxied through
the server to avoid browser CORS restrictions.

## Features

- **Request builder** — choose an HTTP method (GET, POST, PUT, PATCH, DELETE, OPTIONS, HEAD), enter a
  URL, and configure query params, headers, and body.
- **Body types** — `none`, JSON, plain text, and `application/x-www-form-urlencoded`.
- **Authentication** — none, Bearer token, Basic auth, and API key (in header or query string).
- **Environments** — create multiple environments with key/value variables. Use `{{variableName}}`
  placeholders in URLs, params, headers, bodies, and auth fields; they are substituted at send time
  from the active environment.
- **Collections / groups** — organize requests into named groups, or keep them ungrouped.
- **Persistence** — requests, groups, and environments are stored in a local SQLite database
  (`apitester.db`).
- **Response viewer** — shows status code, status text, duration, response headers, formatted body,
  and raw body, with JSON syntax highlighting and a "Prettify JSON" helper.
- **Proxy execution** — requests are sent by the server (`/api/proxy/execute`), so calls to APIs
  with strict CORS policies still work from the browser UI.
- **Reset app** — a danger-styled button clears all environments, groups, and requests after
  confirming the reset password stored in `appsettings.json`.

## Tech stack

- .NET 10 / ASP.NET Core MVC
- Entity Framework Core with SQLite provider
- Vanilla JavaScript front end (no build step) styled with Bootstrap 5
- Google Fonts (Roboto / Roboto Mono)

## Prerequisites

- .NET SDK 10.0 or later
- (Optional) An IDE that supports ASP.NET Core, such as Visual Studio 2022+ or VS Code with the C# extension

## Getting started

```bash
# Restore and build
dotnet build

# Run the application
dotnet run
```

By default the app starts on `http://localhost:5127` (see `Properties/launchSettings.json`).
Open that URL in a browser. The SQLite database file `apitester.db` is created automatically on
first run.

To use HTTPS, run with the `https` profile:

```bash
dotnet run --launch-profile https
```

## Configuration

Connection string (SQLite database path) is read from `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=apitester.db"
  }
}
```

Override it with the `ConnectionStrings__Default` environment variable or in
`appsettings.Development.json` for local development.

The reset password is configured under `Reset:Password`:

```json
{
  "Reset": {
    "Password": "changeme"
  }
}
```

Reset is disabled when this value is empty.

## Project structure

```
BuildWorkflowConnect/
├── BuildWorkflowConnect.csproj
├── Program.cs                      # App bootstrap, DI, middleware, routing
├── Controllers/
│   ├── HomeController.cs           # MVC pages (Index = API tester UI)
│   ├── EnvironmentsController.cs   # CRUD for environments
│   ├── GroupsController.cs        # CRUD for request groups/collections
│   ├── RequestsController.cs      # CRUD for saved requests
│   └── ProxyController.cs         # Executes HTTP requests server-side
├── Data/
│   └── AppDbContext.cs             # EF Core DbContext
├── Models/
│   ├── Entities.cs                 # EnvironmentEntity, EnvironmentVariable, ApiGroup, ApiRequest
│   └── ProxyDtos.cs                # ProxyExecuteRequest, ProxyResult
├── Views/
│   ├── Home/
│   │   └── Index.cshtml            # Single-page API tester UI
│   └── Shared/
│       └── _Layout.cshtml
├── wwwroot/                        # Static assets (CSS, JS, libs)
├── appsettings.json
└── README.md
```

## Data model

- `EnvironmentEntity` — an environment with a name and its `EnvironmentVariable` entries.
- `ApiGroup` — a named collection; owns zero or more `ApiRequest` records.
- `ApiRequest` — a saved request: method, URL, body type/body, headers and params (stored as JSON),
  auth type/payload (stored as JSON), and optional group id.

Relationships use cascade delete (deleting a group removes its requests; deleting an environment
removes its variables). The database is created with `EnsureCreated()`; no EF Core migrations are
used.

## HTTP API

| Endpoint | Description |
| --- | --- |
| `GET /api/environments` | List all environments (with variables). |
| `POST /api/environments` | Create an environment. |
| `PUT /api/environments/{id}` | Replace an environment and its variables. |
| `DELETE /api/environments/{id}` | Delete an environment. |
| `GET /api/groups` | List all groups (with requests). |
| `POST /api/groups` | Create a group. |
| `PUT /api/groups/{id}` | Rename a group. |
| `DELETE /api/groups/{id}` | Delete a group and its requests. |
| `GET /api/requests?groupId=` | List requests, optionally filtered by group. |
| `POST /api/requests` | Create a request. |
| `PUT /api/requests/{id}` | Update a request. |
| `DELETE /api/requests/{id}` | Delete a request. |
| `POST /api/proxy/execute` | Execute an HTTP request and return status, headers, body, and duration. |
| `POST /api/admin/reset` | Delete all environments, groups, and requests. Requires `{ "password": "..." }`. |

## Security and limitations

- The proxy endpoint fetches arbitrary URLs supplied by the client. This tool is intended for
  **local development and personal use only**; do not expose it to untrusted users or the public
  internet without adding authentication, URL allow-listing, and rate limiting.
- Secrets stored in environments (tokens, passwords) are kept in the local SQLite database in plain
  text. Protect the database file accordingly.
- No user authentication or multi-user isolation is included.

## License

No license file is included in this repository; add one if you intend to distribute the project.