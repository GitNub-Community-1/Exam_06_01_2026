using System.Net;
using AutoMapper;
using Infastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WebAPIWithJWTAndIdentity.Response;
public class ActivityTypeService(ApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache, ILogger<ActivityTypeService> _logger) : IActivityTypeService
{
    public async Task<Response<ActivityTypeDto>> AddActivityTypeAsync(ActivityTypeCreatDto listing)
    {
        var accountType = mapper.Map<ActivityType>(listing);

        context.ActivityTypes.Add(accountType);
        await context.SaveChangesAsync();

        var result = mapper.Map<ActivityTypeDto>(accountType);
        _logger.LogInformation("Activity: Added AccountType {@Activity}", new
        {
            Id = result.Id,
            Name = result.Name,
            Time = DateTime.UtcNow
        });

        memoryCache.Remove("accountType_list");

        return new Response<ActivityTypeDto>(HttpStatusCode.Created, "ActivityTypeDto created successfully!", result);
    }

    public async Task<Response<string>> DeleteActivityTypeAsync(int id, int currentUserId)
    {
        var accountType = await context.ActivityTypes.FindAsync(id);
        if (accountType == null)
            return new Response<string>(HttpStatusCode.NotFound, "ActivityTypeDto not found");

        context.ActivityTypes.Remove(accountType);
        await context.SaveChangesAsync();

        _logger.LogInformation("Activity: Deleted ActivityTypeDto {@Activity}", new
        {
            RequestUserId = currentUserId,
            DeletedId = id,
            Time = DateTime.UtcNow
        });

        memoryCache.Remove($"activityType_{id}");
        memoryCache.Remove("activityType_list");

        return new Response<string>(HttpStatusCode.OK, "ActivityType deleted successfully!");
    }

    public async Task<Response<List<ActivityTypeDto>>> GetActivityTypesAsync(ActivityTypeFilter filter, int currentUserId)
    {
        const string cacheKey = "activityType_list";

        if (memoryCache.TryGetValue(cacheKey, out Response<List<ActivityTypeDto>>? cachedResponse))
        {
            return cachedResponse!;
        }

        var query = context.ActivityTypes.AsQueryable();

        if (filter.Id.HasValue)
            query = query.Where(x => x.Id == filter.Id.Value);
        if (!string.IsNullOrEmpty(filter.Name))
            query = query.Where(x => x.Name.Contains(filter.Name));

        var activityTypes = await query.ToListAsync();
        var result = mapper.Map<List<ActivityTypeDto>>(activityTypes);

        _logger.LogInformation("Activity: Getting AccountTypes {@Activity}", new
        {
            RequestUserId = currentUserId,
            Count = result.Count,
            Time = DateTime.UtcNow
        });

        var response = new Response<List<ActivityTypeDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "ActivityTypes retrieved successfully!",
            Data = result
        };

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));
        memoryCache.Set(cacheKey, response, cacheEntryOptions);

        return response;
    }

    public async Task<Response<ActivityTypeDto>> GetActivityTypeByIdAsync(int id, int currentUserId)
    {
        string cacheKey = $"activityType_{id}";

        if (memoryCache.TryGetValue(cacheKey, out Response<ActivityTypeDto>? cachedResponse))
        {
            return cachedResponse!;
        }

        var activityType = await context.ActivityTypes.FirstOrDefaultAsync(a => a.Id == id);
        if (activityType == null)
            return new Response<ActivityTypeDto>(HttpStatusCode.NotFound, "ActivityType not found");

        var result = mapper.Map<ActivityTypeDto>(activityType);

        _logger.LogInformation("Activity: Getting ActivityType By ID {@Activity}", new
        {
            RequestUserId = currentUserId,
            Data = result,
            Time = DateTime.UtcNow
        });

        var response = new Response<ActivityTypeDto>(HttpStatusCode.OK, "ActivityType retrieved successfully!", result);

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        memoryCache.Set(cacheKey, response, cacheEntryOptions);

        return response;
    }

    public async Task<Response<ActivityTypeDto>> UpdateActivityTypeAsync(ActivityTypeDto listing, int currentUserId)
    {
        var check = await context.ActivityTypes.FindAsync(listing.Id);
        if (check == null)
            return new Response<ActivityTypeDto>(HttpStatusCode.NotFound, "ActivityType not found");

        check.Name = listing.Name;

        await context.SaveChangesAsync();

        var result = mapper.Map<ActivityTypeDto>(check);

        _logger.LogInformation("Activity: Updated ActivityType {@Activity}", new
        {
            RequestUserId = currentUserId,
            Data = result,
            Time = DateTime.UtcNow
        });

        memoryCache.Remove($"activityType_{listing.Id}");
        memoryCache.Remove("activityType_list");

        return new Response<ActivityTypeDto>(HttpStatusCode.OK, "ActivityType updated successfully!", result);
    }
}
