Susano'o
====
A simple, fast, fluently structured library that takes the pain out of writing ADO.NET by eliminating boiler plate code and providing SQL result mappings to strongly-typed objects with no attributing or baseclass/interface, just pure POCOs and fast! 

#####Usage
```csharp
var command = CommandManager
    .DefineCommand(@"SELECT Id, FirstName, LastName 
                     FROM Customers
                     WHERE HasStoreCard = @HasStoreCard", CommandType.Text)
    .DefineResults<Customer>()
    .Finalize();
```
```csharp
using (var databaseManager =
    new DatabaseManager("DepartmentStoreConnectionString"))
{
    IEnumerable<Customer> customers =
        command.Execute(databaseManager, new { HasStoreCard = true });
}
```

#####How does it work?
Susanoo uses Linq expression trees to dynamically write and compile code to map your objects before the command ever executes, leaving your SQL calls unaffected.

#####Installation
Susanoo does not require any configuration out of the box. The easiest way to install is of course via nuget package.

```
PM> Install-Package Susanoo.Core
```

#####Debugging Susanoo

Debugging symbols are hosted at [SymbolSource](http://www.symbolsource.org/MyGet/Metadata/susanoo/Project/Susanoo.Core).
A guide on adding the symbol server to Visual Studio is available [here](http://www.symbolsource.org/Public/Wiki/Using).
