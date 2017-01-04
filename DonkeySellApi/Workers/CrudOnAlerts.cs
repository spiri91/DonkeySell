using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DonkeySellApi.Models;
using DonkeySellApi.Models.Shared;

namespace DonkeySellApi.Workers
{
    public interface ICrudOnAlerts
    {
        Task<List<Alert>> GetAlerts(string username);

        Task<Alert> AddAlert(Alert alert);

        Task<int> DeleteAlert(int id);

        Task<List<string>> GetUsersWithAlertProduct(string productName);
        Task DeleteAlertsOfUser(string username);
    }

    public class CrudOnAlerts:ICrudOnAlerts, IDisposable
    {
        private DonkeySellContext context;

        public CrudOnAlerts()
        {
            context = new DonkeySellContext();
        }

        public async Task<List<Alert>> GetAlerts(string username)
        {
            var alerts = context.Alerts.Where(x => x.User.UserName == username).ToList();

            return alerts;
        }

        public async Task<Alert> AddAlert(Alert alert)
        {
            if (!alert.IsValid())
                throw new FormatException();

            if(!context.Users.Any(x => x.UserId == alert.UserId))
                throw new ObjectNotFoundException();

            var newAlert = context.Alerts.Add(alert);

            await context.SaveChangesAsync();

            return newAlert;
        }

        public async Task<int> DeleteAlert(int id)
        {
            if(!context.Alerts.Any(x => x.Id == id))
                throw  new ObjectNotFoundException();

            var alert = context.Alerts.Single(x => x.Id == id);
            context.Alerts.Remove(alert);

            await context.SaveChangesAsync();

            return alert.Id;
        }

        public async Task<List<string>> GetUsersWithAlertProduct(string productName)
        {
            var userThatWhatProduct =
                context.Alerts.Where(x => productName.ToLower().Contains(x.ProductName.ToLower()))
                    .Select(x => x.User.Email).ToList();

            return userThatWhatProduct;
        }

        public async Task DeleteAlertsOfUser(string username)
        {
            var userId = context.Users.Single(x => x.UserName == username).UserId;
            var alerts = context.Alerts.Where(x => x.UserId == userId).ToList();
            context.Alerts.RemoveRange(alerts);

            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}