var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var assignments = new[]
{
    new Assignment(1, "Bai tap 1", "Tao API dau tien"),
    new Assignment(2, "Bai tap 2", "Xay dung client goi API")
};

var submissions = new List<SubmissionResult>();

app.MapGet("/", () => Results.Ok(new
{
    service = "Submission.Api",
    status = "running"
}));

app.MapGet("/api/assignments", () => Results.Ok(assignments));

app.MapPost("/api/submissions", (CreateSubmissionRequest request) =>
{
    if (request.AssignmentId <= 0)
    {
        return Results.BadRequest(new { message = "AssignmentId must be greater than 0." });
    }

    if (!assignments.Any(a => a.Id == request.AssignmentId))
    {
        return Results.BadRequest(new { message = "Assignment does not exist." });
    }

    if (string.IsNullOrWhiteSpace(request.StudentCode))
    {
        return Results.BadRequest(new { message = "StudentCode must not be empty." });
    }

    if (!Uri.TryCreate(request.RepositoryUrl, UriKind.Absolute, out var repositoryUri)
        || repositoryUri.Scheme is not ("http" or "https"))
    {
        return Results.BadRequest(new { message = "RepositoryUrl must be a valid HTTP or HTTPS URL." });
    }

    var submission = new SubmissionResult(
        Guid.NewGuid(),
        request.AssignmentId,
        request.StudentCode.Trim(),
        "received",
        DateTimeOffset.UtcNow);

    lock (submissions)
    {
        submissions.Add(submission);
    }

    return Results.Created($"/api/submissions/{submission.Id}", submission);
});

app.MapGet("/api/submissions/{id:guid}", (Guid id) =>
{
    SubmissionResult? submission;
    lock (submissions)
    {
        submission = submissions.FirstOrDefault(s => s.Id == id);
    }

    return submission is null
        ? Results.NotFound(new { message = "Submission not found." })
        : Results.Ok(submission);
});

app.Run();

record Assignment(int Id, string Title, string Description);

record CreateSubmissionRequest(int AssignmentId, string StudentCode, string RepositoryUrl);

record SubmissionResult(
    Guid Id,
    int AssignmentId,
    string StudentCode,
    string Status,
    DateTimeOffset ReceivedAt);
