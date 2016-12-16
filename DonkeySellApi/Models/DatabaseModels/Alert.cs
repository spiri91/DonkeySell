

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DonkeySellApi.Models.DatabaseModels;

namespace DonkeySellApi.Models.Shared
{
    public class Alert
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual DonkeySellUser User { get; set; }

        public string ProductName { get; set; }
    }
}