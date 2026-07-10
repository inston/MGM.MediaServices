using MGM.MediaServices.Core.Models;
using MGM.MediaServices.Core.Services;
using MGM.MediaServices.Worker;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<JobQueue>();
builder.Services.AddSingleton<IJobExecutor, ExternalToolJobExecutor>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok("MGM.MediaServices is healthy"));

app.MapPost("/api/jobs", async (Job job, JobQueue queue, ILogger<Program> logger) =>
{
    logger.LogInformation("Received async job {JobId} for tool {Tool}", job.Id, job.Tool);

    job.Status = JobStatus.Queued;
    await queue.EnqueueAsync(job);

    // Callback can be called from worker when job completes (future enhancement)

    return Results.Accepted($"/api/jobs/{job.Id}", job);
});

app.MapPost("/api/jobs/sync", async (Job job, IJobExecutor executor, ILogger<Program> logger) =>
{
    logger.LogInformation("Received sync job {JobId} for tool {Tool}", job.Id, job.Tool);

    job.Status = JobStatus.Running;
    var progress = new ProgressReporter(job, _ => { });
    var result = await executor.ExecuteAsync(job, progress);
    return Results.Ok(new { job, result });
});

app.Run();
