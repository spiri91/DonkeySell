using System.Linq;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Workers;
using Ninject;
using NUnit.Framework;

namespace DonkeySell.Tests.Unit_tests
{
    [TestFixture]
    class CrudOnMessagesTests
    {
        private ICrudOnMessages crudOnMessages;
        private Message message;
        private int productId;
        
        [SetUp]
        public void Initialize()
        {
            TestInitialiser.Initialise();
            crudOnMessages = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnMessages>();
            message = TestInitialiser.CreateMessage();
            productId = TestInitialiser.context.Products.ToList()[0].Id;
        }

        [Test]
        public void ShouldGetMessages()
        {
            var dbMessage = crudOnMessages.AddOrUpdate(message).Result;
            var messages = crudOnMessages.GetMessages(productId).Result;
            Assert.IsNotNull(messages);
            crudOnMessages.DeleteMessage(dbMessage.Id).Wait();
        }

        [Test]
        public void ShouldAddMessage()
        {
            var dbMessage = crudOnMessages.AddOrUpdate(message).Result;
            Assert.NotNull(dbMessage);
            crudOnMessages.DeleteMessage(dbMessage.Id).Wait();
        }

        [Test]
        public void ShouldUpdateMessage()
        {
            string newValueForMessage = "another value";
            var dbMessage = crudOnMessages.AddOrUpdate(message).Result;
            dbMessage.Value = newValueForMessage;
            var updatedMessage = crudOnMessages.AddOrUpdate(dbMessage).Result;
            Assert.AreEqual(dbMessage.Value, updatedMessage.Value);
            crudOnMessages.DeleteMessage(updatedMessage.Id).Wait();
        }

        [Test]
        public void ShouldDeleteMessage()
        {
            var dbMessage = crudOnMessages.AddOrUpdate(message).Result;
            var deletedId = crudOnMessages.DeleteMessage(dbMessage.Id).Result;
            Assert.NotNull(deletedId);
        }

        [Test]
        public void ShouldMarkMessageAsRead()
        {
            var dbMessage = crudOnMessages.AddOrUpdate(message).Result;
            var updatedMessage = crudOnMessages.MessageWasRead(dbMessage.Id).Result;
            Assert.IsTrue(updatedMessage.MessageWasRead);
            crudOnMessages.DeleteMessage(dbMessage.Id).Wait();
        }
    }
}
