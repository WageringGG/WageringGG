using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class WagerBid : Bid
    {
        [Required]
        [Column(TypeName = "tinyint unsigned")]
        public int ReceivablePt { get; set; }
        [Required]
        [Column(TypeName = "tinyint unsigned")]
        public int PayablePt { get; set; }
        public bool IsOwner { get; set; }

        [NotMapped]
        public string DisplayName { get; set; }
        [NotMapped]
        public bool IsEditing { get; set; }
    }
}