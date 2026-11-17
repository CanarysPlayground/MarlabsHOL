import { Routes } from '@angular/router';
import { ApprovalQueueComponent } from './approval-queue/approval-queue.component';
import { MyRequestsComponent } from './my-requests/my-requests.component';
import { NewRequestComponent } from './new-request/new-request.component';
import { TeamCalendarComponent } from './team-calendar/team-calendar.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'my-requests' },
  { path: 'my-requests', component: MyRequestsComponent, title: 'My Requests' },
  { path: 'new-request', component: NewRequestComponent, title: 'New Request' },
  { path: 'approval-queue', component: ApprovalQueueComponent, title: 'Approval Queue' },
  { path: 'team-calendar', component: TeamCalendarComponent, title: 'Team Calendar' },
  { path: '**', redirectTo: 'my-requests' }
];
