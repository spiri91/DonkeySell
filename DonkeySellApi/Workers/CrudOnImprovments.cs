using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DonkeySellApi.Extra;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;

namespace DonkeySellApi.Workers
{
    public interface ICrudOnImprovements
    {
        Task AddImprovement(Improvement improvement);
    }

    public class CrudOnImprovements : ICrudOnImprovements
    {
        private DonkeySellContext context;

        public CrudOnImprovements(DonkeySellContext context)
        {
            this.context = context;
        }

        public async Task AddImprovement(Improvement improvement)
        {
            if(!improvement.IsValid())
                throw new FormatException();

            context.Improvements.Add(improvement);
            await context.SaveChangesAsync();
        }
    }
}