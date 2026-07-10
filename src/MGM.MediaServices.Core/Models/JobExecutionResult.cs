namespace MGM.MediaServices.Core.Models;

public class JobExecutionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int ExitCode { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Output { get; set; }
}
