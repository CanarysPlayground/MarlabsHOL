using LeaveManager.Api.Data;
using LeaveManager.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LeaveManager.Api.Services;

public interface ILeaveBalanceService
{
    Task<IReadOnlyList<LeaveBalanceDto>> GetForEmployeeAsync(int employeeId, CancellationToken ct = default);
}

public class LeaveBalanceService(LeaveDbContext db) : ILeaveBalanceService
{
    public async Task<IReadOnlyList<LeaveBalanceDto>> GetForEmployeeAsync(int employeeId, CancellationToken ct = default)
    {
        return await db.LeaveBalances
            .AsNoTracking()
            .Where(b => b.EmployeeId == employeeId)
            .OrderBy(b => b.LeaveType)
            .Select(b => new LeaveBalanceDto(b.LeaveType, b.DaysRemaining))
            .ToListAsync(ct);
    }
}
