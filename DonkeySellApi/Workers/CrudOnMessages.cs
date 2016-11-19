using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using WebGrease.Css.Extensions;

namespace DonkeySellApi.Workers
{
    public interface ICrudOnMessages
    {
        Task<List<Message>> GetMessages(int productId);

        Task<Message> AddOrUpdate(Message message);

        Task<int> DeleteMessage(int id);

        Task DeleteAllMessagesForProduct(int id);

        Task<List<Message>> GetUnreadMessages(string username);

        Task<Message> MessageWasRead(int id);
    }

    public class CrudOnMessages : ICrudOnMessages, IDisposable
    {
        private DonkeySellContext context;

        public CrudOnMessages()
        {
            context = new DonkeySellContext();
        }

        public async Task<List<Message>> GetMessages(int productId)
        {
            if (!context.Products.Any(x => x.Id == productId))
                throw new ObjectNotFoundException();

            return context.Messages.Where(x => x.ProductId == productId).ToList();
        }

        public async Task<Message> AddOrUpdate(Message message)
        {
            if (message.Id == 0)
                return await CreateMessage(message);

            if (!context.Messages.Any(x => x.Id == message.Id))
                throw new ObjectNotFoundException();

            return await UpdateMessage(message);
        }

        private async Task<Message> CreateMessage(Message message)
        {
            if (!message.IsValid())
                throw new FormatException();

            if(!context.Users.Any(x => x.UserName == message.UserName))
                throw new ObjectNotFoundException();

            message.DateCreated = DateTime.Now;
            message.UserId = context.Users.Single(x => x.UserName == message.UserName).UserId;
            context.Messages.Add(message);
            await context.SaveChangesAsync();

            return message;
        }

        private async Task<Message> UpdateMessage(Message message)
        {
            var poco = context.Messages.Single(x => x.Id == message.Id);
            poco.Value = message.Value;
            poco.DateCreated = DateTime.Now;
            await context.SaveChangesAsync();

            return poco;
        }

        public async Task<int> DeleteMessage(int id)
        {
            if(!context.Messages.Any(x => x.Id == id))
                throw new ObjectNotFoundException();

            var poco = context.Messages.Single(x => x.Id == id);
            context.Messages.Remove(poco);
            await context.SaveChangesAsync();

            return poco.Id;
        }

        public async Task DeleteAllMessagesForProduct(int id)
        {
            if(!context.Products.Any(x => x.Id == id))
                throw new ObjectNotFoundException();

            var pocos = context.Messages.Where(x => x.ProductId == id);
            pocos.ForEach(async x =>
            {
                await DeleteMessage(x.Id);
            });
        }

        public async Task<List<Message>> GetUnreadMessages(string username)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            var pocos = context.Messages.Where(x => x.UserName == username && x.MessageWasRead == false);
            return pocos.ToList();
        }

        public async Task<Message> MessageWasRead(int id)
        {
            if(!context.Messages.Any(x => x.Id == id))
                throw new ObjectNotFoundException();

            var message = context.Messages.Single(x => x.Id == id);
            message.MessageWasRead = true;
            await context.SaveChangesAsync();

            return message;
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}