using MGM.MediaServices.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MGM.MediaServices.Core.Services;

public interface IJobStore
{
    Task SaveJobAsync(Job job, CancellationToken ct = default);
    Task<Job?> GetJobAsync(Guid id, CancellationToken ct = default);
    Task<List<Job>> GetPendingJobsAsync(CancellationToken ct = default);
    Task UpdateJobAsync(Job job, CancellationToken ct = default);
    IAsyncEnumerable<Job> GetAllJobsAsync(CancellationToken ct = default);
}