using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonkeySellApi.Models.Shared
{
    public class UsersFavoriteProducts
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        public int ProductId { get; set; }
    }
}