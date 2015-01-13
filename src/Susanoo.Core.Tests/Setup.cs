#region

using System.Data;
using NUnit.Framework;

#endregion

namespace Susanoo.Tests
{
    [SetUpFixture]
    public class Setup
    {
        public static readonly DatabaseManager DatabaseManager = new DatabaseManager("Susanoo");

        [SetUp]
        public void Configure()
        {
            //By explicitly opening the connection, it becomes a shared connection.
            DatabaseManager.OpenConnection();

            BuildDataTypeTable();
        }

        /// <summary>
        ///     Builds the data type table used for testing conversions.
        /// </summary>
        private static void BuildDataTypeTable()
        {
            CommandManager.DefineCommand(
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
                    Guid = CAST('E75B92A3-3299-4407-A913-C5CA196B3CAB' AS uniqueidentifier)

                INTO #DataTypeTable;",
                CommandType.Text)
                .Realize("DataTypeTableBuilder")
                .ExecuteNonQuery(DatabaseManager);
        }

        [TearDown]
        public void Close()
        {
            DatabaseManager.CloseConnection();
            DatabaseManager.Dispose();
        }
    }
}