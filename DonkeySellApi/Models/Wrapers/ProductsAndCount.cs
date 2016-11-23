using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DonkeySellApi.Models.DatabaseModels;

namespace DonkeySellApi.Models.Wrapers
{
    public class ProductsAndCount
    {
        public int Count;

        public IEnumerable<Product> Products;
    }
}