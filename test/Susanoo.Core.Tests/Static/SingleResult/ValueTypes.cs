using NUnit.Framework;
using System;
using System.Linq;
using static Susanoo.SusanooCommander;

namespace Susanoo.Tests.Static.SingleResult
{
    [TestFixture]
    [Category("Type Resolution")]
    public class ValueTypes
    {
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

        [Test]
        public void ValueTypeTest()
        {
            string expectedName = "Joe";
            DateTime expectedDoB = new DateTime(1980, 1, 1);
            CraeteSingleUser(expectedName, expectedDoB);
            string query = "SELECT [String] As UserName, [DateTime] As DoB FROM #DataTypeTable where [String] = @expectedName";
            var results = 
                DefineCommand(query)
                .WithResultsAs<User>()
                .Compile()
                .Execute(_databaseManager, new { expectedName })
                .ToList();
            
            Assert.AreEqual(1, results.Count());
            Assert.True(expectedName == results.First().UserName);
            Assert.True(expectedDoB == results.First().DoB);
        }

        private void CraeteSingleUser(string name, DateTime dob)
        {
            DefineCommand("INSERT INTO #DataTypeTable ([String], [DateTime]) VALUES(@String, @DateTime);")
            .Compile()
            .ExecuteNonQuery(_databaseManager, new { String = name, DateTime = dob });
        }
    }
}