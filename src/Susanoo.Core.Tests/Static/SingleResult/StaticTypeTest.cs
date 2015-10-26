#region

using NUnit.Framework;
using System;
using System.Data;
using System.Linq;

#endregion

namespace Susanoo.Tests.Static.SingleResult
{
    [Category("Type Resolution")]
    [TestFixture]
    public class StaticTypeTest
    {
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

        [Test]
        public void StaticRowPerformance()
        {
            for (int i = 0; i < 500; i++)
            {
                CommandManager.Instance
                    .DefineCommand("SELECT TOP 1 * FROM #DataTypeTable;", CommandType.Text)
                    .DefineResults<TypeTestModel>()
                    .Realize()
                    .Execute(_databaseManager);
            }
        }

        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void StaticResultDataTypes()
        {
            var results = CommandManager.Instance.DefineCommand("SELECT TOP 1 * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel>()
                .Realize()
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

            Assert.IsNull(first.IgnoredByComponentModel);

            Assert.IsNull(first.IgnoredByDescriptorActionsNone);

            Assert.IsNull(first.IgnoredByDescriptorActionsUpdate);
        }

        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void StaticResultDataTypesInsert()
        {
            var result = CommandManager.Instance.DefineCommand("SELECT TOP 1 * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel>()
                .Realize()
                .Execute(_databaseManager).First();

            result.IgnoredByDescriptorActionsUpdate = "ignored";

            CommandManager.Instance.DefineCommand<TypeTestModel>("INSERT INTO #DataTypeTable VALUES(@Bit, @TinyInt, @SmallInt, @Int, @BigInt, @SmallMoney, @Money, @Numeric, @Decimal, @Character, @String, @Text, @Date, @SmallDateTime, @DateTime, @DateTime2, '12:00:00:00', @Guid, 'ignored','ignored', @IgnoredByDescriptorActionsUpdate);", CommandType.Text)
                .Realize()
                .ExecuteNonQuery(_databaseManager, result);
        }



        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void StaticResultDataTypesInsertAnonymous()
        {
            CommandManager.Instance.DefineCommand<dynamic>("INSERT INTO #DataTypeTable VALUES(@Bit, @TinyInt, @SmallInt, @Int, @BigInt, @SmallMoney, @Money, @Numeric, @Decimal, @Character, @String, @Text, @Date, @SmallDateTime, @DateTime, @DateTime2, '12:00:00:00', @Guid, 'ignored','ignored', @IgnoredByDescriptorActionsUpdate);", CommandType.Text)
                .Realize()
                .ExecuteNonQuery(_databaseManager, new { Bit = true, TinyInt = 1, SmallInt = 1, Int = 1, BigInt = 1, SmallMoney = 1, Money = 1, Numeric = 1, Decimal = 1, Character = 'c', String = "string", Text = "yay", Date = DateTime.Now, SmallDateTime = DateTime.Now, DateTime = DateTime.Now, DateTime2 = DateTime.Now, Guid = Guid.NewGuid(), IgnoredByDescriptorActionsUpdate = "ignored" });
        }
    }
}