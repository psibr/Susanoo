using System.Data.Common;
using System.IO;
using Susanoo.Pipeline.Command.ResultSets.Processing;
using JSON = Newtonsoft.Json;

namespace Susanoo
{
    public static class DirectToJsonExtension
    {
        public static string ExecuteToJson<TFilter, TResult>(this ICommandProcessor<TFilter, TResult> processor,
            IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
            where TResult : new()
        {
            var commandExpression = processor.CommandInfo;

            using (var record = databaseManager
                .ExecuteDataReader(
                    commandExpression.CommandText,
                    commandExpression.DbCommandType,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)))
            {
                var columnChecker = processor.RetrieveColumnIndexInfo() ?? new ColumnChecker();

                var fieldCount = record.FieldCount;

                string results;
                using (var writer = new StringWriter())
                using (var json = new JSON.JsonTextWriter(writer))
                {
                    json.WriteStartArray();

                    while (record.Read())
                    {
                        json.WriteStartObject();

                        for (var i = 0; i < fieldCount; i++)
                        {
                            var name = columnChecker.HasColumn(record, i);

                            json.WritePropertyName(name);
                            json.WriteValue(JSON.JsonConvert.ToString(record[i]));
                        }

                        json.WriteEndObject();
                    }

                    json.WriteEndArray();

                    results = writer.ToString();
                }

                processor.UpdateColumnIndexInfo(columnChecker);

                return results;
            }
        }

        public static string ExecuteToJson<TFilter, TResult>(this ICommandProcessor<TFilter, TResult> processor,
            IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
            where TResult : new()
        {
            return ExecuteToJson(processor, databaseManager, default(TFilter), explicitParameters);
        }
    }
}