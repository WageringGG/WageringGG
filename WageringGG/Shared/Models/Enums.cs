namespace WageringGG.Shared.Models
{
    public enum Status
    {
        Created = 0,
        Confirmed = 1,
        Canceled = 2,
        Closed = 3,
        Ongoing = 4,
        Completed = 8
    }

    //Used for notifications
    public enum DataModel
    {
        None = 0,
        ApplicationUser = 10,
        Wager = 20,
        Tournament = 30,
    }
}
