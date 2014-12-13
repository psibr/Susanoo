#region

using System;
using System.Data;
using NUnit.Framework;

#endregion

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
        [ExpectedException(typeof (NullReferenceException))]
        public void NonNullScalarThrowsIfNull()
        {
            CommandManager.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
                .Realize("NullScalar")
                .ExecuteScalar<int>(databaseManager);
        }
    }
}