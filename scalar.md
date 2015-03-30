#Scalar commands

Scalar[^msdn-scalar] commands return a single value. These most commonly source from `SELECT` statements with only one row or results of User-Defined Functions and may be wrapped in a Stored Procedure.

>A [complete example](#complete-example) is available at the bottom of this page.

##Prerequisite Steps

###Database Manager
The first step is to build a database manager.

```csharp
using(var databaseManager = new DatabaseManager("ConnectionString"))
{
}
```
>**Important:** Remember when building a new `DatabaseManager` to always use the `using` keyword so that resources are cleaned up when you are done with it.

##Usage

###Define Command

Next we need to tell Susanoo what SQL command to execute. We do this be build a `CommandExpression` using the `CommandManager.DefineCommand` method which takes 2 parameters, the first is the `CommandText` and the second is the `System.Data.CommandType` enum which controls how ADO.NET sends the command to your SQL environment (Text or StoredProcedure, Table is not supported in Susanoo). 

The command here will insert a new row into the `SomeTable` table and retrieve the primary key value using SQL Servers `@@Identity` macro.

```csharp
var insertCommand = 
	CommandManager
		.DefineCommand(
			  @"INSERT INTO dbo.SomeTable ( SomeColumn )
			  VALUES( 'New Value' );
			  SELECT @@Identity", CommandType.Text);
```

At this point we could also change how Susanoo treats the command in a few different ways, but for now we are just going to wrap the command up into something we can execute, what Susanoo calls a `CommandProcessor`.
We do this by calling `.Realize` on our `CommandExpression`.

```csharp
var insertCommand = 
	CommandManager
		.DefineCommand(
			  @"INSERT INTO dbo.SomeTable ( SomeColumn )
			  VALUES( 'New Value' );
			  SELECT @@Identity", CommandType.Text)
		.Realize();
```

We now have an executable ADO.NET command that we have saved to a variable named `insertCommand` and use elsewhere or use it immediately, the latter which is what we will do now.

###Execute
To execute a command we need to call one of the available Execute methods on our `CommandProcessor` we just built, for this example we will use `ExecuteScalar<TResult>`, where `TResult` is the simple type of the result.

 Which has at most the following parameters.

- **databaseManager** - An instance of the `DatabaseManager` class
- [Optional] **filter** - An anonymous OR strongly typed object with properties matching DbParameters needed for executing
- [Optional] **parameterObject** - An anonymous object with properties matching DbParameters needed for executing
- [Optional] **explicitParameters** - An array of DbParameters you want to include for the command. (Useful for output parameters.)

###Filter Parameters
Every execute method in Susanoo has a filter parameter. Most of the time this parameter is `dynamic`, but can be set to any `Type`. Each property of this object is gathered by Susanoo at execution and turned into `DbParameter` along with the value. The most common way to use this parameter is with anonymous objects. 

This feature is what we will use to pass parameters in this example.

##Complete Example
```csharp
using(var databaseManager =
	CommandManager.BuildDatabaseManager("ConnectionString"))
{
	var insertCommand = 
		CommandManager
			.DefineCommand(
				  @"INSERT INTO dbo.SomeTable ( SomeColumn )
				  VALUES( @SomeColumn );
				  SELECT @@Identity", CommandType.Text)
			.Realize();
		
    var newRowId = 
	    insertCommand 
		    .ExecuteScalar<int>(databaseManager, new 
		    {
			    SomeColumn = "New Value"
		    });
}
```
---
>**Warning:** Parameters with a value of null are NOT sent to the database by default in ADO.NET. See [SendNullValues](#sendnullvalues) to change this behavior or send `DBNull.Value` instead.

[^msdn-scalar]:   [MSDN: DbCommand.ExecuteScalar Method][msdn-execute-scalar]

 [msdn-execute-scalar]: https://msdn.microsoft.com/en-us/library/system.data.common.dbcommand.executescalar(v=vs.110).aspx

