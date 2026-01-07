using WebAPIWithJWTAndIdentity.Response;

public interface IActivityTypeService
{
    public  Task<Response<List<ActivityTypeDto>>> GetActivityTypesAsync(ActivityTypeFilter filter, int currentUserId);
     public  Task<Response<ActivityTypeDto>> AddActivityTypeAsync(ActivityTypeCreatDto listing);
     public  Task<Response<ActivityTypeDto>> UpdateActivityTypeAsync(ActivityTypeDto listing, int currentUserId);
     public  Task<Response<string>> DeleteActivityTypeAsync(int id, int currentUserId);
     public  Task<Response<ActivityTypeDto>> GetActivityTypeByIdAsync(int id, int currentUserId);
}