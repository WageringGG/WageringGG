using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace WageringGG.Shared.Models
{
    public class Profile
    {
        /// <summary>
        /// Id should be the same as ApplicationUser.Id
        /// </summary>
        /// <value>Id</value>
        public string Id { get; set; }
        [RegularExpression(Constants.Regex.DisplayName)]
        [StringLength(12, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        public string DisplayName { get; set; }
        public string NormalizedDisplayName { get; set; }
        public bool IsVerified { get; set; }
        public string PublicKey { get; set; }
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public IList<Notification> Notifications { get; set; } = new List<Notification>();

        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsSelected { get; set; }
    }
}