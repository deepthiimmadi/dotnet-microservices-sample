using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory store for demo only. Replace with a real database in production.
var cases = new ConcurrentDictionary<Guid, CaseRecord>();
var seed = new CaseRecord(Guid.NewGuid(), "Sample case — synthetic data only", "deepthi", "Draft");
cases[seed.Id] = seed;

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/cases", () => Results.Ok(cases.Values.OrderBy(c => c.Title)));

app.MapPost("/cases", (CreateCaseRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { error = "Title is required." });
    }

    var record = new CaseRecord(Guid.NewGuid(), request.Title.Trim(), request.Owner ?? "unassigned", "Draft");
    cases[record.Id] = record;
    return Results.Created($"/cases/{record.Id}", record);
});

app.MapPost("/cases/{id:guid}/submit", (Guid id) =>
{
    if (!cases.TryGetValue(id, out var record) || record.Status != "Draft")
    {
        return Results.BadRequest(new { error = "Only a Draft case can be submitted." });
    }

    var updated = record with { Status = "Submitted" };
    cases[id] = updated;
    return Results.Ok(updated);
});

app.MapPost("/cases/{id:guid}/approve", (Guid id) =>
{
    if (!cases.TryGetValue(id, out var record) || record.Status != "Submitted")
    {
        return Results.BadRequest(new { error = "Only a Submitted case can be approved." });
    }

    var updated = record with { Status = "Approved" };
    cases[id] = updated;
    return Results.Ok(updated);
});

app.Run();

public record CaseRecord(Guid Id, string Title, string Owner, string Status);
public record CreateCaseRequest(string Title, string? Owner);
