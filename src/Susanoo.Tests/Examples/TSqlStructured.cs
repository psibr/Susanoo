using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;

namespace Susanoo.Tests.Examples
{
    [TestFixture]
    public class TSqlStructured
    {
        private readonly DatabaseManager _databaseManager = Setup.databaseManager;

        [SetUp]
        public void DefineReferenceTypeIfNeeded()
        {
            CommandManager.DefineCommand(@"
                IF not EXISTS (SELECT * FROM sys.types WHERE is_user_defined = 1 AND name = 'ReferenceTable')
	                CREATE TYPE [dbo].[ReferenceTable] AS TABLE(
		                [ReferenceId] [int] NOT NULL
	                )

                IF EXISTS ( SELECT  *
                            FROM    sys.objects
                            WHERE   object_id = OBJECT_ID(N'uspStructuredParameterTest')
                                AND type IN ( N'P', N'PC' ))
                BEGIN

	                DROP PROC [dbo].[uspStructuredParameterTest]
                END"
                , CommandType.Text)
                .Realize()
                .ExecuteNonQuery(_databaseManager);

            CommandManager.DefineCommand(@"
	            CREATE PROCEDURE [dbo].[uspStructuredParameterTest]
		            @ReferenceTable dbo.ReferenceTable READONLY
	            AS
		            BEGIN
			            SELECT Id 
			            FROM (SELECT 1 AS Id UNION SELECT 2 AS Id) a
			            INNER JOIN @ReferenceTable r ON r.ReferenceId = a.id
		            END
                ", CommandType.Text)
                .Realize()
                .ExecuteNonQuery(_databaseManager);
        }

        [Test]
        public void StructuredSqlDbType()
        {
            using (var referenceTable = new DataTable("ReferenceTable"))
            {
                referenceTable.Columns.Add("Id", typeof (Int32));
                referenceTable.Rows.Add(2);

                var referenceTableParam = (SqlParameter)_databaseManager.CreateParameter();

                referenceTableParam.SqlDbType = SqlDbType.Structured;
                referenceTableParam.TypeName = "ReferenceTable";
                referenceTableParam.ParameterName = "@ReferenceTable";
                referenceTableParam.Value = referenceTable;

                var results = CommandManager.DefineCommand("[dbo].[uspStructuredParameterTest]", CommandType.StoredProcedure)
                    .DefineResults<dynamic>()
                    .Realize("StructuredParameterTest")
                    .Execute(_databaseManager, referenceTableParam);

                var enumerable = results as IList<dynamic> ?? results.ToList();
                Assert.AreEqual(enumerable.Count(), 1);
                Assert.AreEqual(enumerable.First().Id, 2);
            }
        }
    }
}
