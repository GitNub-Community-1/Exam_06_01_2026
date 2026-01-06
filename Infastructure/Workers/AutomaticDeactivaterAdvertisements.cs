// using Infastructure.Data;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;

// namespace Infastructure.Workers;

// public class AutomaticDeactivaterAdvertisements : BackgroundService
// {
//     private readonly IServiceScopeFactory _scopeFactory;
//     private Timer _timer;

//     public AutomaticDeactivaterAdvertisements(IServiceScopeFactory scopeFactory)
//     {
//         _scopeFactory = scopeFactory;
//     }

//     protected override Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         _timer = new Timer(Work, null, TimeSpan.FromSeconds(2), Timeout.InfiniteTimeSpan);
//         return Task.CompletedTask;
//     }

//     private async void Work(object? state)
//     {
//         var weeks = 1;
//         var date = DateTime.UtcNow.AddDays((-weeks*7));
//         using var scope = _scopeFactory.CreateScope();
//         var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//         var products = await context.Advertisements
//             .Where(x => x.Status == 1 && x.CreatedDate <= date)
//             .ToListAsync();
//         foreach (var item in products)
//         {
//             item.Status = 0;
//             item.DisActiveDate = DateTime.UtcNow;
//         }
//         await context.SaveChangesAsync();
//         _timer.Change(TimeSpan.FromSeconds(2), Timeout.InfiniteTimeSpan);
//     }
// }
