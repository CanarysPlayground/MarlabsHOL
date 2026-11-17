using LeaveManager.Api.Data;
using LeaveManager.Api.Dtos;
using LeaveManager.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManager.Api.Controllers;

[ApiController]
[Produces("application/json")]
public class LeaveBalancesController(ILeaveBalanceService balances, ICurrentUserService currentUser, LeaveDbContext db) : ControllerBase
{
    /// <summary>Remaining balance per leave type for the signed-in user.</summary>
    [HttpGet("api/leave-balances/me")]
    public async Task<ActionResult<IReadOnlyList<LeaveBalanceDto>>> GetMine(CancellationToken ct)
    {
        var me = await currentUser.GetCurrentEmployeeAsync(ct);
        return Ok(await balances.GetForEmployeeAsync(me.Id, ct));
    }

    /// <summary>Who am I (drives the user switcher in the Angular top bar).</summary>
    [HttpGet("api/employees/me")]
    public async Task<ActionResult<EmployeeDto>> Me(CancellationToken ct)
    {
        var me = await currentUser.GetCurrentEmployeeAsync(ct);
        return Ok(new EmployeeDto(me.Id, me.Email, me.Name, me.Team, me.Role, me.ManagerId));
    }

    /// <summary>All demo users (for the user switcher).</summary>
    [HttpGet("api/employees")]
    public async Task<ActionResult<IReadOnlyList<EmployeeDto>>> All(CancellationToken ct)
    {
        var list = await db.Employees.AsNoTracking()
            .OrderBy(e => e.Name)
            .Select(e => new EmployeeDto(e.Id, e.Email, e.Name, e.Team, e.Role, e.ManagerId))
            .ToListAsync(ct);
        return Ok(list);
    }

    // HOL 3 Part B (Day 3): GET /api/holidays -> public holidays and blackout dates. Missing on purpose.
}
