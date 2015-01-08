using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Susanoo.Tests.DbManager
{
    [TestFixture]
    public class ConnectionTests
    {
        private readonly DatabaseManager _databaseManager = Setup.databaseManager;

        [Test]
        public void AffirmConnectionOpen()
        {
            Assert.AreEqual(_databaseManager.State, ConnectionState.Open);
        }
    }
}
