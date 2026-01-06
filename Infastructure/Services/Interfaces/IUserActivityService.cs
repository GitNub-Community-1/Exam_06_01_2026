using WebAPIWithJWTAndIdentity.Response;
using Domain.Dtos;
public interface IUserActivityService
{
    public  Task<Response<List<UserActivityDto>>> GetUserActivitiesAsync(UserActivityFIlter filter);
     public  Task<Response<UserActivityDto>> AddUserActivityAsync(ActivityTypeCreatDto activityTypeCreatDto);
     public  Task<Response<UserActivityDto>> UpdateUserActivityAsync(UserActivityDto activityTypeCreatDto);
     public  Task<Response<string>> DeleteUserActivityAsync(int id);
     public  Task<Response<UserActivityDto>> GetUserActivityByIdAsync(int id);
}