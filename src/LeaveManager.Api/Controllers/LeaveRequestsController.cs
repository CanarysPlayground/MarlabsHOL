using LeaveManager.Api.Dtos;
using LeaveManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManager.Api.Controllers;

[ApiController]
[Route("api/leave-requests")]
[Produces("application/json")]
public class LeaveRequestsController(ILeaveRequestService requests, ICurrentUserService currentUser) : ControllerBase
{
    /// <summary>List leave requests for the signed-in user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestDto>>> GetMine(CancellationToken ct)
    {
        var me = await currentUser.GetCurrentEmployeeAsync(ct);
        return Ok(await requests.GetForEmployeeAsync(me.Id, ct));
    }

    /// <summary>Pending requests from the signed-in manager's direct reports (Approval Queue).</summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(IReadOnlyList<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestDto>>> GetPendingForMe(CancellationToken ct)
    {
        var me = await currentUser.GetCurrentEmployeeAsync(ct);
        return Ok(await requests.GetPendingForManagerAsync(me.Id, ct));
    }

    /// <summary>Create a leave request. STARTER STATE: no validation (HOL 1).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LeaveRequestDto>> Create([FromBody] CreateLeaveRequestDto dto, CancellationToken ct)
    {
        var me = await currentUser.GetCurrentEmployeeAsync(ct);
        var created = await requests.CreateAsync(me.Id, dto, ct);
        return CreatedAtAction(nameof(GetMine), new { id = created.Id }, created);
    }

    // HOL 2 (Day 2): PATCH {id}/decision  -> manager approves or rejects with a comment.
    // HOL 3 (Day 3): GET  /api/leave/team-calendar?month=YYYY-MM  (new controller or here).
}
