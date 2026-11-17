import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { LeaveRequest } from '../core/models';
import { MyRequestsService } from '../my-requests/my-requests.service';
import { StatusChipComponent } from '../shared/status-chip/status-chip.component';

/**
 * Approval Queue (Figma frame "Approval Queue").
 * STARTER STATE (HOL 2): read-only list of pending requests from my direct reports.
 * Day 2 adds Approve / Reject actions with a comment calling PATCH /api/leave-requests/{id}/decision.
 */
@Component({
  selector: 'lm-approval-queue',
  standalone: true,
  imports: [DatePipe, StatusChipComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="card">
      <h1>Approval Queue</h1>
      <p class="muted">Pending requests from your direct reports. Switch to <strong>manager&#64;demo</strong> in the top bar to see Engineering's queue.</p>

      @if (loading()) {
        <p class="muted">Loading…</p>
      } @else if (requests().length === 0) {
        <p class="muted">Nothing waiting for your decision.</p>
      } @else {
        <table class="table">
          <thead>
            <tr><th>Employee</th><th>Type</th><th>From</th><th>To</th><th>Status</th><th>Actions</th></tr>
          </thead>
          <tbody>
            @for (r of requests(); track r.id) {
              <tr>
                <td>{{ r.employeeName }}</td>
                <td>{{ r.leaveType }}</td>
                <td>{{ r.startDate | date: 'dd MMM yyyy' }}</td>
                <td>{{ r.endDate | date: 'dd MMM yyyy' }}</td>
                <td><lm-status-chip [status]="r.status" /></td>
                <td class="muted"><!-- HOL 2: Approve / Reject buttons --> —</td>
              </tr>
            }
          </tbody>
        </table>
      }
    </section>
  `,
  styles: [`h1 { font-size: var(--font-size-xl); margin: 0 0 var(--space-2); }`]
})
export class ApprovalQueueComponent implements OnInit {
  private readonly api = inject(MyRequestsService);

  readonly requests = signal<LeaveRequest[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.api.getPendingForMe().subscribe({
      next: (rows) => { this.requests.set(rows); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }
}
