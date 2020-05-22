#nullable disable
namespace WageringGG.Shared.Models
{
    public class Bid
    {
        public int Id { get; set; }
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public bool? Approved { get; set; }
    }
}
