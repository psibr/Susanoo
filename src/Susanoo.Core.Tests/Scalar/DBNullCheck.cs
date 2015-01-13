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
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

        [TestFixtureSetUp]
        public void SetUp()
        {
        }

        [Test]
        public void ScalarDbNullCheck()
        {
            var result = CommandManager.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
                .Realize("NullScalar")
                .ExecuteScalar<int?>(_databaseManager);

            Assert.AreEqual(result, null);
        }

        [Test]
        [ExpectedException(typeof (NullReferenceException))]
        public void NonNullScalarThrowsIfNull()
        {
            CommandManager.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
                .Realize("NullScalar")
                .ExecuteScalar<int>(_databaseManager);
        }
    }
}