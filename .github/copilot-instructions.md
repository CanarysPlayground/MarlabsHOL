# Copilot instructions for leave-manager

## Backend (src/LeaveManager.Api)
- .NET 8 Web API, EF Core + SQLite, xUnit. Controllers stay thin; rules live in `Services/`.
- Business-rule violations: throw `LeaveRuleViolationException("reason")`; `Program.cs` maps it to 400 ProblemDetails.
- The signed-in user comes from `ICurrentUserService` (X-Demo-User header). Do not add real authentication.
- Do not change the data model or `Program.cs` unless the task says so.
- Tests use `TestDbFactory` (in-memory SQLite, seeded). Name tests `Method_Scenario_Outcome`.

## Frontend (src/leave-manager-ui)
- Angular 18 standalone components, signals for state, `inject()` for DI, OnPush.
- HTTP goes through `MyRequestsService` (thin typed methods). Components never call HttpClient directly.
- Styling uses the CSS custom properties in `src/styles/_tokens.scss`. Never hard-code hex colours.
- Statuses render with `<lm-status-chip>`.
- Tests are Jasmine/Karma specs next to the component.

## Work tracking
- Commit messages start with the Azure DevOps work item id, e.g. `#101 Validate overlap and balance`.
