using NUnit.Framework;
using Susanoo.Exceptions;
using System;
using System.Linq;
using static Susanoo.SusanooCommander;

namespace Susanoo.Tests.Static.SingleResult
{
    [TestFixture]
    [Category("Type Resolution")]
    public class BuiltInTypes
    {
        [Test]
        public void StringResultTest()
        {
            var results = 
                DefineCommand("SELECT * FROM (VALUES ('test')) AS MyValues(string)")
                .WithResultsAs<string>()
                .Compile()
                .Execute(Setup.DatabaseManager);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("test", results.First());
        }

        [Test]
        public void StringResultNullTest()
        {
            var results = DefineCommand("SELECT * FROM (VALUES (null)) AS MyValues(string)")
                .WithResultsAs<string>()
                .Compile()
                .Execute(Setup.DatabaseManager);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(null, results.First());
        }

        [Test]
        public void IntNullableTest()
        {
            var results = 
                DefineCommand("SELECT * FROM (VALUES (null), (5)) AS MyValues(int)")
                .WithResultsAs<int?>()
                .Compile()
                .Execute(Setup.DatabaseManager);

            Assert.AreEqual(2, results.Count());
            Assert.AreEqual(null, results.First());
            Assert.AreEqual(5, results.ElementAt(1));
        }

        [Test]
        public void IntNullTest()
        {
            try
            {
                DefineCommand("SELECT * FROM (VALUES (null)) AS MyValues(int)")
                .WithResultsAs<int>()
                .Compile()
                .Execute(Setup.DatabaseManager);
            }
            catch (AggregateException ex)
                when (ex.InnerExceptions.Count == 1
                      && ex.InnerExceptions.Any(iex =>
                          iex.GetType() == typeof (SusanooExecutionException)
                          && iex.InnerException.GetType() == typeof (InvalidCastException)))
            {
                //Valid exceptions 
            }
            catch (SusanooExecutionException ex)
                when (ex.InnerException.GetType() == typeof (InvalidCastException))
            {
                
            }
        }

        [Test]
        public void IntTest()
        {
            var results =
                DefineCommand("SELECT * FROM (VALUES (5)) AS MyValues(int)")
                .WithResultsAs<int>()
                .Compile()
                .Execute(Setup.DatabaseManager);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(5, results.First());
        }

    }
}
