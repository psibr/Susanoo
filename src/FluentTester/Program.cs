using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Susanoo;

namespace FluentTester
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandManager.RegisterDatabaseManager(new DatabaseManager(System.Data.SqlClient.SqlClientFactory.Instance, "test"));

            var command = CommandManager.DefineCommand("SELECT TOP 1 * FROM SimpleTable", System.Data.CommandType.Text)
                .DefineResultMappings<SimpleTable>()
                .ForResultSet((mapping) => {
                    mapping.ForProperty(result => result.Id, prop => prop.AliasProperty("id"));
                    mapping.ForProperty(result => result.Data, prop => prop.AliasProperty("data"));
                    mapping.ForProperty(result => result.Date, prop => prop.AliasProperty("date"));
                })                    
                .Finalize();

            var x = command.Execute().First();

            Console.WriteLine(x.Data);
            Console.ReadLine();
        }
    }
}
