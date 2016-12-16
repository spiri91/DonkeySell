using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DonkeySellApi.Models.Shared;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DonkeySellApi.Models.DatabaseModels
{
    public class DonkeySellUser: IdentityUser
    {
        [Key]
        public string UserId { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public string Address { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string Phone { get; set; }

        public string Avatar { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public string ConfirmationGuid { get; set; }

        public virtual ICollection<Alert> Alerts { get; set; }
    }
}