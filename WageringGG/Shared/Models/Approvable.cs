namespace WageringGG.Shared.Models
{
    public abstract class Approvable
    {
        public byte Status { get; set; }
        public abstract string GroupName { get; }

        public abstract bool IsApproved();
    }
}
