using System;
using DonkeySellApi.Extra;
using DonkeySellApi.Migrations;
using DonkeySellApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DonkeySell.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ShoudSeedTheDatabase()
        {
            DonkeySellDatabaseFunctions seedClass = new DonkeySellDatabaseFunctions();
            var seedSuccesful = seedClass.Seed();

            Assert.IsTrue(seedSuccesful);
        }
    }
}
