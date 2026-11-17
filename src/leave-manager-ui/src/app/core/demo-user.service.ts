import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'leave-manager.demo-user';

/**
 * Demo sign-in: the current user is a plain email sent as the X-Demo-User header.
 * Switch users from the top bar. No real authentication, by design.
 */
@Injectable({ providedIn: 'root' })
export class DemoUserService {
  readonly email = signal<string>(this.load());

  switchTo(email: string): void {
    this.email.set(email);
    try {
      localStorage.setItem(STORAGE_KEY, email);
    } catch {
      /* storage unavailable: keep in memory */
    }
  }

  private load(): string {
    try {
      return localStorage.getItem(STORAGE_KEY) ?? 'alice@demo';
    } catch {
      return 'alice@demo';
    }
  }
}
