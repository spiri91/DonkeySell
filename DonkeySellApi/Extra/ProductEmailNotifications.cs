using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DonkeySellApi.Workers;
using Ninject;
using WebGrease.Css.Extensions;

namespace DonkeySellApi.Extra
{
    public class ProductEmailNotifications
    {
        private ICrudOnAlerts crudOnAlerts;
        private IMailSender mailSender;

        public ProductEmailNotifications(ICrudOnAlerts crudOnAlerts, IMailSender mailSender)
        {
            this.crudOnAlerts = crudOnAlerts;
            this.mailSender = mailSender;
        }

        public async void SendEmailForProduct(string productTitle, int productId)
        {
            var emails = await crudOnAlerts.GetUsersWithAlertProduct(productTitle);
            emails.AsParallel().ForEach(e =>
            {
                mailSender.SentProductAlert(productId, e, productTitle);
            });
        }
    }
}