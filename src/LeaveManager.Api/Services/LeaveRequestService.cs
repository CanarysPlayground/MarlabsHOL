using LeaveManager.Api.Data;
using LeaveManager.Api.Dtos;
using LeaveManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManager.Api.Services;

public interface ILeaveRequestService
{
    Task<IReadOnlyList<LeaveRequestDto>> GetForEmployeeAsync(int employeeId, CancellationToken ct = default);
    Task<IReadOnlyList<LeaveRequestDto>> GetPendingForManagerAsync(int managerId, CancellationToken ct = default);
    Task<LeaveRequestDto> CreateAsync(int employeeId, CreateLeaveRequestDto dto, CancellationToken ct = default);

    // HOL 2 (Day 2): PATCH /api/leave-requests/{id}/decision is implemented here.
    // HOL 3 (Day 3, Part A): GET /api/leave/team-calendar?month= is implemented here.
}

public class LeaveRequestService(LeaveDbContext db, ILogger<LeaveRequestService> logger) : ILeaveRequestService
{
    public async Task<IReadOnlyList<LeaveRequestDto>> GetForEmployeeAsync(int employeeId, CancellationToken ct = default)
    {
        return await db.LeaveRequests
            .AsNoTracking()
            .Where(r => r.EmployeeId == employeeId)
            .OrderByDescending(r => r.StartDate)
            .Select(r => ToDto(r))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<LeaveRequestDto>> GetPendingForManagerAsync(int managerId, CancellationToken ct = default)
    {
        return await db.LeaveRequests
            .AsNoTracking()
            .Where(r => r.Status == LeaveStatus.Pending && r.Employee.ManagerId == managerId)
            .OrderBy(r => r.StartDate)
            .Select(r => ToDto(r))
            .ToListAsync(ct);
    }

    /// <summary>
    /// Creates a leave request.
    /// STARTER STATE: no validation. HOL 1 (Day 1) adds the overlap and balance rules here
    /// and returns 400 ProblemDetails through <see cref="LeaveRuleViolationException"/>.
    /// </summary>
    public async Task<LeaveRequestDto> CreateAsync(int employeeId, CreateLeaveRequestDto dto, CancellationToken ct = default)
    {
        var request = new LeaveRequest
        {
            EmployeeId = employeeId,
            LeaveType = dto.LeaveType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = LeaveStatus.Pending
        };

        db.LeaveRequests.Add(request);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Leave request {RequestId} created for employee {EmployeeId} ({Start} to {End})",
            request.Id, employeeId, dto.StartDate, dto.EndDate);

        await db.Entry(request).Reference(r => r.Employee).LoadAsync(ct);
        return ToDto(request);
    }

    private static LeaveRequestDto ToDto(LeaveRequest r) => new(
        r.Id,
        r.EmployeeId,
        r.Employee.Name,
        r.LeaveType,
        r.StartDate,
        r.EndDate,
        r.Status,
        r.ManagerComment,
        r.CreatedAt);
}

/// <summary>
/// Throw this from a service when a business rule is violated.
/// The ProblemDetails middleware in Program.cs maps it to HTTP 400 with the message as "detail".
/// </summary>
public class LeaveRuleViolationException(string message) : Exception(message);
