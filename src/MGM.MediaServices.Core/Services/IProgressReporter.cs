namespace MGM.MediaServices.Core.Services;

public interface IProgressReporter
{
    Task ReportProgressAsync(int percent, string message, CancellationToken cancellationToken = default);
}
