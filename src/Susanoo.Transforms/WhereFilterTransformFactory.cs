using System;
using System.Collections.Generic;
using System.Linq;
using Susanoo.Command;
using Susanoo.Mapping;
using Susanoo.Processing;

namespace Susanoo.Transforms
{
    internal sealed class WhereFilterTransformFactory<TFilter, TResult>
    {
        private readonly ISingleResultSetCommandProcessor<TFilter, TResult> _processor;

        public WhereFilterTransformFactory(ISingleResultSetCommandProcessor<TFilter, TResult> processor, IDictionary<string, object> whereFilterOptions)
        {
            _processor = processor;
            WhereFilterOptions = whereFilterOptions;
        }

        /// <summary>
        /// Gets the where filter options. Null if no where filter.
        /// </summary>
        /// <value>The where filter options.</value>
        private IDictionary<string, object> WhereFilterOptions { get; }

        /// <summary>
        /// Builds the where filter implementation.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>ICommandSingleResultExpression&lt;TFilter, TResult&gt;.</returns>
        public IExecutableCommandInfo BuildWhereFilterTransform(IExecutableCommandInfo info)
        {
            var mappings = info.Parameters
                .Join(_processor.CommandResultInfo.GetExporter().Export(typeof(TResult)), parameter =>
                        parameter.SourceColumn, pair => pair.Key,
                    (parameter, pair) =>
                        new Tuple<string, Type, string, string>(
                            pair.Key,                                 //Property Name
                            pair.Value.PropertyMetadata.PropertyType, //Property Type
                            parameter.ParameterName,                  //Parameter Name
                            pair.Value.ActiveAlias                    //Result Column Name
                            ))
                .GroupJoin(WhereFilterOptions, tuple => tuple.Item1, pair => pair.Key,
                    (tuple, pairs) => new { tuple, comparer = pairs.Select(kvp => kvp.Value).FirstOrDefault() })
                .Select(o => new Tuple<string, Type, string, string, object>(
                    o.tuple.Item1,                                          //Property Name
                    o.tuple.Item2,                                          //Property Type
                    o.tuple.Item3,                                          //Parameter Name
                    o.tuple.Item4,                                          //Result Column Name
                    o.comparer ?? GetDefaultCompareMethod(o.tuple.Item2)    //Comparer
                    ));

            return new ExecutableCommandInfo
            {
                CommandText = info.CommandText + string.Concat(mappings.Select(o =>
                {
                    var parameter = info.Parameters.SingleOrDefault(param => param.ParameterName == o.Item3);

                    var compareFormat = string.Empty;
                    //If no matching parameter or value is null or DBNull, don't add a comaparison.
                    if (parameter?.Value != null && parameter.Value != DBNull.Value)
                    {
                        if (o.Item5 is CompareMethod)
                            compareFormat = Comparison.GetComparisonFormat((CompareMethod)o.Item5);
                    }

                    var value = o.Item5 as ComparisonOverride;
                    compareFormat = value != null ? value.OverrideText : compareFormat;

                    if (compareFormat.Contains('{'))
                        compareFormat = string.Format(compareFormat, o.Item3, "[" + o.Item4 + "]");

                    return compareFormat;
                })),
                DbCommandType = info.DbCommandType,
                Parameters = info.Parameters
            };
        }

        /// <summary>
        /// Gets the default compare method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>CompareMethod.</returns>
        private static CompareMethod GetDefaultCompareMethod(Type type)
        {
            var result = Comparison.Equal;
            if (type == typeof(string))
                result = CompareMethod.Contains;
            else if (type == typeof(DateTime) || CommandManager.GetDbType(type) == null)
                result = CompareMethod.Ignore;

            return result;
        }
    }
}