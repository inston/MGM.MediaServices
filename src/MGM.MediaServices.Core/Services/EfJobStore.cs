using Microsoft.EntityFrameworkCore;
using MGM.MediaServices.Core.Data;
using MGM.MediaServices.Core.Models;

namespace MGM.MediaServices.Core.Services;

public class EfJobStore : IJobStore
{
    private readonly AppDbContext _context;

    public EfJobStore(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveJobAsync(Job job, CancellationToken ct = default)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Job?> GetJobAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Jobs.FindAsync([id], cancellationToken: ct);
    }

    public async Task<List<Job>> GetPendingJobsAsync(CancellationToken ct = default)
    {
        return await _context.Jobs
            .Where(j => j.Status == JobStatus.Queued || j.Status == JobStatus.Running)
            .ToListAsync(ct);
    }

    public async Task UpdateJobAsync(Job job, CancellationToken ct = default)
    {
        _context.Jobs.Update(job);
        await _context.SaveChangesAsync(ct);
    }

    public IAsyncEnumerable<Job> GetAllJobsAsync(CancellationToken ct = default)
    {
        return _context.Jobs.AsAsyncEnumerable();
    }
}