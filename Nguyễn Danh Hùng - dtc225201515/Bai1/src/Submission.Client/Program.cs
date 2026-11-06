using System.Net.Http.Json;

var apiAddress = args.Length > 0 ? args[0] : "http://localhost:5000";

using var client = new HttpClient
{
    BaseAddress = new Uri(apiAddress)
};

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine($"API: {client.BaseAddress}");

var assignments = await client.GetFromJsonAsync<List<Assignment>>("/api/assignments")
                  ?? throw new InvalidOperationException("API did not return an assignment list.");

Console.WriteLine("Danh sach bai tap:");
foreach (var assignment in assignments)
{
    Console.WriteLine($"  - {assignment.Id}: {assignment.Title} - {assignment.Description}");
}

var request = new CreateSubmissionRequest(
    AssignmentId: 1,
    StudentCode: "dtc225201515",
    RepositoryUrl: "https://github.com/dtc225201515-cloud/CNTTK23D");

var postResponse = await client.PostAsJsonAsync("/api/submissions", request);
Console.WriteLine($"POST /api/submissions -> {(int)postResponse.StatusCode} {postResponse.StatusCode}");
postResponse.EnsureSuccessStatusCode();

var createdSubmission = await postResponse.Content.ReadFromJsonAsync<SubmissionResult>()
                        ?? throw new InvalidOperationException("API did not return the created submission.");

Console.WriteLine($"Da nop bai. Ma luot nop: {createdSubmission.Id}");
Console.WriteLine($"Trang thai: {createdSubmission.Status}");

var savedSubmission = await client.GetFromJsonAsync<SubmissionResult>(
    $"/api/submissions/{createdSubmission.Id}")
    ?? throw new InvalidOperationException("Could not read the saved submission.");

Console.WriteLine($"Doc lai thanh cong: {savedSubmission.Id} - {savedSubmission.Status}");

record Assignment(int Id, string Title, string Description);

record CreateSubmissionRequest(int AssignmentId, string StudentCode, string RepositoryUrl);

record SubmissionResult(
    Guid Id,
    int AssignmentId,
    string StudentCode,
    string Status,
    DateTimeOffset ReceivedAt);
