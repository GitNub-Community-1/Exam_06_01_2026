using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<ActivityType> ActivityTypes  { get; set; }

}