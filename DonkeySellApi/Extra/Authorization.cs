using System;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using DonkeySellApi.Models;

namespace DonkeySellApi.Extra
{
    public interface IAuthorization
    {
        Task<bool> UserHasRightsOnMessage(string username, int messageId);

        Task<bool> UserCanUpdateProduct(string username, int productId, string viewProductUserName);

        Task<bool> UserIsHimself(string currentUserName, string usernameForDelete);

        Task<bool> UserOwnsThisProduct(string getUserName, string username, int id);

        Task<bool> UserCanDeleteProduct(string getUserName, int id);

        Task<bool> UserOwnsThisAlert(int id, string userId);
    }

    public class Authorization : IAuthorization, IDisposable
    {
        private DonkeySellContext context;

        public Authorization(DonkeySellContext context)
        {
            this.context = context;
        }

        public async Task<bool> UserIsHimself(string currentUserName, string username)
        {
            return currentUserName == username;
        }

        public async Task<bool> UserHasRightsOnMessage(string username, int messageId)
        {
            if (messageId == 0)
                return true;

            if(!context.Messages.Any(x => x.Id == messageId))
                throw new ObjectNotFoundException();

            var messageUsername = context.Messages.Single(x => x.Id == messageId).UserName;

            return messageUsername == username;
        }

        public async Task<bool> UserCanUpdateProduct(string username, int productId, string viewProductUserName)
        {
            if (productId == 0)
                return true;

            if(!context.Products.Any(x => x.Id == productId))
                throw new ObjectNotFoundException();

            var productUsername = context.Products.Single(x => x.Id == productId).UserName;

            if ((username == productUsername) && (username == viewProductUserName))
                return true;

            return false;
        }

        public async Task<bool> UserOwnsThisProduct(string currentUserName, string username, int id)
        {
            if(!context.Messages.Any(x => x.Id == id))
                throw new ObjectNotFoundException();

            var productId = context.Messages.Single(x => x.Id == id).ProductId;

            if(!context.Products.Any(x => x.Id == productId))
                throw new ObjectNotFoundException();

            var productOwnerName = context.Products.Single(x => x.Id == productId).UserName;
            if ((productOwnerName == currentUserName) && (currentUserName == username))
                return true;

            return false;
        }

        public async Task<bool> UserCanDeleteProduct(string username, int id)
        {
            if(!context.Products.Any(x => x.Id == id))
                throw new ObjectNotFoundException();

            var productUsername = context.Products.Single(x => x.Id == id).UserName;

            return username == productUsername;
        }

        public async Task<bool> UserOwnsThisAlert(int id, string username)
        {
            if(! context.Alerts.Any(x => x.Id == id))
                throw new ObjectNotFoundException();

            var alert = context.Alerts.Single(x => x.Id == id);

            return alert.User.UserName == username;
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}