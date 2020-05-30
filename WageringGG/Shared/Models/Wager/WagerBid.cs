using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class WagerBid : Bid
    {
        [Required]
        public byte ReceivablePt { get; set; }
        [Required]
        public byte PayablePt { get; set; }
        public bool IsOwner { get; set; }

        [NotMapped]
        public string DisplayName { get; set; }
        [NotMapped]
        public bool IsEditing { get; set; }
    }
}