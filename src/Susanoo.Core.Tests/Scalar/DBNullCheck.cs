#region

using System;
using System.Data;
using NUnit.Framework;

#endregion

namespace Susanoo.Tests.Scalar
{
    [Category("Scalar")]
    [TestFixture]
    public class ScalarTests
    {
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

        [TestFixtureSetUp]
        public void SetUp()
        {
        }

        [Test]
        public void NullableScalarAcceptsNull()
        {
            var result = CommandManager.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
                .Realize()
                .ExecuteScalar<int?>(_databaseManager);

            Assert.AreEqual(result, null);
        }

        [Test]
        [ExpectedException(typeof (NullReferenceException))]
        public void NonNullableScalarThrowsIfNull()
        {
            CommandManager.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
                .Realize()
                .ExecuteScalar<int>(_databaseManager);
        }
    }
}