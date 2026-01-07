using WebAPIWithJWTAndIdentity.Response;
using Domain.Dtos;
public interface IUserActivityService
{
    public  Task<Response<List<UserActivityDto>>> GetUserActivitiesAsync(UserActivityFIlter filter,int currentUserId);
     public  Task<Response<UserActivityDto>> AddUserActivityAsync(UserActivityCreatDto activityTypeCreatDto);
     public  Task<Response<UserActivityDto>> UpdateUserActivityAsync(UserActivityDto activityTypeCreatDto,int currentUserId);
     public  Task<Response<string>> DeleteUserActivityAsync(int id,int currentUserId);
     public  Task<Response<UserActivityDto>> GetUserActivityByIdAsync(int id,int currentUserId);
}