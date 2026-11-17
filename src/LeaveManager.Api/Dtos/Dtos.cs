using LeaveManager.Api.Models;

namespace LeaveManager.Api.Dtos;

public record LeaveRequestDto(
    int Id,
    int EmployeeId,
    string EmployeeName,
    LeaveType LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    LeaveStatus Status,
    string? ManagerComment,
    DateTime CreatedAt);

public record CreateLeaveRequestDto(
    LeaveType LeaveType,
    DateOnly StartDate,
    DateOnly EndDate);

public record LeaveBalanceDto(LeaveType LeaveType, decimal DaysRemaining);

public record EmployeeDto(int Id, string Email, string Name, string Team, string Role, int? ManagerId);

// Used by HOL 2 (Day 2). Not referenced by any endpoint in the starter repo.
public record DecisionDto(LeaveStatus Decision, string? Comment);
