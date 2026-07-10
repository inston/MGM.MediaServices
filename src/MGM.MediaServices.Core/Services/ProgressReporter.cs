namespace MGM.MediaServices.Core.Services;

public class ProgressReporter : IProgressReporter
{
    private readonly Job _job;
    private readonly Action<Job> _updateJob;

    public ProgressReporter(Job job, Action<Job> updateJob)
    {
        _job = job;
        _updateJob = updateJob;
    }

    public Task ReportProgressAsync(int percent, string message, CancellationToken cancellationToken = default)
    {
        _job.ProgressPercent = percent;
        _job.ProgressMessage = message;
        _job.LastUpdatedAt = DateTimeOffset.UtcNow;

        _updateJob(_job);

        return Task.CompletedTask;
    }
}
