using System.Net;
using AutoMapper;
using Infastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebAPIWithJWTAndIdentity.Response;

public class ActivityTypeService(ApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache) : IActivityTypeService
{
    public async Task<Response<ActivityTypeDto>> AddActivityTypeAsync(ActivityTypeCreatDto listing)
    {
        var activityType = mapper.Map<ActivityType>(listing);

        context.ActivityTypes.Add(activityType);
        await context.SaveChangesAsync();

        var result = mapper.Map<ActivityTypeDto>(activityType);
        memoryCache.Remove("activityType_list");

        return new Response<ActivityTypeDto>(HttpStatusCode.Created, "Activity Type created successfully!", result);
    }

    public async Task<Response<string>> DeleteActivityTypeAsync(int id)
    {
         var activityType = await context.ActivityTypes.FindAsync(id);
        if (activityType == null)
            return new Response<string>(HttpStatusCode.NotFound, "Activity Type not found");

        context.ActivityTypes.Remove(activityType);
        await context.SaveChangesAsync();

        memoryCache.Remove($"activityType_{id}");
        memoryCache.Remove("activityType_list");

        return new Response<string>(HttpStatusCode.OK, "Activity Type deleted successfully!");
    }

    public async Task<Response<ActivityTypeDto>> GetActivityTypeByIdAsync(int id)
    {
        string cacheKey = $"activityType_{id}";

        if (memoryCache.TryGetValue(cacheKey, out Response<ActivityTypeDto>? cachedResponse))
        {
            return cachedResponse!;
        }

        var activityType = await context.ActivityTypes.FirstOrDefaultAsync(a => a.Id == id);
        if (activityType == null)
            return new Response<ActivityTypeDto>(HttpStatusCode.NotFound, "Activity Type not found");

        var result = mapper.Map<ActivityTypeDto>(activityType);
        var response = new Response<ActivityTypeDto>(HttpStatusCode.OK, "Activity Type retrieved successfully!", result);

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        memoryCache.Set(cacheKey, response, cacheEntryOptions);

        return response;
    }

    public async Task<Response<List<ActivityTypeDto>>> GetActivityTypesAsync(ActivityTypeFilter filter)
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

        var response = new Response<List<ActivityTypeDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Activity Type retrieved successfully!",
            Data = result
        };

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));
        memoryCache.Set(cacheKey, response, cacheEntryOptions);

        return response;
    }

    public async Task<Response<ActivityTypeDto>> UpdateActivityTypeAsync(ActivityTypeDto listing)
    {
         var check = await context.ActivityTypes.FindAsync(listing.Id);
        if (check == null)
            return new Response<ActivityTypeDto>(HttpStatusCode.NotFound, "Activity Type not found");

        
        check.Name = listing.Name;

        await context.SaveChangesAsync();

        var result = mapper.Map<ActivityTypeDto>(check);

        memoryCache.Remove($"activityTypes_{listing.Id}");
        memoryCache.Remove("activityTypes_list");

        return new Response<ActivityTypeDto>(HttpStatusCode.OK, "Activity Type updated successfully!", result);
    }
}
