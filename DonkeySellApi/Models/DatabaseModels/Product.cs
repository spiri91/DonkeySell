using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonkeySellApi.Models.DatabaseModels
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual DonkeySellUser User { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public int Price { get; set; }

        public bool TradesAccepted { get; set; }

        public string Title { get; set; }

        public bool Rental { get; set; }

        public bool Free { get; set; }

        public DateTime DatePublished { get; set; }

        public string MeetingPoint { get; set; }
    }
}