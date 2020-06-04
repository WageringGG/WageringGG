using System;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace WageringGG.Shared.Models
{
    public class Notification
    {
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Link { get; set; }
    }
}