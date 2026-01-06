public class UserActivityCreatDto 
{
    public int UserId {get;set;}
    public string Description {get;set;}
    public DateTime CreadAt {get;set;} = DateTime.UtcNow;
    public int ActivityId{get;set;}
}