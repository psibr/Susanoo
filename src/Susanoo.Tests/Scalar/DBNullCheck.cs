using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Core;
using NUnit.Framework;

namespace Susanoo.Tests.Scalar
{
    [TestFixture]
    public class ScalarTests
    {
        private readonly DatabaseManager databaseManager = Setup.databaseManager;

        [TestFixtureSetUp]
        public void SetUp()
        {

        }

        [Test]
        public void ScalarDBNullCheck()
        {
            var result = CommandManager.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
                .Realize("NullScalar")
                .ExecuteScalar<int?>(databaseManager);

            Assert.AreEqual(result, null);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void NonNullScalarThrows()
        {
            CommandManager.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
                .Realize("NullScalar")
                .ExecuteScalar<int>(databaseManager);
        }

    }
}
