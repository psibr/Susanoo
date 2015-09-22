#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using NUnit.Framework;

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
            var results = CommandManager.Instance.DefineCommand("SELECT Int, String FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<KeyValuePair<int, string>>()
                .ForResults(expression =>
                {
                    expression.ForProperty(pair => pair.Key, configuration => configuration.UseAlias("Int"));
                    expression.ForProperty(pair => pair.Value, configuration => configuration.UseAlias("String"));
                })
                .Realize()
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
            var results = CommandManager.Instance.DefineCommand("SELECT String, Int FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<KeyValuePair<int, string>>()
                .ForResults(expression =>
                {
                    expression.ForProperty(pair => pair.Key, configuration => configuration.UseAlias("Int"));
                    expression.ForProperty(pair => pair.Value, configuration => configuration.UseAlias("String"));
                })
                .Realize()
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
            var results = CommandManager.Instance.DefineCommand("SELECT Int, String FROM #DataTypeTable;", CommandType.Text)
                .DefineResults<KeyValuePair<string, string>>()
                .ForResults(expression =>
                {
                    expression.ForProperty(pair => pair.Key, configuration => configuration.UseAlias("Int"));
                    expression.ForProperty(pair => pair.Value, configuration => configuration.UseAlias("String"));
                })
                .Realize()
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
        //    var results = CommandManager.Instance.DefineCommand<KeyValuePair<string, string>>("SELECT Int, String FROM #DataTypeTable", CommandType.Text)
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
        //        .Realize()
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
        //        .Realize()
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
        //    CommandManager.Instance.DefineCommand<KeyValuePair<string, string>>(
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
        //        .Realize()
        //        .Execute(Setup.DatabaseManager,
        //            new KeyValuePair<string, string>(null, "o"),
        //            new { OrderBy = "Int DESC'" });
        //}
    }
}