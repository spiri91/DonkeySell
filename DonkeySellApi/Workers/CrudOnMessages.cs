using System;
using System.Collections.Generic;
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

    public class CrudOnMessages: ICrudOnMessages
    {
        private DonkeySellContext context;
        public CrudOnMessages()
        {
            context = new DonkeySellContext();
        }

        public async Task<List<Message>> GetMessages(int productId)
        {
            return context.Messages.Where(x => x.ProductId == productId).ToList();
        }

        public async Task<Message> AddOrUpdate(Message message)
        {
            if (message.Id != 0 || context.Messages.Any(x => x.Id == message.Id))
            {
                var poco = context.Messages.Single(x => x.Id == message.Id);
                poco.Value = message.Value;
                poco.DateCreated = DateTime.Now;
                await context.SaveChangesAsync();

                return poco;
            }

            message.DateCreated = DateTime.Now;
            message.UserId = context.Users.Single(x => x.UserName == message.UserName).UserId;
            context.Messages.Add(message);
            await context.SaveChangesAsync();

            return message;
        }

        public async Task<int> DeleteMessage(int id)
        {
            var poco = context.Messages.Single(x => x.Id == id);
            context.Messages.Remove(poco);
            await context.SaveChangesAsync();

            return poco.Id;
        }

        public async Task DeleteAllMessagesForProduct(int id)
        {
            var pocos = context.Messages.Where(x => x.ProductId == id);
            pocos.ForEach(async x =>
            {
                await DeleteMessage(x.Id);
            });
        }

        public async Task<List<Message>> GetUnreadMessages(string username)
        {
            var pocos = context.Messages.Where(x => x.UserName == username && x.MessageWasRead == false);
            return pocos.ToList();
        }

        public async Task<Message> MessageWasRead(int id)
        {
            var message = context.Messages.Single(x => x.Id == id);
            message.MessageWasRead = true;
            await context.SaveChangesAsync();

            return message;
        }
    }
}