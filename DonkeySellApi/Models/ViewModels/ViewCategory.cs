using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DonkeySellApi.Models.DatabaseModels;

namespace DonkeySellApi.Models.ViewModels
{
    public class ViewCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}