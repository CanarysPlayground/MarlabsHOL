# GitHub Copilot Hands-on Labs — Leave & Time-Off Manager (.NET + Angular)

Version 1.0 · 17 Nov 2026

## About this guide

Four hands-on labs (HOLs) on one sample application, the **Leave & Time-Off Manager**, spread over four days with a total of 2 hours 30 minutes of lab time. Every lab has a start state, timed steps, a done state, and a stretch task for fast finishers.

| Day | HOL | Time | Participant builds | Copilot capability exercised |
| --- | --- | --- | --- | --- |
| 1 | Validation story with Plan, Agent and Test | 30 min | Overlap and balance checks on `POST /api/leave-requests`; error shown on the Angular form | Plan mode, Agent mode, `/tests`, inline chat |
| 2 | Custom agent reads an Azure DevOps work item | 30 min | `PATCH /api/leave-requests/{id}/decision` with manager-only rule; Approve/Reject in the queue | Custom agent (`.agent.md`), Azure DevOps MCP, approval gate |
| 3 | Figma to Angular, then team mini-challenge | 60 min | Team Calendar component from the Figma frame (30 min); holidays/blackout rule with AI tests and AI code review (30 min) | Figma MCP, design tokens, `/tests`, Copilot code review |
| 4 | Azure Pipelines YAML and troubleshooting | 30 min | Build-test-deploy pipeline; fix the planted failing run | Copilot in Azure DevOps YAML, log analysis in chat |

**Stack (fixed for all labs):** .NET 8 Web API, EF Core + SQLite, xUnit · Angular 18 standalone components, signals, Jasmine/Karma · Azure DevOps (Boards, Repos, Pipelines) · Figma · VS Code with GitHub Copilot (Business or Enterprise) and Copilot Chat.

**Before Day 1, each participant must have:**

- [ ] VS Code with the GitHub Copilot and GitHub Copilot Chat extensions signed in to the workshop org
- [ ] .NET 8 SDK, Node 20 LTS and Angular CLI installed; `dotnet --version` and `ng version` run clean
- [ ] Cloned the starter repo `leave-manager` and run `dotnet run` (API on `https://localhost:7043`) and `ng serve` (UI on `http://localhost:4200`) once
- [ ] Contributor access to the Azure DevOps project **Leave-Manager-Workshop** with a Personal Access Token (Work Items: Read & Write, Code: Read, Build: Read & Execute)
- [ ] Viewer access to the Figma file **Leave Manager UI** and a Figma personal access token (Day 3 only)

**Starter repo state:** `GET` and `POST /api/leave-requests` work; validation, the decision endpoint, the team calendar, holidays, tests and the pipeline are deliberately missing. Each lab ends when its missing piece exists and its tests pass.

**Roles in the room:** one trainer leads; one helper per 6 participants watches for blockers. Participants work solo on Days 1, 2 and 4 and in teams of three (.NET, Angular, QA) for the second half of Day 3.

## Day 1 HOL — Implement a story with Plan, Agent and Test (30 min)

