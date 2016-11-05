using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DonkeySellApi.Models.DatabaseModels;

namespace DonkeySellApi.Models.ViewModels
{
    public class ViewUser
    {
        public string UserId { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }

        [Required]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Password { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        public string Avatar { get; set; }
    }
}