using System.Data.Common;
using System.IO;
using JSON = Newtonsoft.Json;

namespace Susanoo.Json
{
    public static class DirectToJsonExtension
    {
        public static string ExecuteToJson<TFilter, TResult>(this ICommandProcessor<TFilter, TResult> processor,
            IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
            where TResult : new()
        {
            var commandExpression = processor.CommandExpression;

            using (var record = databaseManager
                .ExecuteDataReader(
                    commandExpression.CommandText,
                    commandExpression.DbCommandType,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)))
            {
                var columnChecker = processor.RetrieveColumnIndexInfo();

                var props = processor.CommandResultExpression.Export<TResult>();

                string results;
                using (var writer = new StringWriter())
                using (var json = new JSON.JsonTextWriter(writer))
                {
                    json.WriteStartArray();

                    while (record.Read())
                    {
                        json.WriteStartObject();

                        foreach (var pair in props)
                        {
                            var ordinal = columnChecker.HasColumn(record, pair.Value.ActiveAlias);
                            if (ordinal < 0) continue;

                            json.WritePropertyName(pair.Key);
                            json.WriteValue(JSON.JsonConvert.ToString(record[ordinal]));
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