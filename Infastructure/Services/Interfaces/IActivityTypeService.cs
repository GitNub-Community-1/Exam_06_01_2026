using WebAPIWithJWTAndIdentity.Response;

public interface IActivityTypeService
{
    public  Task<Response<List<ActivityTypeDto>>> GetActivityTypesAsync(ActivityTypeFilter filter);
     public  Task<Response<ActivityTypeDto>> AddActivityTypeAsync(ActivityTypeCreatDto listing);
     public  Task<Response<ActivityTypeDto>> UpdateActivityTypeAsync(ActivityTypeDto listing);
     public  Task<Response<string>> DeleteActivityTypeAsync(int id);
     public  Task<Response<ActivityTypeDto>> GetActivityTypeByIdAsync(int id);
}