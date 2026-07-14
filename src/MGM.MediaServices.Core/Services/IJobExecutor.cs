using MGM.MediaServices.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MGM.MediaServices.Core.Services;

public interface IJobExecutor
{
    Task<JobExecutionResult> ExecuteAsync(
        Job job,
        IProgressReporter progressReporter,
        CancellationToken cancellationToken = default);
}