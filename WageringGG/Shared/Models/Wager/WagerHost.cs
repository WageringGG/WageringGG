using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class WagerHost
    {
        public int Id { get; set; }
        public int WagerId { get; set; }
        public Wager Wager { get; set; }
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public bool? IsApproved { get; set; }

        [Column(TypeName = "decimal(18,7)")]
        public decimal Payable { get; set; }
        [Column(TypeName = "decimal(18,7)")]
        public decimal Receivable { get; set; }
    }
}
