export type LeaveType = 'Annual' | 'Sick';
export type LeaveStatus = 'Pending' | 'Approved' | 'Rejected';

export interface LeaveRequest {
  id: number;
  employeeId: number;
  employeeName: string;
  leaveType: LeaveType;
  startDate: string; // YYYY-MM-DD
  endDate: string;   // YYYY-MM-DD
  status: LeaveStatus;
  managerComment: string | null;
  createdAt: string;
}

export interface CreateLeaveRequest {
  leaveType: LeaveType;
  startDate: string;
  endDate: string;
}

export interface LeaveBalance {
  leaveType: LeaveType;
  daysRemaining: number;
}

export interface Employee {
  id: number;
  email: string;
  name: string;
  team: string;
  role: 'Employee' | 'Manager';
  managerId: number | null;
}

/** RFC 7807 body returned by the API on business-rule violations (HOL 1). */
export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
}
