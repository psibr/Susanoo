using NUnit.Framework;
using Susanoo.Command;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Susanoo.SusanooCommander;

namespace Susanoo.Transforms.SqlServer.Tests
{
    [Category("Structured DataType")]
    [TestFixture]
    public class FetchOffset
    {
        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void KeyValueWithOffsetFetch()
        {
            var results =
                DefineCommand<KeyValuePair<string, string>>(
                    @"SELECT Int, String
                    FROM (VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four')) AS SampleSet(Int, String)
                    ORDER BY Int")
                .ExcludeProperty(o => o.Key)
                .ExcludeProperty(o => o.Value)
                .SendNullValues(NullValueMode.FilterOnlyFull)
                .WithResultsAs<KeyValuePair<string, string>>(result =>
                {
                    result.MapPropertyToColumn(pair => pair.Key, "Int");
                    result.MapPropertyToColumn(pair => pair.Value, "String");
                })
                .Compile()
                .WithTransforms(source => new[]
                {
                    SqlServerTransforms.OffsetFetch(source)
                })
                .Execute(Setup.DatabaseManager, default(KeyValuePair<string, string>), new { RowCount = 2, PageNumber = 2 });

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count());

            var first = results.First();
            var last = results.Last();

            Assert.AreEqual("3", first.Key);
            Assert.AreEqual("Three", first.Value);

            Assert.AreEqual("4", last.Key);
            Assert.AreEqual("Four", last.Value);
        }

        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void KeyValueWithWhereFilterAndOffsetFetch()
        {
            var results = 
                DefineCommand<KeyValuePair<string, string>>(
                    @"SELECT Int, String FROM ( VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four')) AS SampleSet(Int, String)")
                .IncludeProperty(o => o.Key, parameter => parameter.ParameterName = "Int")
                .IncludeProperty(o => o.Value, parameter => parameter.ParameterName = "String")
                .SendNullValues(NullValueMode.FilterOnlyFull)
                .WithResultsAs<KeyValuePair<string, string>>(result =>
                {
                    result.MapPropertyToColumn(pair => pair.Key, "Int");
                    result.MapPropertyToColumn(pair => pair.Value, "String");
                })
                .Compile()
                .WithTransforms(source => new[]
                {
                    SqlServerTransforms.QueryWrapperWithTotalRowCount(source),
                    Transforms.WhereFilter(source, typeof(KeyValuePair<string, string>)),
                    Transforms.OrderByExpression(),
                    SqlServerTransforms.OffsetFetch(source)
                })
                .Execute(Setup.DatabaseManager,
                    new KeyValuePair<string, string>(null, "o"),
                    new { OrderBy = "Int DESC", RowCount = 2, PageNumber = 2 });

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());

            var first = results.First();

            Assert.AreEqual("1", first.Key);
            Assert.AreEqual("One", first.Value);
        }

        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void OffsetFetchAndTotalRowCount()
        {
            var results = 
                DefineCommand<dynamic>(
                    @"SELECT Int, String
                    FROM (VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four')) AS SampleSet(Int, String)")
                .SendNullValues(NullValueMode.FilterOnlyFull)
                .WithResultsAs<dynamic>()
                .Compile()
                .WithTransforms(source => new[]
                {
                    SqlServerTransforms.QueryWrapperWithTotalRowCount(source),
                    Transforms.WhereFilter(source, typeof(object)),
                    Transforms.OrderByExpression(),
                    SqlServerTransforms.OffsetFetch(source)
                })
            .Execute(Setup.DatabaseManager, null, new { RowCount = 2, PageNumber = 2, OrderBy = "Int" });

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count());

            var first = results.First();
            var last = results.Last();

            Assert.AreEqual(4, first.TotalRows);

            Assert.AreEqual("3", first.Int);
            Assert.AreEqual("Three", first.String);

            Assert.AreEqual("4", last.Int);
            Assert.AreEqual("Four", last.String);
        }
    }
}