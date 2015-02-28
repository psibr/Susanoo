#region

using NUnit.Framework;

#endregion


namespace Susanoo.SqlServer.Tests
{
    [SetUpFixture]
    public class Setup
    {
        public static DatabaseManager DatabaseManager;
    
        [SetUp]
        public void Configure()
        {
            CommandManager.RegisterBootstrapper(new TestBootstrapper());

            DatabaseManager = new DatabaseManager("Susanoo");

            //By explicitly opening the connection, it becomes a shared connection.
            DatabaseManager.OpenConnection();
        }

        [TearDown]
        public void Close()
        {
            DatabaseManager.CloseConnection();
            DatabaseManager.Dispose();
        }
    }
}