using LeaveManager.Api.Dtos;
using LeaveManager.Api.Models;
using LeaveManager.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LeaveManager.Api.Tests;

/// <summary>
/// HOL 1 (Day 1): add tests here with Copilot /tests for
///   - CreateAsync rejecting a request that overlaps existing Pending/Approved leave
///   - CreateAsync rejecting a request whose day count exceeds the balance
/// HOL 2 (Day 2): DecideAsync tests (non-manager 403, already decided 409, approved reduces balance).
/// </summary>
public class LeaveRequestServiceTests
{
    [Fact]
    public async Task Smoke_SeededData_IsPresent()
    {
        using var factory = new TestDbFactory();
        await using var db = factory.CreateContext();
        var service = new LeaveRequestService(db, NullLogger<LeaveRequestService>.Instance);

        var alice = db.Employees.Single(e => e.Email == "alice@demo");
        var requests = await service.GetForEmployeeAsync(alice.Id);

        Assert.Single(requests);
        Assert.Equal(LeaveStatus.Approved, requests[0].Status);
    }

    // Example of the shape Copilot should produce (remove or replace during HOL 1):
    //
    // [Fact]
    // public async Task CreateAsync_Overlap_Returns400()
    // {
    //     using var factory = new TestDbFactory();
    //     await using var db = factory.CreateContext();
    //     var service = new LeaveRequestService(db, NullLogger<LeaveRequestService>.Instance);
    //     var alice = db.Employees.Single(e => e.Email == "alice@demo");
    //
    //     var dto = new CreateLeaveRequestDto(LeaveType.Annual, new DateOnly(2026, 12, 23), new DateOnly(2026, 12, 28));
    //
    //     await Assert.ThrowsAsync<LeaveRuleViolationException>(() => service.CreateAsync(alice.Id, dto));
    // }
}
