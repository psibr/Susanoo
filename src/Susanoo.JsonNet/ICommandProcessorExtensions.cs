using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON = Newtonsoft.Json;

namespace Susanoo
{
    public static class ICommandProcessorExtensions
    {
        public static string ExecuteToJson<TFilter, TResult>(this ICommandProcessor<TFilter, TResult> processor,
            IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
            where TResult : new()
        {
            string results = null;

            ICommandExpression<TFilter> commandExpression = processor.CommandExpression;

            using (IDataReader record = databaseManager
                .ExecuteDataReader(
                    commandExpression.CommandText,
                    commandExpression.DbCommandType,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)))
            {
                var columnChecker = new ColumnChecker();

                var props = processor.CommandResultExpression.Export<TResult>();

                using (var writer = new StringWriter())
                using (var json = new JSON.JsonTextWriter(writer))
                {
                    json.WriteStartArray();

                    while (record.Read())
                    {
                        json.WriteStartObject();

                        foreach (var pair in props)
                        {
                            int ordinal = columnChecker.HasColumn(record, pair.Value.ActiveAlias);
                            if (ordinal >= 0)
                            {
                                json.WritePropertyName(pair.Key);
                                json.WriteValue(pair.Value.ConversionProcess(pair.Value.PropertyMetadata.PropertyType, record[ordinal]));
                            }
                        }

                        json.WriteEndObject();
                    }

                    json.WriteEndArray();

                    results = writer.ToString();
                }

                return results;
            }
        }

        public static string ExecuteToJson<TFilter, TResult>(this ICommandProcessor<TFilter, TResult> processor,
            IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
            where TResult : new()
        {
            return ExecuteToJson(processor, databaseManager, default(TFilter), null);
        }
    }
}