**Story (ADO #101):** Reject leave requests that overlap existing leave or exceed the balance.

**Acceptance criteria:** `POST /api/leave-requests` returns `400 ProblemDetails` with a clear reason for an overlap or an insufficient balance; the Angular New Request form shows the error and the remaining balance; unit tests cover both rules.

**Start state:** `LeaveRequestService.CreateAsync` saves any request without checks. `new-request.component.ts` posts the form and ignores non-2xx responses. The test project has `TestDbFactory` and one smoke test only.

**Done state:** two new tests in `LeaveManager.Api.Tests` pass, `dotnet test` is green, and submitting an overlapping request in the UI shows the red error banner with the balance.

### Steps

| # | Time | Step | What to type or click |
| --- | --- | --- | --- |
| 1 | 0–3 min | Open the repo, read the story | Open `leave-manager` in VS Code. In Copilot Chat ask: `@workspace Where is a leave request created and where would validation belong?` Expect it to point at `LeaveRequestService.CreateAsync`. |
| 2 | 3–8 min | **Plan** | Switch Copilot Chat to **Plan** mode. Prompt: `Plan the change to reject a leave request that overlaps an existing Pending or Approved request for the same employee, or whose working-day count exceeds LeaveBalance.DaysRemaining. Throw LeaveRuleViolationException so the API returns 400 ProblemDetails. Do not implement yet.` Read the plan; confirm it touches the service, ProblemDetails, and tests. Edit the plan if it proposes a new table. |
| 3 | 8–16 min | **Agent** implements the API | Switch to **Agent** mode. Prompt: `Implement the approved plan. Use the existing LeaveRuleViolationException and ProblemDetails handler in Program.cs. Keep the controller thin.` Review each diff before accepting. Run `dotnet build`. |
| 4 | 16–21 min | **Test** | Open `LeaveRequestServiceTests.cs`. Use `/tests` on `CreateAsync`: `Generate xUnit tests for the overlap rule and the balance rule using TestDbFactory (seeded in-memory SQLite). Use alice@demo for overlap and bob@demo (2 annual days) for balance.` Run `dotnet test`. Fix any failing assertion with inline chat (`Ctrl+I`). |
| 5 | 21–27 min | Angular form | Open `new-request.component.ts`. Agent prompt: `On a 400 ProblemDetails response, show problem.detail in the existing error banner and display the remaining balance from MyRequestsService.getMyBalances() beside the date pickers. Use signals, no new dependencies.` Run `ng serve`, submit an overlapping request, see the banner. |
| 6 | 27–30 min | Commit and link | Commit with message `#101 Validate overlap and balance`. Push. Confirm the work item moves to **Active** in Azure DevOps. |

### Trainer checkpoints

- At step 2, stop everyone and compare two plans on screen: plan quality depends on the prompt naming the entities (`Pending`, `Approved`, `DaysRemaining`).
- At step 4, ask who got a test that passed first time; show how a vague `/tests` prompt produces tests that mock the wrong layer.

### Stretch (if finished early)

- Ask Agent to add a `LeaveRequestValidator` using FluentValidation and move the two rules there, keeping tests green.
- Ask `/tests` for an Angular Jasmine spec that asserts the banner renders on a 400 response.

### Common blockers

| Symptom | Fix |
| --- | --- |
| Plan mode missing from the chat mode dropdown | Update the Copilot Chat extension; Plan requires the current pre-release or stable 1.100+. |
| Agent edits `Program.cs` and breaks startup | Reject that edit; re-prompt with `Do not change Program.cs.` |
| `dotnet test` cannot find SQLite | Run `dotnet restore`; `Microsoft.Data.Sqlite` is already in the test csproj. |

## Day 2 HOL — Custom agent reads the Azure DevOps work item through MCP (30 min)

**Story (ADO #102):** Manager approves or rejects a pending request with a comment.

**Acceptance criteria:** `PATCH /api/leave-requests/{id}/decision` accepts `{ decision: "Approved" | "Rejected", comment }`; only the employee's line manager may decide; a decided request cannot be changed again; balance is reduced on approval; the Angular Approval Queue has Approve and Reject actions.

**Start state:** no decision endpoint (a `DecisionDto` already exists in `Dtos.cs`); `approval-queue.component.ts` lists pending requests read-only. The Azure DevOps MCP server is configured in `.vscode/mcp.json` but disabled. A starter custom agent file exists at `.github/agents/story-implementer.agent.md` with only a title.

**Done state:** the custom agent fetched #102 from Azure DevOps on its own, posted a plan, paused for your approval, then implemented the endpoint, the queue actions and three passing tests. The work item has a comment from the agent and is in **Resolved**.

### Steps

| # | Time | Step | What to type or click |
| --- | --- | --- | --- |
| 1 | 0–4 min | Enable the ADO MCP server | Open `.vscode/mcp.json`. Replace `YOUR-ADO-ORG-NAME` with the org name and set `"disabled": false` on `azure-devops`. When prompted, paste your PAT. In Copilot Chat click the **Tools** icon and confirm the work-item tools (e.g. `wit_get_work_item`, `wit_add_work_item_comment`, `wit_update_work_item`) are listed. Tool names vary by server version — use the names shown. |
| 2 | 4–10 min | Write the custom agent | Open `story-implementer.agent.md`. Paste the agent definition from Appendix D, replacing the tool names with the ones shown in your Tools picker. Key parts: the `tools:` list, a **Plan → wait for the word APPROVE → Implement → Test → Comment** instruction, and `Never change the data model without asking.` Save; the agent appears in the chat mode dropdown as **Story Implementer**. |
| 3 | 10–13 min | Run the agent on #102 | Select **Story Implementer**. Prompt: `Implement work item 102.` Watch it call the get-work-item tool, read the acceptance criteria, and print a numbered plan. It must stop and ask for approval. |
| 4 | 13–15 min | Review and approve | Check the plan covers: manager check from `Employee.ManagerId`, `Status != Pending` guard returning 409, balance decrement inside one transaction, Angular actions. Type `APPROVE` (or correct it first, e.g. `Use 409 Conflict for an already-decided request, then proceed.`). |
| 5 | 15–24 min | Agent implements and tests | Accept diffs as they arrive. The agent runs `dotnet test`; expect `DecideAsync_NonManager_Returns403`, `DecideAsync_AlreadyDecided_Returns409`, `DecideAsync_Approved_ReducesBalance`. In the UI, switch to **manager@demo**, open **Approval Queue** and approve Bob's request. |
| 6 | 24–28 min | Close the loop in ADO | The agent adds a comment with a summary and sets the state to **Resolved**. Open the work item in the browser and confirm. |
| 7 | 28–30 min | Commit | `#102 Manager decision endpoint and queue actions`. Push. |

### Trainer checkpoints

- At step 3, project one participant's screen: the point of the lab is that the **requirements came from the work item, not from the prompt**.
- At step 4, ask: what would you remove from the agent's tool list in production? (Answer to draw out: work-item state changes should need a human.)

### Stretch

- Add the create-work-item tool to the agent and tell it: `If you find a bug outside this story's scope, create a Bug work item linked to 102 instead of fixing it.` Plant one by asking it to review `LeaveBalanceService`.
- Add a second custom agent `reviewer.agent.md` with read-only tools that comments on the diff.

### Common blockers

| Symptom | Fix |
| --- | --- |
| MCP server shows a red dot | PAT missing **Work Items: Read & Write** scope, or the org name in `mcp.json` is wrong (use the name only, not the URL). |
| Agent implements without waiting | The approval instruction must be in the agent body, not only in the description; wording that works: `After presenting the plan, STOP. Do not edit files until the user replies with the single word APPROVE.` |
| Agent cannot see the ADO tools | The `tools:` list in the agent file must use the exact tool names shown in the Tools picker. |

## Day 3 HOL — Figma to Angular, then team mini-challenge (60 min)

Two halves: **Part A (0–30 min, solo)** builds the Team Calendar screen from the Figma frame through the Figma MCP server. **Part B (30–60 min, teams of three)** implements the holidays and blackout-dates story with AI-generated tests and an AI code review.

### Part A — Team Calendar from Figma (30 min)

**Story (ADO #103):** Team calendar view. **Acceptance criteria:** matches the Figma frame **Team Calendar**; reuses the shared `StatusChipComponent`; responsive at tablet width (768 px); keyboard accessible (arrow keys move the focused day, Enter opens the request).

**Start state:** `GET /api/leave/team-calendar?month=` is missing; `team-calendar.component.ts` is an empty shell with a route. `_tokens.scss` holds the Figma colour and spacing tokens already exported.

**Done state:** the API returns who is out per day for the month; the Angular page renders a 7-column grid with status chips, passes the Chrome **Lighthouse accessibility** audit at 90+, and matches the frame side by side.

| # | Time | Step | What to type or click |
| --- | --- | --- | --- |
| A1 | 0–3 min | Enable Figma MCP | In `.vscode/mcp.json` set `figma` to enabled; paste your Figma token. Confirm `get_figma_data` and `download_figma_images` appear under Tools. |
| A2 | 3–9 min | Read the frame | Copy the **Team Calendar** frame link from Figma. Agent prompt: `Read this Figma frame <link>. List the layout structure, components used, spacing and colour tokens, and any state variants of the status chip.` Check the token names match `_tokens.scss`. |
| A3 | 9–15 min | API first | Agent prompt: `Add GET /api/leave/team-calendar?month=YYYY-MM returning, for each day of the month, the employees in my team who are Approved out, with leave type. Reuse LeaveRequestService. Add one xUnit test for a month with two overlapping approved leaves.` Run `dotnet test`. |
| A4 | 15–24 min | Generate the component | Agent prompt: `Implement team-calendar.component from the Figma frame you read. Use CSS grid, the tokens in _tokens.scss, and the existing StatusChipComponent for each absence. Make it responsive at 768px (collapse to a week list) and keyboard navigable with roving tabindex and aria-labels. Use signals and add a getTeamCalendar(month) method to MyRequestsService.` Review diffs; reject any hard-coded hex colours. |
| A5 | 24–28 min | Check against design and a11y | Run `ng serve`, open `/team-calendar` beside the Figma frame. Run Lighthouse (Accessibility only). Fix findings with inline chat: `Fix the Lighthouse issue: <paste>`. Resize to 768 px and confirm the collapse. |
| A6 | 28–30 min | Commit | `#103 Team calendar from Figma frame`. |

**Trainer checkpoint (A4):** show one generated component that used raw hex values next to one that used tokens; the difference is almost always whether the prompt named `_tokens.scss`.

### Part B — Team mini-challenge: holidays and blackout dates (30 min)

**Story (ADO #104):** Blackout dates and public holidays. **Acceptance criteria:** weekends and public holidays are excluded from the day count; `GET /api/holidays` returns holidays and blackout dates; the New Request form shows a warning when the range contains a holiday or blackout date; integration tests use `WebApplicationFactory`.

**Teams of three.** Each member owns one lane and all three must merge green before the clock stops.

| Lane | Owner | Deliverable | Copilot feature to use |
| --- | --- | --- | --- |
| .NET | Backend dev | `GET /api/holidays` (the `Holiday` entity and seed rows already exist), `WorkingDayCalculator` that skips weekends and holidays, wired into the Day 1 balance check | Agent mode; `/tests` for the calculator (boundary cases: range starting on a Saturday, holiday on both ends) |
| Angular | Frontend dev | Warning banner on New Request when the selected range contains a holiday or blackout date; day count shown live | Agent mode; `/tests` for a Jasmine spec of the warning |
| QA | Tester | Integration tests with `WebApplicationFactory<Program>` hitting `POST /api/leave-requests` across a holiday; one negative, one positive | `/tests` with the prompt naming `WebApplicationFactory`; `@workspace` to find the seed data |

| # | Time | Step |
| --- | --- | --- |
| B1 | 30–33 min | Team reads #104 together; agree the `WorkingDayCalculator` signature (`int CountWorkingDays(DateOnly start, DateOnly end, IReadOnlyList<DateOnly> holidays)`) so lanes can work in parallel. Each creates a branch `feature/104-<lane>`. |
| B2 | 33–48 min | Build in parallel with Agent mode and `/tests`. QA writes integration tests against the agreed signature and endpoint shape before the .NET lane finishes. |
| B3 | 48–55 min | Open one pull request per lane in Azure DevOps. Request **Copilot code review** on each PR (Repos → PR → Reviewers → Copilot). Read the findings; fix at least one real finding per PR with inline chat. |
| B4 | 55–60 min | Merge in order .NET → QA → Angular. Run `dotnet test` and `npm test` on `main`. First team fully green wins. |

**Scoring (trainer):** 3 points green on `main`, 1 point per Copilot review finding fixed (max 3), 1 point for a test that catches a planted bug (trainer merges a one-line `>=` to `>` change in the calculator and asks whose tests fail).

**Trainer checkpoints**

- At B3, show a Copilot review comment that is wrong and ask the room how they decided it was wrong. The lesson is review the reviewer.
- At B4, ask whose integration tests failed first after the planted bug and why unit tests on the calculator did not.

### Common blockers

| Symptom | Fix |
| --- | --- |
| Figma MCP returns `403` | Token lacks **File content: read**; or the file is in a team the token's user cannot see. |
| Generated component has 40 `<div>`s and no grid | Re-prompt naming the layout: `Use display:grid with 7 columns; one cell component per day.` |
| `WebApplicationFactory` cannot find `Program` | `public partial class Program {}` is already at the end of `Program.cs`; make sure the test project references `Microsoft.AspNetCore.Mvc.Testing` (it does). |
| Copilot code review not offered on the PR | Copilot must be enabled for the Azure DevOps org under Organization settings → GitHub Copilot; trainer does this before Day 3. |

## Day 4 HOL — AI-assisted Azure Pipelines YAML and troubleshooting (30 min)

**Story (ADO #105):** Pipeline for the feature. **Acceptance criteria:** build, test with coverage, dependency and secret scanning, publish an artifact, deploy to **staging**; **production** stage requires a manual approval.

**Start state:** `azure-pipelines.yml` exists with a Build stage only, and it fails: the test project path is planted wrong (`tests/LeaveManager.Tests/` instead of `tests/LeaveManager.Api.Tests/`). Environments `staging` and `production` exist in Azure DevOps; `production` has an approval on the trainer.

**Done state:** the pipeline runs green through Build → Test → Scan → Deploy-Staging, and Deploy-Production is waiting on approval.

### Steps

| # | Time | Step | What to type or click |
| --- | --- | --- | --- |
| 1 | 0–3 min | Trigger the failing run | Push any commit or click **Run pipeline**. Open the failed run; copy the error text from the `DotNetCoreCLI@2` test task log. |
| 2 | 3–8 min | Diagnose with Copilot | In Copilot Chat, with `azure-pipelines.yml` open: `This pipeline run failed with the log below. Find the cause and propose the minimal YAML fix. <paste log>` Expect: project path does not match `tests/LeaveManager.Api.Tests/*.csproj`. Apply the fix with inline chat. Re-run; the Test stage goes green. |
| 3 | 8–16 min | Extend the pipeline | Agent prompt: `Extend azure-pipelines.yml into stages: Build (dotnet build, npm ci && npm run build), Test (dotnet test with Cobertura coverage via coverlet, npm test, publish test results and coverage), Scan (dependency check with dotnet list package --vulnerable, secret scan with gitleaks), Package (publish the API and the Angular dist as one artifact), Deploy-Staging (AzureWebApp@1 to environment staging), Deploy-Production (same, environment production). Use variables for the web app names and the ubuntu-latest image.` Review every task version it picks. |
| 4 | 16–22 min | Run and read | Commit `#105 Multi-stage pipeline`. Watch the run. Fix any task-version or path error by pasting the log back into chat. |
| 5 | 22–26 min | Approval gate | Open the run; Deploy-Production shows **Waiting for approval**. Trainer approves one team's run live to show the gate. |
| 6 | 26–30 min | Harden | Inline chat on the YAML: `Add a condition so Deploy-Production runs only from main, and fail the pipeline if coverage is below 60%.` Commit. |

### Trainer checkpoints

- At step 2, ask who pasted the whole 400-line log versus the failing task's lines; show that the shorter paste gets the fix faster.
- At step 3, show a hallucinated task version (for example `PublishCodeCoverageResults@3`) and how to check it against the Azure DevOps task reference before committing.

### Stretch

- Ask Agent to add a `templates/dotnet-test.yml` step template and refactor the Test stage to use it.
- Ask Copilot to write a `README-pipeline.md` explaining each stage for a new joiner, then check it for errors.

### Common blockers

| Symptom | Fix |
| --- | --- |
| `No hosted parallelism has been purchased or granted` | Trainer requests the free grant for the org at least 3 days before the workshop, or uses a self-hosted agent pool. |
| `AzureWebApp@1` needs a service connection | Use the pre-created service connection `leave-manager-azure`; its name is in the pipeline variables group. |
| gitleaks task not found | Use the script step `docker run --rm -v $(pwd):/repo zricethezav/gitleaks:latest detect --source /repo` instead of a marketplace task. |
| `npm test` hangs on the agent | Karma needs headless Chrome: run `npm test -- --watch=false --browsers=ChromeHeadless`. |

## Appendices

### A. Starter repo layout

```
leave-manager/
  src/LeaveManager.Api/            .NET 8 Web API, EF Core + SQLite (leave.db seeded on start)
    Controllers/LeaveRequestsController.cs, LeaveBalancesController.cs
    Services/LeaveRequestService.cs, LeaveBalanceService.cs, CurrentUserService.cs
    Data/LeaveDbContext.cs, SeedData.cs
    Program.cs                     ProblemDetails mapping; `public partial class Program {}`
  src/leave-manager-ui/            Angular 18, standalone, signals, Jasmine/Karma
    src/app/my-requests/ new-request/ approval-queue/ team-calendar/
    src/app/shared/status-chip/    shared StatusChipComponent
    src/styles/_tokens.scss        Figma colour and spacing tokens
  tests/LeaveManager.Api.Tests/    xUnit, TestDbFactory (in-memory SQLite), one smoke test
  .github/agents/story-implementer.agent.md
  .github/copilot-instructions.md  repo-wide Copilot instructions
  .vscode/mcp.json                 azure-devops and figma servers, disabled
  azure-pipelines.yml              Build stage only, planted wrong test path
  docs/HOL.md                      this guide
```

### B. Seed data

| Entity | Rows |
| --- | --- |
| Employee | `alice@demo` (Engineering, manager `manager@demo`), `bob@demo` (Engineering, manager `manager@demo`), `carol@demo` (Finance, manager `cfo@demo`), `manager@demo`, `cfo@demo` |
| LeaveBalance | Annual 12 days and Sick 6 days for every employee; `bob@demo` Annual set to 2 so the balance rule can fail |
| LeaveRequest | `alice@demo` Approved 2026-12-21 to 2026-12-24; `bob@demo` Pending 2026-12-01 to 2026-12-02 |
| Holiday | 2026-12-25 Christmas Day, 2026-12-26 Boxing Day, 2027-01-01 New Year's Day; blackout 2026-12-31 Year-end close |

Sign-in is a header `X-Demo-User: alice@demo` set by the Angular interceptor; a dropdown in the top bar switches user. No real auth, by design.

### C. Business rules and which day proves them

| Rule | Day | Test that proves it |
| --- | --- | --- |
| No overlapping requests for one employee | 1 | `CreateAsync_Overlap_Returns400` |
| Days requested cannot exceed balance | 1 | `CreateAsync_ExceedsBalance_Returns400` |
| Only the employee's line manager can decide | 2 | `DecideAsync_NonManager_Returns403` |
| A decided request cannot be changed | 2 | `DecideAsync_AlreadyDecided_Returns409` |
| Balance reduced on approval | 2 | `DecideAsync_Approved_ReducesBalance` |
| Weekends and public holidays not counted | 3B | `CountWorkingDays_SkipsWeekendsAndHolidays`, integration `PostAcrossHoliday_CountsOnlyWorkingDays` |
| Production deploy needs manual approval | 4 | Pipeline run shows **Waiting for approval** |

### D. Custom agent file (Day 2)

Replace the tool names with the exact names shown in your Copilot Chat Tools picker.

```markdown
---
name: Story Implementer
description: Implements one Azure DevOps story end to end, with a plan approval gate.
tools: [wit_get_work_item, wit_add_work_item_comment, wit_update_work_item, codebase, editFiles, runCommands, runTests]
---
You implement exactly one Azure DevOps work item in this repository.

1. Call the get-work-item tool for the id the user gives. Read title, description and acceptance criteria.
2. Inspect the codebase and write a numbered plan: files to change, tests to add, anything unclear.
3. After presenting the plan, STOP. Do not edit files until the user replies with the single word APPROVE.
4. On APPROVE: implement, run `dotnet test` and `npm test -- --watch=false`, fix failures, show a summary.
5. Add a work item comment with the summary and the test results. Set the work item state to Resolved.

Rules: never change the data model or Program.cs without asking; keep controllers thin; throw LeaveRuleViolationException for rule violations; follow the Angular signals pattern in my-requests.service.ts; use <lm-status-chip> for statuses and tokens from _tokens.scss, never raw hex.
```

### E. Prompt cheat-sheet

| Need | Prompt pattern |
| --- | --- |
| Find where to change | `@workspace Where is <behaviour> implemented and where would <new rule> belong?` |
| Plan without changing code | Plan mode: `Plan <change>. Name the entities and status values. Do not implement yet.` |
| Implement safely | Agent mode: `Implement the approved plan. Do not change <file>. Reuse <existing service>.` |
| Tests that hit the right layer | `/tests Generate xUnit tests for <method> using <test fixture>. Cover <case 1>, <case 2>, boundary <case 3>.` |
| UI from design | `Implement <component> from the Figma frame you read. Use tokens in _tokens.scss and <shared component>. Responsive at 768px, keyboard navigable.` |
| Fix a build or pipeline | `This failed with the log below. Find the cause and propose the minimal fix. <paste only the failing task's lines>` |

### F. Trainer preparation checklist

- [ ] T-7 days: request the Azure DevOps free parallelism grant for the workshop org
- [ ] T-5 days: create project **Leave-Manager-Workshop**; import the starter repo; create Epic plus stories #101–#105 with the acceptance criteria above; create environments `staging` and `production` with approval on the trainer
- [ ] T-5 days: enable GitHub Copilot for the Azure DevOps org so Copilot code review appears on PRs (Day 3B)
- [ ] T-3 days: publish the Figma file **Leave Manager UI** with the four frames and the status chip component; share view access
- [ ] T-3 days: create two Azure App Services (`leave-manager-staging`, `leave-manager-prod`, F1 tier) and the service connection `leave-manager-azure`
- [ ] T-1 day: send participants the prerequisites list from About this guide and the repo link; confirm each has Copilot Business or Enterprise
- [ ] T-1 day: dry-run Day 2 end to end on a clean machine; MCP token prompts are the most common Day 2 delay
- [ ] Day of: fresh `main` with the planted pipeline failure in place; each participant forks or branches from it
- [ ] Day 3B: prepare the one-line planted bug (`>=` to `>` in `WorkingDayCalculator`) on a branch, ready to merge
