using LeaveManager.Api.Models;

namespace LeaveManager.Api.Data;

public static class SeedData
{
    public static void Apply(LeaveDbContext db)
    {
        if (db.Employees.Any())
        {
            return;
        }

        var manager = new Employee { Email = "manager@demo", Name = "Meera Manager", Team = "Engineering", Role = "Manager" };
        var cfo = new Employee { Email = "cfo@demo", Name = "Chandra Finance", Team = "Finance", Role = "Manager" };
        db.Employees.AddRange(manager, cfo);
        db.SaveChanges();

        var alice = new Employee { Email = "alice@demo", Name = "Alice Anand", Team = "Engineering", ManagerId = manager.Id };
        var bob = new Employee { Email = "bob@demo", Name = "Bob Bhat", Team = "Engineering", ManagerId = manager.Id };
        var carol = new Employee { Email = "carol@demo", Name = "Carol Castelino", Team = "Finance", ManagerId = cfo.Id };
        db.Employees.AddRange(alice, bob, carol);
        db.SaveChanges();

        foreach (var employee in new[] { manager, cfo, alice, bob, carol })
        {
            db.LeaveBalances.Add(new LeaveBalance { EmployeeId = employee.Id, LeaveType = LeaveType.Annual, DaysRemaining = 12 });
            db.LeaveBalances.Add(new LeaveBalance { EmployeeId = employee.Id, LeaveType = LeaveType.Sick, DaysRemaining = 6 });
        }
        db.SaveChanges();

        // Bob has only 2 annual days left so the balance rule (Day 1) can be exercised.
        var bobAnnual = db.LeaveBalances.Single(b => b.EmployeeId == bob.Id && b.LeaveType == LeaveType.Annual);
        bobAnnual.DaysRemaining = 2;

        db.LeaveRequests.AddRange(
            new LeaveRequest
            {
                EmployeeId = alice.Id,
                LeaveType = LeaveType.Annual,
                StartDate = new DateOnly(2026, 12, 21),
                EndDate = new DateOnly(2026, 12, 24),
                Status = LeaveStatus.Approved,
                ManagerComment = "Enjoy the break."
            },
            new LeaveRequest
            {
                EmployeeId = bob.Id,
                LeaveType = LeaveType.Annual,
                StartDate = new DateOnly(2026, 12, 1),
                EndDate = new DateOnly(2026, 12, 2),
                Status = LeaveStatus.Pending
            });

        db.Holidays.AddRange(
            new Holiday { Date = new DateOnly(2026, 12, 25), Name = "Christmas Day" },
            new Holiday { Date = new DateOnly(2026, 12, 26), Name = "Boxing Day" },
            new Holiday { Date = new DateOnly(2027, 1, 1), Name = "New Year's Day" },
            new Holiday { Date = new DateOnly(2026, 12, 31), Name = "Year-end close", IsBlackout = true });

        db.SaveChanges();
    }
}
