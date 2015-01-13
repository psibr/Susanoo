#region

using System.Data;
using NUnit.Framework;
using Susanoo;

#endregion

[SetUpFixture]
public class Setup
{
    public static readonly DatabaseManager databaseManager = new DatabaseManager("Susanoo");

    [SetUp]
    public void Configure()
    {
        //By explicitly opening the connection, it becomes a shared connection.
        databaseManager.OpenConnection();
    }

    [TearDown]
    public void Close()
    {
        databaseManager.CloseConnection();
        databaseManager.Dispose();
    }
}