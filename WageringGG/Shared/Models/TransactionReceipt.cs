using System;

namespace WageringGG.Shared.Models
{
    public class TransactionReceipt
    {
        public int Id { get; set; }

        public string ProfileId { get; set; }
        public Profile Profile { get; set; }

        public DateTime Date { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public string Data { get; set; }
    }
}
