using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonkeySellApi.Models.DatabaseModels
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
       
        public string UserId { get; set; }

        public string UserName { get; set; }

        [ForeignKey("UserId")]
        public virtual DonkeySellUser User { get; set; }

        public string Value { get; set; }

        public DateTime DateCreated { get; set; }
       
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public bool MessageWasRead { get; set; }
    }
}