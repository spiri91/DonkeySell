using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DonkeySellApi.Models.ViewModels
{
    public class ViewAlert
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string ProductName { get; set; }
    }
}