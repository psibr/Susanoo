#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;

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

            var results = CommandManager.Instance.DefineCommand("SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel, TypeTestModel>()
                .Realize("IdenticalResults2Test")
                .Execute(_databaseManager);

            Assert.AreEqual(results.GetType(), typeof(Tuple<IEnumerable<TypeTestModel>, IEnumerable<TypeTestModel>>));

            Assert.IsNotNull(results.Item1);
            Assert.IsNotNull(results.Item2);

            Assert.AreEqual(results.Item1.Count(), 1);
            Assert.AreEqual(results.Item2.Count(), 1);

            Assert.AreNotSame(results.Item1, results.Item2);
        }

        [Test(Description = "Tests that results come through properly when the type is the same.")]
        public void IdenticalResults3Test()
        {

            var results = CommandManager.Instance.DefineCommand("SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel, TypeTestModel, TypeTestModel>()
                .Realize("IdenticalResults3Test")
                .Execute(_databaseManager);

            Assert.AreEqual(results.GetType(), typeof(Tuple<
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>>));

            Assert.IsNotNull(results.Item1);
            Assert.IsNotNull(results.Item2);
            Assert.IsNotNull(results.Item3);

            Assert.AreEqual(results.Item1.Count(), 1);
            Assert.AreEqual(results.Item2.Count(), 1);
            Assert.AreEqual(results.Item3.Count(), 1);

            Assert.AreNotSame(results.Item1, results.Item2);
            Assert.AreNotSame(results.Item2, results.Item3);

        }

        [Test(Description = "Tests that results come through properly when the type is the same.")]
        public void IdenticalResults4Test()
        {

            var results = CommandManager.Instance.DefineCommand("SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel>()
                .Realize("IdenticalResults4Test")
                .Execute(_databaseManager);

            Assert.AreEqual(results.GetType(), typeof(Tuple<
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>>));

            Assert.IsNotNull(results.Item1);
            Assert.IsNotNull(results.Item2);
            Assert.IsNotNull(results.Item3);
            Assert.IsNotNull(results.Item4);

            Assert.AreEqual(results.Item1.Count(), 1);
            Assert.AreEqual(results.Item2.Count(), 1);
            Assert.AreEqual(results.Item3.Count(), 1);
            Assert.AreEqual(results.Item4.Count(), 1);

            Assert.AreNotSame(results.Item1, results.Item2);
            Assert.AreNotSame(results.Item2, results.Item3);
            Assert.AreNotSame(results.Item3, results.Item4);
        }

        [Test(Description = "Tests that results come through properly when the type is the same.")]
        public void IdenticalResults5Test()
        {

            var results = CommandManager.Instance.DefineCommand("SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel>
                ()
                .Realize("IdenticalResults5Test")
                .Execute(_databaseManager);

            Assert.AreEqual(results.GetType(), typeof(Tuple<
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>>));

            Assert.IsNotNull(results.Item1);
            Assert.IsNotNull(results.Item2);
            Assert.IsNotNull(results.Item3);
            Assert.IsNotNull(results.Item4);
            Assert.IsNotNull(results.Item5);

            Assert.AreEqual(results.Item1.Count(), 1);
            Assert.AreEqual(results.Item2.Count(), 1);
            Assert.AreEqual(results.Item3.Count(), 1);
            Assert.AreEqual(results.Item4.Count(), 1);
            Assert.AreEqual(results.Item5.Count(), 1);

            Assert.AreNotSame(results.Item1, results.Item2);
            Assert.AreNotSame(results.Item2, results.Item3);
            Assert.AreNotSame(results.Item3, results.Item4);
            Assert.AreNotSame(results.Item4, results.Item5);
        }

        [Test(Description = "Tests that results come through properly when the type is the same.")]
        public void IdenticalResults6Test()
        {

            var results = CommandManager.Instance.DefineCommand("SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel>
                ()
                .Realize("IdenticalResults6Test")
                .Execute(_databaseManager);

            Assert.AreEqual(results.GetType(), typeof(Tuple<
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>>));

            Assert.IsNotNull(results.Item1);
            Assert.IsNotNull(results.Item2);
            Assert.IsNotNull(results.Item3);
            Assert.IsNotNull(results.Item4);
            Assert.IsNotNull(results.Item5);
            Assert.IsNotNull(results.Item6);

            Assert.AreEqual(results.Item1.Count(), 1);
            Assert.AreEqual(results.Item2.Count(), 1);
            Assert.AreEqual(results.Item3.Count(), 1);
            Assert.AreEqual(results.Item4.Count(), 1);
            Assert.AreEqual(results.Item5.Count(), 1);
            Assert.AreEqual(results.Item6.Count(), 1);

            Assert.AreNotSame(results.Item1, results.Item2);
            Assert.AreNotSame(results.Item2, results.Item3);
            Assert.AreNotSame(results.Item3, results.Item4);
            Assert.AreNotSame(results.Item4, results.Item5);
            Assert.AreNotSame(results.Item5, results.Item6);
        }

        [Test(Description = "Tests that results come through properly when the type is the same.")]
        public void IdenticalResults7Test()
        {

            var results = CommandManager.Instance.DefineCommand("SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel>()
                .Realize("IdenticalResults7Test")
                .Execute(_databaseManager);

            Assert.AreEqual(results.GetType(), typeof(Tuple<
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>>));

            Assert.IsNotNull(results.Item1);
            Assert.IsNotNull(results.Item2);
            Assert.IsNotNull(results.Item3);
            Assert.IsNotNull(results.Item4);
            Assert.IsNotNull(results.Item5);
            Assert.IsNotNull(results.Item6);
            Assert.IsNotNull(results.Item7);

            Assert.AreEqual(results.Item1.Count(), 1);
            Assert.AreEqual(results.Item2.Count(), 1);
            Assert.AreEqual(results.Item3.Count(), 1);
            Assert.AreEqual(results.Item4.Count(), 1);
            Assert.AreEqual(results.Item5.Count(), 1);
            Assert.AreEqual(results.Item6.Count(), 1);
            Assert.AreEqual(results.Item7.Count(), 1);

            Assert.AreNotSame(results.Item1, results.Item2);
            Assert.AreNotSame(results.Item2, results.Item3);
            Assert.AreNotSame(results.Item3, results.Item4);
            Assert.AreNotSame(results.Item4, results.Item5);
            Assert.AreNotSame(results.Item5, results.Item6);
            Assert.AreNotSame(results.Item6, results.Item7);
        }

        [Test(Description = "Tests that attempting to get less results than available works fine.")]
        public void LessResultsThanAvailableTest()
        {

            var results = CommandManager.Instance.DefineCommand("SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel, TypeTestModel>()
                .Realize("LessResultsAreAvailableTest")
                .Execute(_databaseManager);

            Assert.AreEqual(results.GetType(), typeof(Tuple<
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>>));

            Assert.IsNotNull(results.Item1);
            Assert.IsNotNull(results.Item2);

            Assert.AreEqual(results.Item1.Count(), 1);
            Assert.AreEqual(results.Item2.Count(), 1);

            Assert.AreNotSame(results.Item1, results.Item2);
        }

        [Test(Description = "Tests that attempting to get more results than available provides null for the additional results.")]
        public void MoreResultsThanAvailableTest()
        {

            var results = CommandManager.Instance.DefineCommand("SELECT * FROM #DataTypeTable;" +
                                                       "SELECT * FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel, TypeTestModel>()
                .Realize("MoreResultsAreAvailableTest")
                .Execute(_databaseManager);

            Assert.AreEqual(results.GetType(), typeof(Tuple<
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>,
                IEnumerable<TypeTestModel>>));

            Assert.IsNotNull(results.Item1);
            Assert.IsNotNull(results.Item2);
            Assert.IsNull(results.Item3);
            Assert.IsNull(results.Item4);
            Assert.IsNull(results.Item5);
            Assert.IsNull(results.Item6);
            Assert.IsNull(results.Item7);

            Assert.AreEqual(results.Item1.Count(), 1);
            Assert.AreEqual(results.Item2.Count(), 1);

            Assert.AreNotSame(results.Item1, results.Item2);
        }
    }
}