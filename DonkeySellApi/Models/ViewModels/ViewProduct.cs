using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DonkeySellApi.Models.DatabaseModels;

namespace DonkeySellApi.Models.ViewModels
{
    public class ViewProduct
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public ViewCategory Category { get; set; }

        [Required]
        public int CityId { get; set; }

        public ViewCity City { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public bool Rental { get; set; }
 
        public bool Free { get; set; }

        public bool TradesAccepted { get; set; }

        [Required]
        public int Price { get; set; }

        public string Title { get; set; }
       
        public DateTime DatePublished { get; set; }

        public string MeetingPoint { get; set; }
    }
}