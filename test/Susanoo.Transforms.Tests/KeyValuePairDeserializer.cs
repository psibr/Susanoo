#region

using NUnit.Framework;
using Susanoo.Command;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Susanoo.SusanooCommander;

#endregion

namespace Susanoo.Transforms.Tests
{
    [Category("Type Resolution")]
    [TestFixture]
    public class KeyValuePairTests
    {
        private readonly DatabaseManager _databaseManager = Setup.DatabaseManager;

        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void KeyValueWithWhereFilter()
        {
            var results = 
                DefineCommand<KeyValuePair<string, string>>(
                    "SELECT Int, String FROM #DataTypeTable")
                .IncludeProperty(o => o.Key, parameter => parameter.ParameterName = "Int")
                .IncludeProperty(o => o.Value, parameter => parameter.ParameterName = "String")
                .SendNullValues(NullValueMode.FilterOnlyFull)
                .WithResultsAs<KeyValuePair<string, string>>()
                .ForResults(result =>
                {
                    result.MapPropertyToColumn(pair => pair.Key, "Int");
                    result.MapPropertyToColumn(pair => pair.Value, "String");
                })
                .Compile()
                .WithTransforms(source => new[]
                {
                    Transforms.QueryWrapper(),
                    Transforms.WhereFilter(source, typeof(KeyValuePair<string, string>),
                        new
                        {
                            Key = Comparison.Ignore,
                            Value = Comparison.Equal
                        })
                })
                .Execute(_databaseManager, new KeyValuePair<string, string>(null, "varchar"));

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());

            var first = results.First();

            Assert.AreEqual(first.Key, "1");
            Assert.AreEqual(first.Value, "varchar");
        }


        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void KeyValueWithWhereFilterAndOrderBy()
        {
            var results = 
                DefineCommand<Tuple<string, string>>(
                    "SELECT Int, String\n" +
                    "FROM ( VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four'))\n" +
                    "\tAS SampleSet(Int, String)")
                .IncludeProperty(o => o.Item1, parameter => parameter.ParameterName = "Int")
                .IncludeProperty(o => o.Item2, parameter => parameter.ParameterName = "String")
                .SendNullValues(NullValueMode.FilterOnlyFull)
                .WithResultsAs<KeyValuePair<string, string>>(result =>
                {
                    result.MapPropertyToColumn(pair => pair.Key, "Int");
                    result.MapPropertyToColumn(pair => pair.Value, "String");
                })
                .Compile()
                .WithTransforms(source => new[]
                {
                    Transforms.QueryWrapper(),
                    Transforms.WhereFilter(source, typeof(KeyValuePair<string, string>)),
                    Transforms.OrderByExpression(),
                },(info) =>
                {
                })
                .Execute(Setup.DatabaseManager,
                    new Tuple<string, string>(null, "o"),
                    new { OrderBy = "Int DESC" });

            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.Count());

            var first = results.First();

            Assert.AreEqual("4", first.Key);
            Assert.AreEqual("Four", first.Value);
        }

        [Test]
        public void OrderByThrowsIfUnsafe()
        {
            Assert.Catch<FormatException>(() =>
            {                
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
                    Transforms.QueryWrapper(),
                    Transforms.WhereFilter(source, typeof(KeyValuePair<string, string>)),
                    Transforms.OrderByExpression()
                })
                .Execute(Setup.DatabaseManager,
                    new KeyValuePair<string, string>(null, "o"),
                    new { OrderBy = "Int DESC'" });
            });

        }
    }
}