using System;
using System.ComponentModel.DataAnnotations;

namespace DonkeySellApi.Models.ViewModels
{
    public class ViewMessage
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [MaxLength(200)]
        public string Value { get; set; }

        public DateTime DateCreated { get; set; }

        [Required]
        public int ProductId { get; set; }

        public bool MessageWasRead { get; set; }
    }
}