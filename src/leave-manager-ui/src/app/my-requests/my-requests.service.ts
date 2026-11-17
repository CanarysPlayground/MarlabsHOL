import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API } from '../core/api';
import { CreateLeaveRequest, Employee, LeaveBalance, LeaveRequest } from '../core/models';

/**
 * Reference HttpClient service pattern for the workshop: thin methods, typed responses,
 * no state. Components hold state in signals.
 */
@Injectable({ providedIn: 'root' })
export class MyRequestsService {
  private readonly http = inject(HttpClient);

  getMine(): Observable<LeaveRequest[]> {
    return this.http.get<LeaveRequest[]>(`${API}/api/leave-requests`);
  }

  getPendingForMe(): Observable<LeaveRequest[]> {
    return this.http.get<LeaveRequest[]>(`${API}/api/leave-requests/pending`);
  }

  create(body: CreateLeaveRequest): Observable<LeaveRequest> {
    return this.http.post<LeaveRequest>(`${API}/api/leave-requests`, body);
  }

  getMyBalances(): Observable<LeaveBalance[]> {
    return this.http.get<LeaveBalance[]>(`${API}/api/leave-balances/me`);
  }

  getMe(): Observable<Employee> {
    return this.http.get<Employee>(`${API}/api/employees/me`);
  }

  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${API}/api/employees`);
  }

  // HOL 2 (Day 2): decide(id, decision, comment) -> PATCH /api/leave-requests/{id}/decision
  // HOL 3 (Day 3): getTeamCalendar(month) -> GET /api/leave/team-calendar?month=YYYY-MM
  // HOL 3B (Day 3): getHolidays() -> GET /api/holidays
}
