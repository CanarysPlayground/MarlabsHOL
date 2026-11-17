using System.Text.Json.Serialization;
using LeaveManager.Api.Data;
using LeaveManager.Api.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=leave.db";
builder.Services.AddDbContext<LeaveDbContext>(o => o.UseSqlite(connectionString));

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

// Map business-rule and demo-auth exceptions to RFC 7807 ProblemDetails.
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var feature = context.Features.Get<IExceptionHandlerFeature>();
    var ex = feature?.Error;

    var (status, title) = ex switch
    {
        LeaveRuleViolationException => (StatusCodes.Status400BadRequest, "Leave request rejected"),
        UnknownUserException => (StatusCodes.Status401Unauthorized, "Unknown demo user"),
        _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
    };

    context.Response.StatusCode = status;
    var problem = new ProblemDetails
    {
        Status = status,
        Title = title,
        Detail = status == StatusCodes.Status500InternalServerError ? null : ex?.Message,
        Instance = context.Request.Path
    };
    await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
}));

app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

// Create and seed the SQLite database on start (no migrations: workshop convenience).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LeaveDbContext>();
    db.Database.EnsureCreated();
    SeedData.Apply(db);
}

app.Run();

// Lets WebApplicationFactory<Program> in the test project reach this entry point (HOL 3 Part B).
public partial class Program { }
