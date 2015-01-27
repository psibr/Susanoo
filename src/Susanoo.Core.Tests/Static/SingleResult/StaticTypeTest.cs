#region

using System;
using System.Data;
using System.Linq;
using NUnit.Framework;
using Susanoo.Pipeline.Command.ResultSets.Processing;

#endregion

namespace Susanoo.Tests.Static.SingleResult
{
    [Category("Type Resolution")]
    [TestFixture]
    public class StaticTypeTest
    {
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;


        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void StaticResultDataTypes()
        {

            var results = CommandManager.DefineCommand("SELECT * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel>()
                .Realize("StaticDataTypeTest")
                .Execute(_databaseManager);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), 1);

            var first = results.First();

            Assert.AreEqual(first.Bit, true);

            Assert.AreEqual(first.TinyInt, 0x05);

            Assert.AreEqual(first.SmallInt, 0x0004);

            Assert.AreEqual(first.Int, 1);

            Assert.AreEqual(first.BigInt, 2147483648);

            Assert.AreEqual(first.SmallMoney, 10000.50m);

            Assert.AreEqual(first.Money, 1000000.50m);

            Assert.AreEqual(first.Numeric, 1000000.50m);

            Assert.AreEqual(first.Decimal, 1000000.50m);

            Assert.AreEqual(first.Character, "c");

            Assert.AreEqual(first.String, "varchar");

            Assert.AreEqual(first.Text, "text");

            Assert.AreEqual(first.Date, new DateTime(2014, 12, 25));

            Assert.AreEqual(first.SmallDateTime, new DateTime(2014, 12, 25, 12, 00, 00));

            Assert.AreEqual(first.DateTime, new DateTime(2014, 12, 25, 12, 00, 00));

            Assert.AreEqual(first.DateTime2, new DateTime(2014, 12, 25, 12, 00, 00));

            Assert.AreEqual(first.Time, new TimeSpan(12, 00, 00));

            Assert.AreEqual(first.Guid, new Guid("E75B92A3-3299-4407-A913-C5CA196B3CAB"));
        }
    }
}