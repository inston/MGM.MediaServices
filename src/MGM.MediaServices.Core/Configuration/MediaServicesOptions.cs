namespace MGM.MediaServices.Core.Configuration;

public class MediaServicesOptions
{
    public int MaxConcurrentJobs { get; set; } = Environment.ProcessorCount;
    public string FfmpegPath { get; set; } = "/usr/bin/ffmpeg";
    public string SoxPath { get; set; } = "/usr/bin/sox";
    public int DefaultTimeoutSeconds { get; set; } = 3600;
}
