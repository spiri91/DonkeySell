using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DonkeySellApi.Models.ViewModels;

namespace DonkeySellApi.Models.Wrapers
{
    public class ViewProductsAndCount
    {
        public int Count;

        public IEnumerable<ViewProduct> Products;
    }
}