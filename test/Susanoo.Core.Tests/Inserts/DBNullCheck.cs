#region

using NUnit.Framework;
using Susanoo.Exceptions;
using System;
using static Susanoo.SusanooCommander;

#endregion

namespace Susanoo.Tests.Scalar
{
    [Category("Scalar")]
    [TestFixture]
    public class ScalarTests
    {
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

        [OneTimeSetUp]
        public void SetUp()
        {
        }

        [Test]
        public void NullableScalarAcceptsNull()
        {
            var result = 
                DefineCommand("SELECT CAST(NULL AS INT)")
                .Compile()
                .ExecuteScalar<int?>(_databaseManager);

            Assert.AreEqual(result, null);
        }

        [Test]
        public void NonNullableScalarThrowsIfNull()
        {
            try
            {
                DefineCommand("SELECT CAST(NULL AS INT)")
                .Compile()
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