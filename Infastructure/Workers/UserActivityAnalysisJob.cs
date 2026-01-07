using Quartz;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Infastructure.Data;

public class UserActivityAnalysisJob : IJob
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserActivityAnalysisJob> _logger;

    public UserActivityAnalysisJob(ApplicationDbContext context, ILogger<UserActivityAnalysisJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Quartz Job started at {Time}", DateTime.UtcNow);
        var stats = await _context.UserActivities
            .GroupBy(a => a.ActivityId)
            .Select(g => new { ActivityId = g.Key, Count = g.Count() })
            .ToListAsync();

        foreach (var stat in stats)
        {
            _logger.LogInformation("ActivityType {ActivityId} has {Count} actions", stat.ActivityId, stat.Count);
        }

        _logger.LogInformation("Quartz Job finished at {Time}", DateTime.UtcNow);
    }
}
