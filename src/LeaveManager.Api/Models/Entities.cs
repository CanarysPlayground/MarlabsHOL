namespace LeaveManager.Api.Models;

public enum LeaveType
{
    Annual,
    Sick
}

public enum LeaveStatus
{
    Pending,
    Approved,
    Rejected
}

public class Employee
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public string Role { get; set; } = "Employee"; // Employee | Manager
}

public class LeaveBalance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; }
    public decimal DaysRemaining { get; set; }
}

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? ManagerComment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Holiday
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsBlackout { get; set; }
}
