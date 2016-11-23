using NUnit.Framework;
using System.Data;

namespace Susanoo.Tests.DbManager
{
    [Category("Connection")]
    [TestFixture]
    public class ConnectionTests
    {
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

        [Test]
        public void AffirmConnectionOpen()
        {
            Assert.AreEqual(_databaseManager.State, ConnectionState.Open);
        }
    }
}