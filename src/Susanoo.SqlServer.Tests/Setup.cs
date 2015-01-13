#region

using System.Data;
using NUnit.Framework;
using Susanoo;

#endregion

[SetUpFixture]
public class Setup
{
    public static readonly DatabaseManager DatabaseManager = new DatabaseManager("Susanoo");

    [SetUp]
    public void Configure()
    {
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