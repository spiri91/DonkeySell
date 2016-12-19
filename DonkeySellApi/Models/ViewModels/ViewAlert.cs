using System.ComponentModel.DataAnnotations;

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