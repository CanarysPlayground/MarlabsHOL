import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LeaveRequest } from '../core/models';
import { StatusChipComponent } from '../shared/status-chip/status-chip.component';
import { MyRequestsService } from './my-requests.service';

@Component({
  selector: 'lm-my-requests',
  standalone: true,
  imports: [DatePipe, RouterLink, StatusChipComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="card">
      <header class="header">
        <h1>My Requests</h1>
        <a class="btn btn--primary" routerLink="/new-request">New request</a>
      </header>

      @if (loading()) {
        <p class="muted">Loading…</p>
      } @else if (requests().length === 0) {
        <p class="muted">You have no leave requests yet.</p>
      } @else {
        <table class="table">
          <thead>
            <tr><th>Type</th><th>From</th><th>To</th><th>Status</th><th>Manager comment</th></tr>
          </thead>
          <tbody>
            @for (r of requests(); track r.id) {
              <tr>
                <td>{{ r.leaveType }}</td>
                <td>{{ r.startDate | date: 'dd MMM yyyy' }}</td>
                <td>{{ r.endDate | date: 'dd MMM yyyy' }}</td>
                <td><lm-status-chip [status]="r.status" /></td>
                <td class="muted">{{ r.managerComment ?? '—' }}</td>
              </tr>
            }
          </tbody>
        </table>
      }
    </section>
  `,
  styles: [`
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: var(--space-4); }
    h1 { font-size: var(--font-size-xl); margin: 0; }
  `]
})
export class MyRequestsComponent implements OnInit {
  private readonly api = inject(MyRequestsService);

  readonly requests = signal<LeaveRequest[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.api.getMine().subscribe({
      next: (rows) => { this.requests.set(rows); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }
}
