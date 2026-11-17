# Leave & Time-Off Manager — GitHub Copilot Hands-on Workshop starter

Starter repository for the four-day GitHub Copilot hands-on labs (.NET 8 + Angular 18).
The lab guide is in [docs/HOL.md](docs/HOL.md). **Read the lab guide first; this file only gets the app running.**

## What works, what is missing (on purpose)

| Area | Starter state | Built in |
| --- | --- | --- |
| `GET /api/leave-requests`, `GET /api/leave-requests/pending` | Done | — |
| `POST /api/leave-requests` | Done, **no validation** | Day 1 |
| `PATCH /api/leave-requests/{id}/decision` | Missing | Day 2 |
| `GET /api/leave/team-calendar?month=` | Missing | Day 3 Part A |
| `GET /api/holidays`, working-day count | Missing | Day 3 Part B |
| Angular New Request form | Posts, ignores errors | Day 1 |
| Angular Approval Queue | Read-only | Day 2 |
| Angular Team Calendar | Empty shell | Day 3 Part A |
| Tests (`tests/LeaveManager.Api.Tests`) | Fixture + 1 smoke test | Days 1–3 |
| `.github/agents/story-implementer.agent.md` | Title only | Day 2 |
| `.vscode/mcp.json` (Azure DevOps, Figma) | Present, disabled | Days 2, 3 |
| `azure-pipelines.yml` | Build only, **fails on first run** | Day 4 |

## Prerequisites

- .NET 8 SDK — `dotnet --version` prints 8.x
- Node 20 LTS and npm — `node --version`
- VS Code with **GitHub Copilot** and **GitHub Copilot Chat** (signed in to the workshop org) — recommended extensions are in `.vscode/extensions.json`
- Azure DevOps access to the workshop project and a PAT (Day 2 onwards)
- Figma viewer access to **Leave Manager UI** and a Figma token (Day 3)

## Run the API

```bash
cd src/LeaveManager.Api
dotnet dev-certs https --trust     # once, so the browser accepts https://localhost:7043
dotnet run
```

- Swagger UI: https://localhost:7043/swagger
- The SQLite database `leave.db` is created and seeded on first start. Delete the file to reset the data.

## Run the UI

```bash
cd src/leave-manager-ui
npm ci
npm start                           # http://localhost:4200
```

## Run the tests

```bash
dotnet test                         # from the repo root
cd src/leave-manager-ui && npm test # Jasmine/Karma, needs Chrome
```

## Demo users

Sign-in is a header (`X-Demo-User`) set by the Angular interceptor. Switch users from the top bar.

| Email | Team | Role | Reports to | Notes |
| --- | --- | --- | --- | --- |
| alice@demo | Engineering | Employee | manager@demo | Approved leave 21–24 Dec 2026 |
| bob@demo | Engineering | Employee | manager@demo | **2** annual days left; pending request 1–2 Dec 2026 |
| carol@demo | Finance | Employee | cfo@demo | — |
| manager@demo | Engineering | Manager | — | Sees Alice's and Bob's requests in the Approval Queue |
| cfo@demo | Finance | Manager | — | Sees Carol's requests |

Holidays seeded: 25 Dec 2026, 26 Dec 2026, 1 Jan 2027; blackout 31 Dec 2026.

## Repo layout

```
leave-manager/
  src/LeaveManager.Api/            .NET 8 Web API, EF Core + SQLite
    Controllers/                   LeaveRequestsController, LeaveBalancesController
    Services/                      LeaveRequestService, LeaveBalanceService, CurrentUserService
    Data/                          LeaveDbContext, SeedData
    Program.cs                     ProblemDetails mapping; `public partial class Program {}`
  src/leave-manager-ui/            Angular 18, standalone, signals, Jasmine/Karma
    src/app/my-requests/ new-request/ approval-queue/ team-calendar/
    src/app/shared/status-chip/    shared StatusChipComponent
    src/styles/_tokens.scss        Figma colour and spacing tokens
  tests/LeaveManager.Api.Tests/    xUnit, TestDbFactory (in-memory SQLite)
  .github/agents/                  custom agent stub (Day 2)
  .github/copilot-instructions.md  repo-wide Copilot instructions
  .vscode/mcp.json                 Azure DevOps and Figma MCP servers (disabled)
  azure-pipelines.yml              Build stage only, planted failure (Day 4)
  docs/HOL.md                      the lab guide
```

## Conventions Copilot is told to follow

See `.github/copilot-instructions.md`. In short: rules in services, `LeaveRuleViolationException` → 400 ProblemDetails, signals + `inject()`, tokens not hex, `<lm-status-chip>` for statuses, commit messages start with the work item id.
