#region

using System;
using System.Data;
using NUnit.Framework;
using Susanoo.Exceptions;

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
            var result = CommandManager.Instance.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
                .Realize()
                .ExecuteScalar<int?>(_databaseManager);

            Assert.AreEqual(result, null);
        }

        [Test]
        public void NonNullableScalarThrowsIfNull()
        {
            try
            {
                CommandManager.Instance.DefineCommand("SELECT CAST(NULL AS INT)", CommandType.Text)
                    .Realize()
                    .ExecuteScalar<int>(_databaseManager);
            }
            catch (SusanooExecutionException ex)
            {
                if (ex.InnerException.GetType() != typeof (NullReferenceException))
                    throw;
            }

        }
    }
}