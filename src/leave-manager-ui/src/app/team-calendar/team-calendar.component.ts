import { ChangeDetectionStrategy, Component } from '@angular/core';

/**
 * Team Calendar (Figma frame "Team Calendar").
 * STARTER STATE (HOL 3, Day 3 Part A): empty shell with a route.
 * Build it from the Figma frame through the Figma MCP server:
 *   - 7-column CSS grid, one cell per day, absences as <lm-status-chip>
 *   - tokens from src/styles/_tokens.scss only (no raw hex)
 *   - responsive at 768px (collapse to a week list), keyboard navigable (roving tabindex, aria-labels)
 *   - data from GET /api/leave/team-calendar?month=YYYY-MM (also missing: build the API first)
 */
@Component({
  selector: 'lm-team-calendar',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="card">
      <h1>Team Calendar</h1>
      <p class="muted">Not built yet — this is Day 3, Part A.</p>
    </section>
  `,
  styles: [`h1 { font-size: var(--font-size-xl); margin: 0 0 var(--space-2); }`]
})
export class TeamCalendarComponent {}
