import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CreateLeaveRequest, LeaveType } from '../core/models';
import { MyRequestsService } from '../my-requests/my-requests.service';

/**
 * New Request form (Figma frame "New Request").
 * STARTER STATE (HOL 1): posts the form and ignores non-2xx responses.
 *   - Show problem.detail from a 400 ProblemDetails response in the error banner.
 *   - Show the remaining balance (GET /api/leave-balances/me) beside the date pickers.
 * HOL 3B: warn when the selected range contains a holiday or blackout date.
 */
@Component({
  selector: 'lm-new-request',
  standalone: true,
  imports: [FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="card form">
      <h1>New Request</h1>

      @if (error()) {
        <div class="banner banner--error" role="alert">{{ error() }}</div>
      }

      <form (ngSubmit)="submit()" #f="ngForm">
        <div class="grid">
          <div class="field">
            <label for="leaveType">Leave type</label>
            <select id="leaveType" name="leaveType" [(ngModel)]="model.leaveType" required>
              <option value="Annual">Annual</option>
              <option value="Sick">Sick</option>
            </select>
          </div>
          <div class="field">
            <label for="startDate">From</label>
            <input id="startDate" name="startDate" type="date" [(ngModel)]="model.startDate" required />
          </div>
          <div class="field">
            <label for="endDate">To</label>
            <input id="endDate" name="endDate" type="date" [(ngModel)]="model.endDate" required />
          </div>
        </div>

        <footer class="actions">
          <button type="button" class="btn" (click)="cancel()">Cancel</button>
          <button type="submit" class="btn btn--primary" [disabled]="f.invalid || submitting()">
            {{ submitting() ? 'Submitting…' : 'Submit request' }}
          </button>
        </footer>
      </form>
    </section>
  `,
  styles: [`
    .form { max-width: 640px; }
    h1 { font-size: var(--font-size-xl); margin: 0 0 var(--space-4); }
    .grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: var(--space-4); }
    .actions { display: flex; justify-content: flex-end; gap: var(--space-2); margin-top: var(--space-6); }
    @media (max-width: 768px) { .grid { grid-template-columns: 1fr; } }
  `]
})
export class NewRequestComponent {
  private readonly api = inject(MyRequestsService);
  private readonly router = inject(Router);

  readonly model: CreateLeaveRequest = { leaveType: 'Annual' as LeaveType, startDate: '', endDate: '' };
  readonly error = signal<string | null>(null);
  readonly submitting = signal(false);

  submit(): void {
    this.error.set(null);
    this.submitting.set(true);
    this.api.create(this.model).subscribe({
      next: () => this.router.navigateByUrl('/my-requests'),
      error: () => {
        // STARTER STATE: the API error is swallowed. HOL 1 replaces this.
        this.submitting.set(false);
      }
    });
  }

  cancel(): void {
    this.router.navigateByUrl('/my-requests');
  }
}
