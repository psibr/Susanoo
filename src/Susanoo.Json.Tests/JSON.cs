using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Susanoo.Pipeline.Command;

namespace Susanoo.Json.Tests
{
    [Category("JSON")]
    [TestFixture]
    public class JSON
    {
        [Test(Description = "Tests that results correctly map data to CLR types.")]
        public void KeyValueJson()
        {
            var results = CommandManager.DefineCommand<KeyValuePair<string, string>>(@"
                    SELECT Int, String
                    FROM (VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four')) AS SampleSet(Int, String)
                    ORDER BY Int", CommandType.Text)
                .ExcludeProperty(o => o.Key)
                .ExcludeProperty(o => o.Value)
                .SendNullValues(NullValueMode.FilterOnlyFull)
                .DefineResults<KeyValuePair<string, string>>()
                .ForResults(expression =>
                {
                    expression.ForProperty(pair => pair.Key,
                        configuration => configuration.UseAlias("Int"));

                    expression.ForProperty(pair => pair.Value,
                        configuration => configuration.UseAlias("String"));
                })
                .Realize()
                .ExecuteToJson(Setup.DatabaseManager);

            Assert.AreEqual(
                 "[{\"Int\":\"\\\"1\\\"\",\"String\":\"\\\"One\\\"\"},"
                + "{\"Int\":\"\\\"2\\\"\",\"String\":\"\\\"Two\\\"\"}," 
                + "{\"Int\":\"\\\"3\\\"\",\"String\":\"\\\"Three\\\"\"},"
                + "{\"Int\":\"\\\"4\\\"\",\"String\":\"\\\"Four\\\"\"}]", 
                results);
        }
    }
}