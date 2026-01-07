using System.Text;
using Domain.Dtos;
using Infastructure.AutoMapper;
using Infastructure.Data;
using Infrastructure.Seed;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Serilog;
using WebAPIWithJWTAndIdentity.MiddleWare;

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Information()
.WriteTo.Console()
.WriteTo.File(
    "logs/app-.log",
    rollingInterval: RollingInterval.Day
)
.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connection));

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
if (emailConfig != null)
{
    builder.Services.AddSingleton(emailConfig);
}
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddIdentity<User, IdentityRole<int>>(config =>
{
    config.Password.RequiredLength = 4;
    config.Password.RequireDigit = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
    config.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
})
.AddJwtBearer(o =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!);
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var jobKey = new JobKey("UserActivityAnalysisJob");
    q.AddJob<UserActivityAnalysisJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("UserActivityAnalysisJob-trigger")
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(10).RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


builder.Services.AddMemoryCache();
builder.Services.AddScoped<Seeder>();

builder.Services.AddScoped<IUserActivityService,UserActivityService>();
builder.Services.AddScoped<IActivityTypeService,ActivityTypeService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Real Estate Platform API",
        Version = "v1",
        Description = "API for real estate listings and categories"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();


if (!app.Environment.IsEnvironment("EfMigration"))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try 
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        var seeder = services.GetRequiredService<Seeder>();
        await seeder.SeedRole();
        await seeder.SeedUser();
    }
    catch (Exception ex)
    {

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration or seeding.");
    }
}

app.UseMiddleware<CustomLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();