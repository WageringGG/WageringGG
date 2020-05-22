#nullable disable
namespace WageringGG.Shared.Models
{
    public class WagerRule
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public Rule Rule { get; set; }
        public int WagerId { get; set; }
        public Wager Wager { get; set; }
    }
}
