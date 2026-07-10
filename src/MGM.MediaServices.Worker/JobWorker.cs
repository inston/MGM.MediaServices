using MGM.MediaServices.Core.Models;
using MGM.MediaServices.Core.Services;

namespace MGM.MediaServices.Worker;

public class JobWorker
{
    private readonly IJobExecutor _executor;
    private readonly JobQueue _queue;
    private readonly Action<Job> _updateJob;
    private readonly int _maxConcurrency;

    public JobWorker(IJobExecutor executor, JobQueue queue, Action<Job> updateJob, int maxConcurrency)
    {
        _executor = executor;
        _queue = queue;
        _updateJob = updateJob;
        _maxConcurrency = maxConcurrency;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = _maxConcurrency,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(_queue.ReadAllAsync(cancellationToken), options, async (job, ct) =>
        {
            try
            {
                job.Status = JobStatus.Running;
                job.StartedAt = DateTimeOffset.UtcNow;
                _updateJob(job);

                var progress = new ProgressReporter(job, _updateJob);

                var result = await _executor.ExecuteAsync(job, progress, ct);

                job.CompletedAt = DateTimeOffset.UtcNow;
                job.Status = result.Success ? JobStatus.Succeeded : JobStatus.Failed;
                job.ErrorMessage = result.ErrorMessage;
            }
            catch (Exception ex)
            {
                job.Status = JobStatus.Failed;
                job.ErrorMessage = ex.Message;
                job.CompletedAt = DateTimeOffset.UtcNow;
            }
            finally
            {
                _updateJob(job);
            }
        });
    }
}
