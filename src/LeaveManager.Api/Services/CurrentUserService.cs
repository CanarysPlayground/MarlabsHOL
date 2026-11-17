using LeaveManager.Api.Data;
using LeaveManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManager.Api.Services;

/// <summary>
/// Demo "authentication": the signed-in user is whoever the X-Demo-User header names.
/// Defaults to alice@demo when the header is absent. No real auth, by design.
/// </summary>
public interface ICurrentUserService
{
    Task<Employee> GetCurrentEmployeeAsync(CancellationToken ct = default);
}

public class CurrentUserService(IHttpContextAccessor accessor, LeaveDbContext db) : ICurrentUserService
{
    public const string HeaderName = "X-Demo-User";
    public const string DefaultUser = "alice@demo";

    public async Task<Employee> GetCurrentEmployeeAsync(CancellationToken ct = default)
    {
        var email = accessor.HttpContext?.Request.Headers[HeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(email))
        {
            email = DefaultUser;
        }

        var employee = await db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Email == email, ct);
        return employee ?? throw new UnknownUserException(email);
    }
}

public class UnknownUserException(string email) : Exception($"Unknown demo user '{email}'.")
{
    public string Email { get; } = email;
}
