using DonkeySellApi.Extra;
using NUnit.Framework;

namespace DonkeySell.Tests.Unit_tests
{
    [TestFixture]
    class MyQueryBuilderTests
    {
        [Test]
        public void ShouldBuildComplexQuery()
        {
            var givenString = "Username,spiri0c7fa;CityId,2;Price,1400,2000;TradesAccepted,1;CategoryId,4";
            var desiredString =
                "select * from Product where Username ='spiri0c7fa' and CityId ='2' and Price >'1400' and Price <'2000' and TradesAccepted ='1' and CategoryId ='4'";

            var myQueryBuilder = new MyQueryBuilder();
            var result = myQueryBuilder.BuildQuery(givenString).Result;
            Assert.AreEqual(result, desiredString);
        }

        [Test]
        public void ShouldBuildSimpleQuery()
        {
            var givenString = "CityId,2";
            var desiredString = "select * from Product where CityId ='2'";

            var myQueryBuilder = new MyQueryBuilder();
            var result = myQueryBuilder.BuildQuery(givenString).Result;
            Assert.AreEqual(result, desiredString);
        }

        [Test]
        public void ShouldBuildQueryWithLike()
        {
            var givenString = "Title,samsung,samsung,true";
            var desiredString = "select * from Product where Title like '%samsung%'";

            var myQueryBuilder = new MyQueryBuilder();
            var result = myQueryBuilder.BuildQuery(givenString).Result;
            Assert.AreEqual(result, desiredString);
        }
    }
}
