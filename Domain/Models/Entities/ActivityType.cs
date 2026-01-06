public class ActivityType : BaseEntity
{
    public string Name {get;set;}
    
    public List<UserActivity> UserActivities;
}