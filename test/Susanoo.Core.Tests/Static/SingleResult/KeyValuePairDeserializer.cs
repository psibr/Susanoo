#region

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Susanoo;
using static Susanoo.SusanooCommander;

#endregion

namespace Susanoo.Tests.Static.SingleResult
{
    [Category("Type Resolution")]
    [TestFixture]
    public class KeyValuePairTests
    {
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void KeyValuePairMap()
        {
            var results = 
                DefineCommand("SELECT Int, String FROM #DataTypeTable;")
                .WithResultsAs<KeyValuePair<int, string>>()
                .ForResults(whenMapping =>
                {
                    whenMapping.MapPropertyToColumn(pair => pair.Key, "Int");
                    whenMapping.MapPropertyToColumn(pair => pair.Value, "String");
                })
                .Compile()
                .Execute(_databaseManager);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), 1);

            var first = results.First();

            Assert.AreEqual(first.Key, 1);
            Assert.AreEqual(first.Value, "varchar");
        }

        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void KeyValuePairMapReverse()
        {
            var results =
                DefineCommand("SELECT String, Int FROM #DataTypeTable;")
                .WithResultsAs<KeyValuePair<int, string>>(whenMapping =>
                {
                    whenMapping.MapPropertyToColumn(o => o.Key, "Int");
                    whenMapping.MapPropertyToColumn(o => o.Value, "String");
                })
                .Compile()
                .Execute(_databaseManager);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), 1);

            var first = results.First();

            Assert.AreEqual(first.Key, 1);
            Assert.AreEqual(first.Value, "varchar");
        }

        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void KeyValuePairMapStringCoercion()
        {
            var results =
                DefineCommand("SELECT Int, String FROM #DataTypeTable;")
                .WithResultsAs<KeyValuePair<string, string>>(result =>
                {
                    result.MapPropertyToColumn(pair => pair.Key, "Int");
                    result.MapPropertyToColumn(pair => pair.Value, "String");
                })
                .Compile()
                .Execute(_databaseManager);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), 1);

            var first = results.First();

            Assert.AreEqual(first.Key, "1");
            Assert.AreEqual(first.Value, "varchar");
        }

        //[Test(Description = "Tests that results correctly map data to CLR types.")]
        //public void KeyValueWithWhereFilter()
        //{
        //    var results = DefineCommand<KeyValuePair<string, string>>("SELECT Int, String FROM #DataTypeTable")
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
        //        .BuildWhereFilter(new
        //        {
        //            Key = Comparison.Ignore,
        //            Value = Comparison.Equal
        //        })
        //        .Compile()
        //        .Execute(_databaseManager, new KeyValuePair<string, string>(null, "varchar"));

        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(1, results.Count());

        //    var first = results.First();

        //    Assert.AreEqual(first.Key, "1");
        //    Assert.AreEqual(first.Value, "varchar");
        //}


        //[Test(Description = "Tests that results correctly map data to CLR types.")]
        //public void KeyValueWithWhereFilterAndOrderBy()
        //{
        //    var results = DefineCommand<KeyValuePair<string, string>>(
        //        @"SELECT Int, String FROM ( VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four')) AS SampleSet(Int, String)")
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
        //        .Compile()
        //        .Execute(Setup.DatabaseManager, 
        //            new KeyValuePair<string, string>(null, "o"),
        //            new { OrderBy = "Int DESC" });

        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(3, results.Count());

        //    var first = results.First();

        //    Assert.AreEqual("4", first.Key);
        //    Assert.AreEqual("Four", first.Value);
        //}

        //[Test]
        //[ExpectedException(typeof(FormatException))]
        //public void OrderByThrowsIfUnsafe()
        //{
        //    DefineCommand<KeyValuePair<string, string>>(
        //        @"SELECT Int, String FROM ( VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four')) AS SampleSet(Int, String)")
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
        //        .Compile()
        //        .Execute(Setup.DatabaseManager,
        //            new KeyValuePair<string, string>(null, "o"),
        //            new { OrderBy = "Int DESC'" });
        //}
    }
}