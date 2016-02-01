A simple, fast, fluently structured library that takes the pain out of writing ADO.NET by eliminating boiler plate code and providing SQL result mappings to strongly-typed objects with no attributing or baseclass/interface, just pure POCOs and fast! 

####Usage
```csharp
var command = CommandManager.Instance
    .DefineCommand(@"SELECT Id, FirstName, LastName 
                     FROM Customers
                     WHERE HasStoreCard = @HasStoreCard", CommandType.Text)
    .DefineResults<Customer>()
    .Realize();
    
using (var databaseManager =
    new DatabaseManager("DepartmentStoreConnectionString"))
{
    IEnumerable<Customer> customers =
        command.Execute(databaseManager, new { HasStoreCard = true });
}
```

####Build Status
#####Master
[![Build status](https://ci.appveyor.com/api/projects/status/8v0yyni93b4hmlo8/branch/master?svg=true)](https://ci.appveyor.com/project/psibernetic/susanoo/branch/master)

#####CI
[![Build status](https://ci.appveyor.com/api/projects/status/8v0yyni93b4hmlo8?svg=true)](https://ci.appveyor.com/project/psibernetic/susanoo)

#####Travis CI
[![Build Status](https://travis-ci.org/psibernetic/Susanoo.svg?branch=master)](https://travis-ci.org/psibernetic/Susanoo)

*Currently Structured parameters are not supported on Mono do to limitation with SqlDataRecord missing on Mono.*

####Nuget
[![NuGet version](https://badge.fury.io/nu/Susanoo.Core.svg)](http://badge.fury.io/nu/Susanoo.Core)

####Documentation
[Docs are available here.](https://susanoo.torchpad.com/)

####Project Updates
[Susanoo's Blog is here](http://blog.susanoo.net)

####How does it work?
Susanoo uses Linq expression trees to dynamically write and compile code to map your objects before the command ever executes, leaving your SQL calls unaffected. It does this all while avoiding slow reflection code and nasty reflection.emit IL.

####Installation
Susanoo does not require any configuration out of the box. The easiest way to install is of course via nuget package.

```
PM> Install-Package Susanoo.Core
```

#####Debugging Susanoo

Debugging symbols are hosted at [SymbolSource](http://www.symbolsource.org/MyGet/Metadata/susanoo/Project/Susanoo.Core).
A guide on adding the symbol server to Visual Studio is available [here](http://www.symbolsource.org/Public/Wiki/Using).

## License

Susanoo is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to license.txt for more information.
