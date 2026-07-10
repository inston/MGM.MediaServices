using System.Text.Json.Serialization;

namespace MGM.MediaServices.Core.Models;

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;

    public string Tool { get; set; } = "ffmpeg";
    public string CommandTemplate { get; set; } = string.Empty;

    public JobStatus Status { get; set; } = JobStatus.Queued;

    public int ProgressPercent { get; set; } = 0;
    public string? ProgressMessage { get; set; }

    public int CurrentStep { get; set; } = 1;
    public int TotalSteps { get; set; } = 1;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset LastUpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string? ErrorMessage { get; set; }

    public string? CallbackUrl { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = new();
}

public enum JobStatus
{
    Queued,
    Running,
    Paused,
    Succeeded,
    Failed,
    Cancelled,
    Cancelling
}
