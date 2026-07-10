using System.Diagnostics;
using MGM.MediaServices.Core.Models;

namespace MGM.MediaServices.Core.Services;

public class ExternalToolJobExecutor : IJobExecutor
{
    public async Task<JobExecutionResult> ExecuteAsync(
        Job job,
        IProgressReporter progressReporter,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTimeOffset.UtcNow;

        try
        {
            await progressReporter.ReportProgressAsync(0, $"Starting {job.Tool} on {Path.GetFileName(job.SourcePath)}...", cancellationToken);

            var command = job.CommandTemplate
                .Replace("{input}", job.SourcePath)
                .Replace("{output}", job.DestinationPath);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = job.Tool,
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Support for SoX and other tools is built-in via the Tool property

            using var process = new Process { StartInfo = processStartInfo };

            await progressReporter.ReportProgressAsync(10, "Executing command...", cancellationToken);

            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);

            await progressReporter.ReportProgressAsync(100, $"Completed {job.Tool}", cancellationToken);

            return new JobExecutionResult
            {
                Success = process.ExitCode == 0,
                ExitCode = process.ExitCode,
                Output = output + Environment.NewLine + error,
                Duration = DateTimeOffset.UtcNow - startTime
            };
        }
        catch (Exception ex)
        {
            var errorMsg = $"Failed to execute {job.Tool}: {ex.Message}";
            await progressReporter.ReportProgressAsync(0, errorMsg, cancellationToken);
            return new JobExecutionResult
            {
                Success = false,
                ErrorMessage = errorMsg,
                Duration = DateTimeOffset.UtcNow - startTime
            };
        }
    }
}
