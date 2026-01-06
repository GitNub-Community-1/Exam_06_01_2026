using System.Net;
using AutoMapper;
using Infastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebAPIWithJWTAndIdentity.Response;

public class UserActivityServicepublic(ApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache) : IUserActivityService
{
    public async Task<Response<UserActivityDto>> AddUserActivityAsync(ActivityTypeCreatDto listing)
    {
         var userActivity = mapper.Map<UserActivity>(listing);

        context.UserActivities.Add(userActivity);
        await context.SaveChangesAsync();

        var result = mapper.Map<UserActivityDto>(userActivity);
        memoryCache.Remove("userActivity_list");

        return new Response<UserActivityDto>(HttpStatusCode.Created, "User Activity created successfully!", result);
    }

    public async Task<Response<string>> DeleteUserActivityAsync(int id)
    {
          var userActivity = await context.UserActivities.FindAsync(id);
        if (userActivity == null)
            return new Response<string>(HttpStatusCode.NotFound, "User Activity not found");

        context.UserActivities.Remove(userActivity);
        await context.SaveChangesAsync();

        memoryCache.Remove($"userActivity_{id}");
        memoryCache.Remove("userActivity_list");

        return new Response<string>(HttpStatusCode.OK, "User Activity deleted successfully!");
    }

    public async Task<Response<List<UserActivityDto>>> GetUserActivitiesAsync(UserActivityFIlter filter)
    {
         const string cacheKey = "userActivity_list";

        if (memoryCache.TryGetValue(cacheKey, out Response<List<UserActivityDto>>? cachedResponse))
        {
            return cachedResponse!;
        }

        var query = context.UserActivities.AsQueryable();

        if (filter.Id.HasValue)
            query = query.Where(x => x.Id == filter.Id.Value);
        if (!filter.UserId.HasValue)
            query = query.Where(x => x.UserId == filter.UserId.Value);
        if (!string.IsNullOrEmpty(filter.Description))
            query = query.Where(x => x.Description.Contains(filter.Description));
        if (!filter.CreadAt.HasValue)
            query = query.Where(x => x.CreadAt == filter.CreadAt.Value);
        if (!filter.ActivityId.HasValue)
            query = query.Where(x => x.ActivityId == filter.ActivityId.Value);

        var userActivities = await query.ToListAsync();
        var result = mapper.Map<List<UserActivityDto>>(userActivities);

        var response = new Response<List<UserActivityDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User Activity retrieved successfully!",
            Data = result
        };

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));
        memoryCache.Set(cacheKey, response, cacheEntryOptions);

        return response;
    }

    public async Task<Response<UserActivityDto>> GetUserActivityByIdAsync(int id)
    {
        string cacheKey = $"userActivities_{id}";

        if (memoryCache.TryGetValue(cacheKey, out Response<UserActivityDto>? cachedResponse))
        {
            return cachedResponse!;
        }

        var userActivities = await context.UserActivities.FirstOrDefaultAsync(a => a.Id == id);
        if (userActivities == null)
            return new Response<UserActivityDto>(HttpStatusCode.NotFound, "User Activity not found");

        var result = mapper.Map<UserActivityDto>(userActivities);
        var response = new Response<UserActivityDto>(HttpStatusCode.OK, "User Activity retrieved successfully!", result);

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        memoryCache.Set(cacheKey, response, cacheEntryOptions);

        return response;
    }

    public async Task<Response<UserActivityDto>> UpdateUserActivityAsync(UserActivityDto listing)
    {
        var check = await context.UserActivities.FindAsync(listing.Id);
        if (check == null)
            return new Response<UserActivityDto>(HttpStatusCode.NotFound, "User Activity not found");

        
        check.UserId = listing.UserId;
        check.Description = listing.Description;
        check.CreadAt = listing.CreadAt;
        check.ActivityId = listing.ActivityId;

        await context.SaveChangesAsync();

        var result = mapper.Map<UserActivityDto>(check);

        memoryCache.Remove($"userActivities_{listing.Id}");
        memoryCache.Remove("userActivities_list");

        return new Response<UserActivityDto>(HttpStatusCode.OK, "User Activity updated successfully!", result);
    }
}