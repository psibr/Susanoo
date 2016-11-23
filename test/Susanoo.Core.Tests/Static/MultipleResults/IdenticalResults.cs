#region

using NUnit.Framework;
using System.Linq;
using static Susanoo.SusanooCommander;

#endregion

namespace Susanoo.Tests.Static.MultipleResults
{
    [Category("Multiple Results")]
    [TestFixture]
    public class IdenticalResults
    {
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

        [Test(Description = "Tests that results come through properly when the type is the same.")]
        public void IdenticalResults2Test()
        {

            var results = DefineCommand("SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;")
                .WithResultsAs(typeof(TypeTestModel), typeof(TypeTestModel))
                .ForResults<TypeTestModel>(result =>
                    result.MapPropertyToColumn(model => model.BigInt, "BigInt"))
                .Compile("IdenticalResults2Test")
                .Execute(_databaseManager);

            var arrResults = results.Select(e => e.Cast<TypeTestModel>()).ToArray();

            Assert.IsNotNull(arrResults[0]);
            Assert.IsNotNull(arrResults[1]);

            Assert.AreEqual(arrResults[0].Count(), 1);
            Assert.AreEqual(arrResults[1].Count(), 1);

            Assert.AreNotSame(arrResults[0], arrResults[1]);
        }

        [Test(Description = "Tests that attempting to get less results than available works fine.")]
        public void LessResultsThanAvailableTest()
        {

            var results = 
                DefineCommand(
                    "SELECT * FROM #DataTypeTable;" +
                    "SELECT * FROM #DataTypeTable;" +
                    "SELECT * FROM #DataTypeTable;" +
                    "SELECT * FROM #DataTypeTable;" +
                    "SELECT * FROM #DataTypeTable;" +
                    "SELECT * FROM #DataTypeTable;" +
                    "SELECT * FROM #DataTypeTable;")
                .WithResultsAs(typeof(TypeTestModel), typeof(TypeTestModel))
                .Compile("LessResultsAreAvailableTest")
                .Execute(_databaseManager).ToArray();

            Assert.IsNotNull(results[0]);
            Assert.IsNotNull(results[1]);

            Assert.AreEqual(results[0].Count(), 1);
            Assert.AreEqual(results[1].Count(), 1);

            Assert.AreNotSame(results[0], results[1]);
        }

        [Test(Description = "Tests that attempting to get more results than available provides null for the additional results.")]
        public void MoreResultsThanAvailableTest()
        {
            var results = 
                DefineCommand(
                    "SELECT * FROM #DataTypeTable;" +
                    "SELECT * FROM #DataTypeTable;")
                .WithResultsAs(typeof(TypeTestModel),
                    typeof(TypeTestModel),
                    typeof(TypeTestModel),
                    typeof(TypeTestModel),
                    typeof(TypeTestModel),
                    typeof(TypeTestModel),
                    typeof(TypeTestModel))
                .Compile("MoreResultsAreAvailableTest")
                .Execute(_databaseManager).ToArray();

            Assert.IsNotNull(results[0]);
            Assert.IsNotNull(results[1]);
            Assert.IsNull(results[2]);
            Assert.IsNull(results[3]);
            Assert.IsNull(results[4]);
            Assert.IsNull(results[5]);
            Assert.IsNull(results[6]);

            Assert.AreEqual(results[0].Count(), 1);
            Assert.AreEqual(results[0].Count(), 1);

            Assert.AreNotSame(results[0], results[1]);
        }
    }
}