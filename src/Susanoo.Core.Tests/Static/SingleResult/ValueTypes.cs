using NUnit.Framework;
using System;
using System.Data;
using System.Linq;

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
            string query = string.Format("SELECT [String] As UserName, [DateTime] As DoB FROM #DataTypeTable where [String] = '{0}'", expectedName);
            var results = CommandManager.Instance.DefineCommand(query, CommandType.Text)
                .DefineResults<User>()
                .Realize()
                .Execute(_databaseManager)
                .ToList();
            
            Assert.AreEqual(1, results.Count());
            Assert.True(expectedName == results.First().UserName);
            Assert.True(expectedDoB == results.First().DoB);
        }

        private void CraeteSingleUser(string name, DateTime dob)
        {
            CommandManager.Instance.DefineCommand<dynamic>("INSERT INTO #DataTypeTable ([String], [DateTime]) VALUES(@String, @DateTime);", CommandType.Text)
                .Realize()
                .ExecuteNonQuery(_databaseManager, new { String = name, DateTime = dob });
        }
    }
}