import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { DemoUserService } from './core/demo-user.service';
import { Employee } from './core/models';
import { MyRequestsService } from './my-requests/my-requests.service';

@Component({
  selector: 'lm-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  readonly user = inject(DemoUserService);
  private readonly api = inject(MyRequestsService);

  readonly employees = signal<Employee[]>([]);
  readonly apiDown = signal(false);

  ngOnInit(): void {
    this.api.getEmployees().subscribe({
      next: (list) => this.employees.set(list),
      error: () => this.apiDown.set(true)
    });
  }

  onUserChange(event: Event): void {
    const email = (event.target as HTMLSelectElement).value;
    this.user.switchTo(email);
    // Simplest way to refresh every screen for the new user.
    window.location.reload();
  }
}
