using System.Threading.Channels;
using MGM.MediaServices.Core.Models;

namespace MGM.MediaServices.Worker;

public class JobQueue
{
    private readonly Channel<Job> _channel = Channel.CreateUnbounded<Job>();

    public ValueTask EnqueueAsync(Job job) => _channel.Writer.WriteAsync(job);

    public IAsyncEnumerable<Job> ReadAllAsync(CancellationToken cancellationToken = default)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}
