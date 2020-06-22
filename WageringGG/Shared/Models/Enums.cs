namespace WageringGG.Shared.Models
{
    public enum Status
    {
        Pending = 0,
        Confirmed = 1,
        Canceled = 2,
        Closed = 3,
        Completed = 8,
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
