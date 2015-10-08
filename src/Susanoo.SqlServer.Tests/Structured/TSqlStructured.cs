using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;

namespace Susanoo.SqlServer.Tests.Structured
{
	[Category("Structured DataType")]
	[TestFixture]
	public class TSqlStructured
	{
		private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

		[SetUp]
		public void DefineReferenceTypeIfNeeded()
		{
			CommandManager.Instance.DefineCommand(@"
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

			CommandManager.Instance.DefineCommand(@"
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

		[Test(Description = "Uses SqlDbType.Structured and TypeName to pass a set of data to a stored procedure.")]
		public void StructuredViaInclude()
		{
			var list = new List<KeyValuePair<int, string>>();

			for (var i = 2; i < 501; i++)
				list.Add(new KeyValuePair<int, string>(i, null));

			var results = CommandManager.Instance.DefineCommand("[dbo].[uspStructuredParameterTest]", CommandType.StoredProcedure)
				.IncludePropertyAsStructured("ReferenceTable", "ReferenceTable")
				.DefineResults<dynamic>()
				.Realize("StructuredViaInclude")
				.Execute(_databaseManager, new { ReferenceTable = list.Select(o => new { Id = o.Key }) });

			var enumerable = results as IList<dynamic> ?? results.ToList();
			Assert.AreEqual(1, enumerable.Count);
			Assert.AreEqual(2, enumerable.First().Id);
		}

		[Test(Description = "Uses SqlDbType.Structured and TypeName to pass a set of data to a stored procedure.")]
		public void StructuredViaIncludeNoRecords()
		{
			var list = new List<KeyValuePair<int, string>>();

			var results = CommandManager.Instance.DefineCommand("[dbo].[uspStructuredParameterTest]", CommandType.StoredProcedure)
				.IncludePropertyAsStructured("ReferenceTable", "ReferenceTable")
				.DefineResults<dynamic>()
				.Realize("StructuredViaInclude")
				.Execute(_databaseManager, new { ReferenceTable = list.Select(o => new { Id = o.Key }) });

			var enumerable = results as IList<dynamic> ?? results.ToList();
			Assert.AreEqual(false, enumerable.Any());
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void MakeTvpThrowsIfParamIsNull()
		{
			DbParameterExtensions.MakeTableValuedParameter(null, string.Empty, string.Empty);
		}


	}
}