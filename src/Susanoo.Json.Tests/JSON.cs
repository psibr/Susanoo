using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Susanoo.Json.Tests;
using Susanoo.Pipeline.Command;

namespace Susanoo.Json.Tests
{
    [Category("JSON")]
    [TestFixture]
    public class JSON
    {
//        [Test(Description = "Tests that results correctly map data to CLR types.")]
//        public void KeyValueJson()
//        {
//            var results = CommandManager.DefineCommand<KeyValuePair<string, string>>(@"
//                SELECT Int, String
//                FROM (VALUES ('1', 'One'), ('2', 'Two'), ('3', 'Three'), ('4', 'Four')) AS SampleSet(Int, String)
//                ORDER BY Int", CommandType.Text)
//                .ExcludeProperty(o => o.Key)
//                .ExcludeProperty(o => o.Value)
//                .SendNullValues(NullValueMode.FilterOnlyFull)
//                .DefineResults<KeyValuePair<string, string>>()
//                .ForResults(expression =>
//                {
//                    expression.ForProperty(pair => pair.Key,
//                        configuration => configuration.UseAlias("Int"));
//                    expression.ForProperty(pair => pair.Value,
//                        configuration => configuration.UseAlias("String"));
//                })
//                .Realize()
//                .ExecuteToJson(Setup.DatabaseManager);

//            GC.KeepAlive(results);

//            Assert.AreEqual(1,1);
//        }
    }
}