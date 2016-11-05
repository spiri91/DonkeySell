using System.ComponentModel.DataAnnotations.Schema;

namespace DonkeySellApi.Models.ViewModels
{
    public class ViewUsersFavoriteProducts
    {
        public string Username { get; set; }

        public int ProductId { get; set; }
    }
}