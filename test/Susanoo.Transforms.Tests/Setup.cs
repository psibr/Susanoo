#region

using NUnit.Framework;
using Susanoo.Deserialization;
using Susanoo.Processing;
using Susanoo.ResultSets;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

#endregion

namespace Susanoo.Transforms.Tests
{
    public class InterceptedSusanooBootstrapper : SusanooBootstrapper
    {
        public InterceptedSusanooBootstrapper()
        {
            Container.Register<ISingleResultSetCommandProcessorFactory>((container) 
                => new InterceptedSingleResultSetFactory(container.Resolve<IDeserializerResolver>()));
        }
    }

    public class InterceptedSingleResultSetFactory
        : SingleResultSetCommandProcessorFactory
    {
        public InterceptedSingleResultSetFactory(IDeserializerResolver deserializerResolver)
            : base(deserializerResolver)
        {
            
        }

        /// <summary>
        /// Builds the command processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public override ISingleResultSetCommandProcessor<TFilter, TResult> BuildCommandProcessor<TFilter, TResult>(ICommandResultInfo<TFilter> mappings)
            => base.BuildCommandProcessor<TFilter, TResult>(mappings)
                .InterceptExceptions(ex => Debug.Write($"{ex.Info.CommandText}"));
    }

    [SetUpFixture]
    public class Setup
    {
        public static readonly DatabaseManager DatabaseManager =
            DatabaseManager.CreateFromConnectionString(
                SqlClientFactory.Instance,
                "Data Source=(LocalDB)\\MSSQLLocalDb;Initial Catalog=tempdb;Integrated Security=True;MultipleActiveResultSets=True");

        public Setup()
        {
            Configure();
        }

        public void Configure()
        {
            SusanooCommander.Instance.Bootstrap(new InterceptedSusanooBootstrapper());

            //By explicitly opening the connection, it becomes a shared connection.
            DatabaseManager.OpenConnection();

            BuildDataTypeTable();
        }

        /// <summary>
        ///     Builds the data type table used for testing conversions.
        /// </summary>
        private static void BuildDataTypeTable()
        {
            SusanooCommander.DefineCommand(
                @"
                IF OBJECT_ID('tempdb..#DataTypeTable') IS NOT NULL 
                BEGIN
                    DROP TABLE #DataTypeTable;
                END

                SELECT 
                    Bit = CAST(1 AS BIT),
                    TinyInt = CAST(5 AS TINYINT),
                    SmallInt = CAST(4 AS SmallInt),
                    Int = CAST(1 AS INT),
                    BigInt = CAST(2147483648 AS BIGINT),
                    SmallMoney = CAST($10000.50 AS SMALLMONEY),
                    Money = CAST($1000000.50 AS MONEY),
                    Numeric = CAST(1000000.50 AS NUMERIC(10, 2)),
                    Decimal = CAST(1000000.50 AS DECIMAL(10, 2)),
                    Character = CAST('c' AS CHAR(1)),
                    String = CAST('varchar' AS VARCHAR(7)), 
                    Text = CAST('text' AS Text),
                    Date = CAST('12/25/2014' AS Date),
                    SmallDateTime = CAST('12/25/2014 12:00:00' AS SmallDateTime),
                    DateTime = CAST('12/25/2014 12:00:00' AS DateTime),
                    DateTime2 = CAST('12/25/2014 12:00:00' AS DateTime2(7)),
                    Time = CAST('12:00:00' AS Time),
                    Guid = CAST('E75B92A3-3299-4407-A913-C5CA196B3CAB' AS uniqueidentifier),
                    IgnoredByComponentModel = CAST('ignored' AS VARCHAR(7)),
                    IgnoredByDescriptorActionsNone = CAST('ignored' AS VARCHAR(7)),
                    IgnoredByDescriptorActionsUpdate = CAST('ignored' AS VARCHAR(7))
                INTO #DataTypeTable;",
                CommandType.Text)
                .Compile()
                .ExecuteNonQuery(DatabaseManager);
        }
    }
}