using MGM.MediaServices.Core.Configuration;
using MGM.MediaServices.Core.Data;
using MGM.MediaServices.Core.Models;
using MGM.MediaServices.Core.Services;
using MGM.MediaServices.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<MediaServicesOptions>(builder.Configuration.GetSection("MediaServices"));

// Database - SQLite for cross-platform (Linux/Windows)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var optionsValue = builder.Configuration.GetSection("MediaServices").Get<MediaServicesOptions>() 
        ?? new MediaServicesOptions();
    var dbPath = Path.Combine(AppContext.BaseDirectory, optionsValue.DatabasePath);
    Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
    options.UseSqlite($"Data Source={dbPath}");
});

// Services
builder.Services.AddSingleton<JobQueue>();
builder.Services.AddSingleton<IJobExecutor, ExternalToolJobExecutor>();
builder.Services.AddScoped<IJobStore, EfJobStore>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health check
app.MapGet("/health", () => Results.Ok("MGM.MediaServices is healthy"));

// Async job endpoint with persistence
app.MapPost("/api/jobs", async (Job job, JobQueue queue, IJobStore store, ILogger<Program> logger) =>
{
    logger.LogInformation("Received async job {JobId} for tool {Tool}", job.Id, job.Tool);

    job.Status = JobStatus.Queued;
    await store.SaveJobAsync(job);
    await queue.EnqueueAsync(job);

    return Results.Accepted($"/api/jobs/{job.Id}", job);
});

// Sync endpoint (for testing)
app.MapPost("/api/jobs/sync", async (Job job, IJobExecutor executor, ILogger<Program> logger) =>
{
    logger.LogInformation("Received sync job {JobId} for tool {Tool}", job.Id, job.Tool);
    job.Status = JobStatus.Running;
    var progress = new ProgressReporter(job, _ => { });
    var result = await executor.ExecuteAsync(job, progress);
    return Results.Ok(new { job, result });
});

app.Run();