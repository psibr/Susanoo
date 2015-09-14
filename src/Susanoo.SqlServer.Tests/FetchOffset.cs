using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Susanoo.SqlServer.Tests
{
    [Category("Structured DataType")]
    [TestFixture]
    public class FetchOffset
    {
        //[Test(Description = "Tests that results correctly map data to CLR types.")]
        //public void KeyValueWithOffsetFetch()
        //{
        //    var results = CommandManager.Instance.DefineCommand<KeyValuePair<string, string>>(@"
        //        SELECT Int, String
        //        FROM (VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four')) AS SampleSet(Int, String)
        //        ORDER BY Int", CommandType.Text)
        //        .ExcludeProperty(o => o.Key)
        //        .ExcludeProperty(o => o.Value)
        //        .SendNullValues(NullValueMode.FilterOnlyFull)
        //        .DefineResults<KeyValuePair<string, string>>()
        //        .ForResults(expression =>
        //        {
        //            expression.ForProperty(pair => pair.Key,
        //                configuration => configuration.UseAlias("Int"));
        //            expression.ForProperty(pair => pair.Value,
        //                configuration => configuration.UseAlias("String"));
        //        })
        //        .OffsetFetch()
        //        .Realize()
        //        .Execute(Setup.DatabaseManager, default(KeyValuePair<string, string>), new { RowCount = 2, PageNumber = 2 });

        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(2, results.Count());

        //    var first = results.First();
        //    var last = results.Last();

        //    Assert.AreEqual("3", first.Key);
        //    Assert.AreEqual("Three", first.Value);

        //    Assert.AreEqual("4", last.Key);
        //    Assert.AreEqual("Four", last.Value);
        //}

        //[Test(Description = "Tests that results correctly map data to CLR types.")]
        //public void KeyValueWithWhereFilterAndOffsetFetch()
        //{
        //    var results = CommandManager.Instance.DefineCommand<KeyValuePair<string, string>>(
        //        @"SELECT Int, String FROM ( VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four')) AS SampleSet(Int, String)", CommandType.Text)
        //        .IncludeProperty(o => o.Key, parameter => parameter.ParameterName = "Int")
        //        .IncludeProperty(o => o.Value, parameter => parameter.ParameterName = "String")
        //        .SendNullValues(NullValueMode.FilterOnlyFull)
        //        .DefineResults<KeyValuePair<string, string>>()
        //        .ForResults(expression =>
        //        {
        //            expression.ForProperty(pair => pair.Key,
        //                configuration => configuration.UseAlias("Int"));
        //            expression.ForProperty(pair => pair.Value,
        //                configuration => configuration.UseAlias("String"));
        //        })
        //        .BuildWhereFilter()
        //        .AddOrderByExpression()
        //        .OffsetFetch()
        //        .Realize()
        //        .Execute(Setup.DatabaseManager,
        //            new KeyValuePair<string, string>(null, "o"),
        //            new { OrderBy = "Int DESC", RowCount = 2, PageNumber = 2 });

        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(1, results.Count());

        //    var first = results.First();

        //    Assert.AreEqual("1", first.Key);
        //    Assert.AreEqual("One", first.Value);
        //}

        //[Test(Description = "Tests that results correctly map data to CLR types.")]
        //public void OffsetFetchAndTotalRowCount()
        //{
        //    var results = CommandManager.Instance.DefineCommand<dynamic>(@"
        //        SELECT Int, String
        //        FROM (VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four')) AS SampleSet(Int, String)", CommandType.Text)
        //        .SendNullValues(NullValueMode.FilterOnlyFull)
        //        .DefineResults<dynamic>()
        //        .AddOrderByExpression()
        //        .OffsetFetch()
        //        .AddTotalRowCount()
        //        .Realize()
        //        .Execute(Setup.DatabaseManager, null, new { RowCount = 2, PageNumber = 2, OrderBy = "Int" });

        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(2, results.Count());

        //    var first = results.First();
        //    var last = results.Last();

        //    Assert.AreEqual(4, first.TotalRows);

        //    Assert.AreEqual("3", first.Int);
        //    Assert.AreEqual("Three", first.String);

        //    Assert.AreEqual("4", last.Int);
        //    Assert.AreEqual("Four", last.String);
        //}
    }
}