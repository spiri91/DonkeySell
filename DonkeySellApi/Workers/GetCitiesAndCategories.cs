using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;

namespace DonkeySellApi.Workers
{
    public interface IGetCitiesAndCategories
    {
        Task<List<Category>> GetCategories();
        Task<List<City>> GetCities();
    }


    public class GetCitiesAndCategories: IGetCitiesAndCategories
    {
        private DonkeySellContext context;

        public GetCitiesAndCategories(DonkeySellContext context)
        {
            this.context = context;
        }

        public async Task<List<Category>> GetCategories()
        {
            return context.Categories.ToList();
        }

        public async Task<List<City>> GetCities()
        {
            return context.Cities.ToList();
        }
    }
}