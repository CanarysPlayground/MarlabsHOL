import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { LeaveStatus } from '../../core/models';

/**
 * Shared status chip from the Figma component "Status chip".
 * Colours come from the design tokens in src/styles/_tokens.scss. Reuse this everywhere
 * a status is shown (My Requests, Approval Queue, Team Calendar).
 */
@Component({
  selector: 'lm-status-chip',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<span class="chip" [class]="'chip chip--' + variant()" role="status">{{ label() }}</span>`,
  styles: [`
    .chip {
      display: inline-flex;
      align-items: center;
      gap: var(--space-1);
      padding: 2px var(--space-2);
      border-radius: var(--radius-pill);
      font-size: var(--font-size-sm);
      font-weight: 600;
      line-height: 1.4;
      border: 1px solid transparent;
      white-space: nowrap;
    }
    .chip::before {
      content: '';
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: currentColor;
    }
    .chip--pending  { color: var(--status-pending-fg);  background: var(--status-pending-bg);  border-color: var(--status-pending-border); }
    .chip--approved { color: var(--status-approved-fg); background: var(--status-approved-bg); border-color: var(--status-approved-border); }
    .chip--rejected { color: var(--status-rejected-fg); background: var(--status-rejected-bg); border-color: var(--status-rejected-border); }
    .chip--neutral  { color: var(--color-text-muted);   background: var(--color-surface-alt);  border-color: var(--color-border); }
  `]
})
export class StatusChipComponent {
  /** Leave status, or any free text (e.g. a leave type) for a neutral chip. */
  readonly status = input.required<LeaveStatus | string>();

  readonly variant = computed(() => {
    const s = String(this.status()).toLowerCase();
    return s === 'pending' || s === 'approved' || s === 'rejected' ? s : 'neutral';
  });

  readonly label = computed(() => String(this.status()));
}
