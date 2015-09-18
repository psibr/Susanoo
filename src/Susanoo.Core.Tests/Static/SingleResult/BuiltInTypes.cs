using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Susanoo.Exceptions;
using Susanoo.Processing;

namespace Susanoo.Tests.Static.SingleResult
{
    [TestFixture]
    [Category("Type Resolution")]
    public class BuiltInTypes
    {
        [Test]
        public void StringResultTest()
        {
            var results = CommandManager.Instance.DefineCommand("SELECT * FROM (VALUES ('test')) AS MyValues(string)", CommandType.Text)
                .DefineResults<string>()
                .Realize()
                .Execute(Setup.DatabaseManager);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("test", results.First());
        }

        [Test]
        public void StringResultNullTest()
        {
            var results = CommandManager.Instance.DefineCommand("SELECT * FROM (VALUES (null)) AS MyValues(string)", CommandType.Text)
                .DefineResults<string>()
                .Realize()
                .Execute(Setup.DatabaseManager);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(null, results.First());
        }

        [Test]
        public void IntNullableTest()
        {
            var results = CommandManager.Instance.DefineCommand("SELECT * FROM (VALUES (null), (5)) AS MyValues(int)", CommandType.Text)
                .DefineResults<int?>()
                .Realize()
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
                CommandManager.Instance.DefineCommand("SELECT * FROM (VALUES (null)) AS MyValues(int)", CommandType.Text)
                    .DefineResults<int>()
                    .Realize()
                    .Execute(Setup.DatabaseManager);
            }
            catch (AggregateException ex)
                when (ex.InnerExceptions.Count == 1
                    && ex.InnerExceptions.Any(iex =>
                       iex.GetType() == typeof(SusanooExecutionException)
                            && iex.InnerException.GetType() == typeof(InvalidCastException)))
            {
                //Valid exceptions 
            }
        }

        [Test]
        public void IntTest()
        {
            var results = CommandManager.Instance.DefineCommand("SELECT * FROM (VALUES (5)) AS MyValues(int)", CommandType.Text)
                .DefineResults<int>()
                .Realize()
                .Execute(Setup.DatabaseManager);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(5, results.First());
        }

    }
}
