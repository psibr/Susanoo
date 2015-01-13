#region

using System;
using System.Data;
using System.Linq;
using NUnit.Framework;

#endregion

namespace Susanoo.Tests.Dynamic
{
    [Category("Type Resolution")]
    [TestFixture]
    public class DynamicTypeTest
    {
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

        [Test(Description = "Tests that dynamic results correctly map data to CLR types.")]
        public void DynamicResultDataTypes()
        {
            var results = CommandManager.DefineCommand("SELECT * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<dynamic>()
                .Realize("DynamicDataTypeTest")
                .Execute(_databaseManager);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), 1);

            var first = results.First();

            Assert.IsTrue(first.Bit is bool);
            Assert.AreEqual(first.Bit, true);

            Assert.IsTrue(first.TinyInt is byte);
            Assert.AreEqual(first.TinyInt, (byte) 0x05);

            Assert.IsTrue(first.SmallInt is Int16);
            Assert.AreEqual(first.SmallInt, (Int16) 0x0004);

            Assert.IsTrue(first.Int is int);
            Assert.AreEqual(first.Int, 1);

            Assert.IsTrue(first.BigInt is Int64);
            Assert.AreEqual(first.BigInt, 2147483648);

            Assert.IsTrue(first.SmallMoney is decimal);
            Assert.AreEqual(first.SmallMoney, 10000.50m);

            Assert.IsTrue(first.SmallMoney is decimal);
            Assert.AreEqual(first.SmallMoney, 10000.50m);

            Assert.IsTrue(first.Money is decimal);
            Assert.AreEqual(first.Money, 1000000.50m);

            Assert.IsTrue(first.Numeric is decimal);
            Assert.AreEqual(first.Numeric, 1000000.50m);

            Assert.IsTrue(first.Decimal is decimal);
            Assert.AreEqual(first.Decimal, 1000000.50m);

            Assert.IsTrue(first.Character is string);
            Assert.AreEqual(first.Character, "c");

            Assert.IsTrue(first.String is string);
            Assert.AreEqual(first.String, "varchar");

            Assert.IsTrue(first.Text is string);
            Assert.AreEqual(first.Text, "text");

            Assert.IsTrue(first.Date is DateTime);
            Assert.AreEqual(first.Date, new DateTime(2014, 12, 25));

            Assert.IsTrue(first.SmallDateTime is DateTime);
            Assert.AreEqual(first.SmallDateTime, new DateTime(2014, 12, 25, 12, 00, 00));

            Assert.IsTrue(first.DateTime is DateTime);
            Assert.AreEqual(first.DateTime, new DateTime(2014, 12, 25, 12, 00, 00));

            Assert.IsTrue(first.DateTime2 is DateTime);
            Assert.AreEqual(first.DateTime2, new DateTime(2014, 12, 25, 12, 00, 00));

            Assert.IsTrue(first.Time is TimeSpan);
            Assert.AreEqual(first.Time, new TimeSpan(12, 00, 00));

            Assert.IsTrue(first.Guid is Guid);
            Assert.AreEqual(first.Guid, new Guid("E75B92A3-3299-4407-A913-C5CA196B3CAB"));
        }
    }
}