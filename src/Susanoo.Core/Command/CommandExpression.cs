#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using Susanoo.Processing;
using Susanoo.ResultSets;

#endregion

namespace Susanoo.Command
{
    /// <summary>
    ///     Contains information needed to build a CommandBuilder and provides FluentPipeline methods for defining results and
    ///     modifiers.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class CommandExpression<TFilter> :
        ICommandExpression<TFilter>,
        ICommandBuilderInfo<TFilter>
    {
        private readonly IPropertyMetadataExtractor _propertyMetadataExtractor;
        private readonly INoResultSetCommandProcessorFactory _noResultSetCommandProcessorFactory;
        private readonly ICommandMultipleResultExpressionFactory _commandMultipleResultExpressionFactory;
        private readonly ICommandSingleResultExpressionFactory _commandSingleResultExpressionFactory;

        /// <summary>
        ///     The constant parameters
        /// </summary>
        private readonly Dictionary<string, Action<DbParameter>> _constantParameters =
            new Dictionary<string, Action<DbParameter>>();

        /// <summary>
        ///     The parameter exclusions
        /// </summary>
        private readonly IList<string> _parameterExclusions = new List<string>();

        /// <summary>
        ///     The parameter inclusions
        /// </summary>
        private readonly IDictionary<string, Action<DbParameter>> _parameterInclusions =
            new Dictionary<string, Action<DbParameter>>();

        private bool _doNotStoreColumnInfo;

        /// <summary>
        ///     The explicit inclusion mode
        /// </summary>
        private bool _explicitInclusionMode;

        private NullValueMode _nullValueMode = NullValueMode.Never;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExpression{TFilter}" /> class.
        /// </summary>
        /// <param name="propertyMetadataExtractor">A property metadata extractor.</param>
        /// <param name="noResultSetCommandProcessorFactory">A no result set command processor factory.</param>
        /// <param name="commandMultipleResultExpressionFactory">The command multiple result expression factory.</param>
        /// <param name="commandSingleResultExpressionFactory">The command single result expression factory.</param>
        /// <param name="commandText">The CommandBuilder text.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <exception cref="System.ArgumentNullException">databaseManager
        /// or
        /// commandText</exception>
        /// <exception cref="System.ArgumentException">No CommandBuilder text provided.;commandText
        /// or
        /// TableDirect is not supported.;commandType</exception>
        public CommandExpression(IPropertyMetadataExtractor propertyMetadataExtractor,
            INoResultSetCommandProcessorFactory noResultSetCommandProcessorFactory,
            ICommandMultipleResultExpressionFactory commandMultipleResultExpressionFactory,
            ICommandSingleResultExpressionFactory commandSingleResultExpressionFactory,
            string commandText, CommandType commandType)
        {
            if (propertyMetadataExtractor == null)
                throw new ArgumentNullException(nameof(propertyMetadataExtractor));
            if (noResultSetCommandProcessorFactory == null)
                throw new ArgumentNullException(nameof(noResultSetCommandProcessorFactory));
            if (commandText == null)
                throw new ArgumentNullException(nameof(commandText));
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentException("No CommandBuilder text provided.", nameof(commandText));
            if (commandType == CommandType.TableDirect)
                throw new ArgumentException("TableDirect is not supported.", nameof(commandType));

            _propertyMetadataExtractor = propertyMetadataExtractor;
            _noResultSetCommandProcessorFactory = noResultSetCommandProcessorFactory;
            _commandMultipleResultExpressionFactory = commandMultipleResultExpressionFactory;
            _commandSingleResultExpressionFactory = commandSingleResultExpressionFactory;

            CommandText = commandText;
            DbCommandType = commandType;
        }

        /// <summary>
        ///     Gets the type of the database CommandBuilder.
        /// </summary>
        /// <value>The type of the database CommandBuilder.</value>
        public virtual CommandType DbCommandType { get; }

        /// <summary>
        /// Gets the property whitelist.
        /// </summary>
        /// <value>The whitelist.</value>
        public IEnumerable<string> PropertyWhitelist => 
            _parameterInclusions.Keys;

        /// <summary>
        /// Gets the property blacklist.
        /// </summary>
        /// <value>The blacklist.</value>
        public IEnumerable<string> PropertyBlacklist => 
            _parameterExclusions;

        /// <summary>
        ///     Gets a value indicating whether storing column information is allowed.
        /// </summary>
        /// <value><c>true</c> if [allow store column information]; otherwise, <c>false</c>.</value>
        public bool AllowStoringColumnInfo => 
            !_doNotStoreColumnInfo;

        /// <summary>
        /// Builds the parameters (Not part of Fluent API).
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">Additional parameters.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        public virtual DbParameter[] BuildParameters(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters)
        {
            if (filter == null && parameterObject == null && _constantParameters.Count == 0 && explicitParameters == new DbParameter[0])
                return new DbParameter[0];

            var propertyParameters = filter != null ? BuildPropertyParameters(databaseManager, filter).ToArray() :  new DbParameter[0];
            var additionalParameters = parameterObject != null ? BuildPropertyParameters(databaseManager, parameterObject).ToArray() : new DbParameter[0];

            int parameterCount;
            DbParameter[] dbParameters = null;

            if (propertyParameters.Count() + additionalParameters.Count() > 0)
            {
                dbParameters =  propertyParameters.Concat(additionalParameters).ToArray();
                parameterCount = (dbParameters.Count() + _constantParameters.Count) +
                                 (explicitParameters?.Count() ?? 0);
            }
            else
            {
                parameterCount = (_constantParameters.Count) +
                                 (explicitParameters?.Count() ?? 0);
            }


            var parameters = new DbParameter[parameterCount];

            var i = 0;

            if (dbParameters != null)
            {
                foreach (var item in dbParameters)
                {
                    parameters[i] = item;
                    i++;
                }
            }

            foreach (var item in _constantParameters)
            {
                var parameter = databaseManager.CreateParameter();
                parameter.ParameterName = item.Key;
                parameter.Direction = ParameterDirection.Input;
                item.Value(parameter);
                parameters[i] = parameter;
                i++;
            }

            if (explicitParameters != null)
                foreach (var item in explicitParameters)
                {
                    if (_nullValueMode == NullValueMode.ExplicitParametersOnly
                        || _nullValueMode == NullValueMode.Full)
                    {
                        item.Value = ReplaceNullWithDbNull(item.Value);
                    }

                    parameters[i] = item;
                    i++;
                }

            return parameters;
        }

        /// <summary>
        ///     Gets the CommandBuilder text.
        /// </summary>
        /// <value>The CommandBuilder text.</value>
        public string CommandText { get; set; }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash => 
            ComputeHash();

        /// <summary>
        ///     Realizes the pipeline with no result mappings.
        /// </summary>
        /// <returns>INoResultCommandProcessor&lt;TFilter&gt;.</returns>
        public INoResultCommandProcessor<TFilter> Realize()
        {
            return _noResultSetCommandProcessorFactory
                .BuildCommandProcessor(this);
        }

        /// <summary>
        ///     Adds parameters that will always use the same value.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterModifier">The parameter transform.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        public virtual ICommandExpression<TFilter> AddConstantParameter(string parameterName,
            Action<DbParameter> parameterModifier)
        {
            _constantParameters.Add(parameterName, parameterModifier);

            return this;
        }

        /// <summary>
        ///     ADO.NET ignores parameters with NULL values. calling this opts in to send DbNull in place of NULL on standard
        ///     parameters.
        ///     Properties with transform Actions do NOT qualify for this behavior
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public ICommandExpression<TFilter> SendNullValues(NullValueMode mode = NullValueMode.FilterOnlyMinimum)
        {
            _nullValueMode = mode;

            return this;
        }

        /// <summary>
        ///     Disables Susanoo's ability to cache a result sets column indexes and names for faster retrieval.
        ///     This is typically only needed for stored procedures that return different columns or columns in different orders
        ///     based on criteria in the procedure.
        /// </summary>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public ICommandExpression<TFilter> DoNotStoreColumnIndexes()
        {
            _doNotStoreColumnInfo = true;

            return this;
        }

        /// <summary>
        ///     Uses the explicit property inclusion mode for a potential filter.
        /// </summary>
        /// <returns>ICommandExpression&lt;TResult&gt;.</returns>
        public ICommandExpression<TFilter> UseExplicitPropertyInclusionMode()
        {
            _explicitInclusionMode = true;

            return this;
        }

        /// <summary>
        ///     Excludes a property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter> ExcludeProperty(Expression<Func<TFilter, object>> propertyExpression)
        {
            return ExcludeProperty(propertyExpression.GetPropertyName());
        }

        /// <summary>
        ///     Excludes a property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter> ExcludeProperty(string propertyName)
        {
            if (!_parameterExclusions.Contains(propertyName))
            {
                _parameterExclusions.Add(propertyName);
            }

            return this;
        }

        /// <summary>
        /// Includes the property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression)
        {
            return IncludeProperty(propertyExpression.GetPropertyName(), null);
        }

        /// <summary>
        /// Includes the property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression,
            Action<DbParameter> parameterOptions)
        {
            return IncludeProperty(propertyExpression.GetPropertyName(), parameterOptions);
        }

        /// <summary>
        /// Includes the property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter> IncludeProperty(string propertyName)
        {
            return IncludeProperty(propertyName, null);
        }

        /// <summary>
        ///     Includes the property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter> IncludeProperty(string propertyName, Action<DbParameter> parameterOptions)
        {
            if (_parameterInclusions.Keys.Contains(propertyName))
            {
                _parameterInclusions[propertyName] = parameterOptions;
            }
            else
            {
                _parameterInclusions.Add(propertyName, parameterOptions);
            }

            return this;
        }

        /// <summary>
        ///     Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>ICommandSingleResultExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandSingleResultExpression<TFilter, TResult> DefineResults<TResult>()
        {
            return _commandSingleResultExpressionFactory
                .BuildCommandSingleResultExpression<TFilter, TResult>(this);
        }

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <param name="resultTypes">The result types in order.</param>
        /// <returns>ICommandSingleResultExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandMultipleResultExpression<TFilter> DefineResults(params Type[] resultTypes)
        {
            return _commandMultipleResultExpressionFactory
                .BuildCommandMultipleResultExpression(this, resultTypes);
        }

        /// <summary>
        /// Computes the hash.
        /// </summary>
        private BigInteger ComputeHash()
        {
            //This is faster than string builder and less resource intensive
            var strings =
                string.Concat(_constantParameters.Select(c => c.Key)
                    .Concat(_parameterInclusions.Select(c => c.Key))
                    .Concat(_parameterExclusions)
                    .Concat(new[]
                    {
                        CommandText,
                        DbCommandType.ToString(),
                        _explicitInclusionMode.ToString(),
                        _nullValueMode.ToString()
                    }));

            return HashBuilder.Compute(strings);
        }


        /// <summary>
        ///     Builds the property inclusion parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        public virtual IEnumerable<DbParameter> BuildPropertyParameters(IDatabaseManager databaseManager, object filter)
        {
            var parameters = new List<DbParameter>();

            if (typeof(TFilter).IsValueType || filter != null)
            {
                if (_explicitInclusionMode)
                {
                    foreach (var item in _parameterInclusions)
                    {
                        var propInfo = filter.GetType()
                            .GetProperty(item.Key, BindingFlags.Instance | BindingFlags.Public);
                        var param = databaseManager.CreateParameter();

                        param.SourceColumn = propInfo.Name;
                        param.ParameterName = item.Key;
                        param.Direction = ParameterDirection.Input;

#if !NETFX40
                        param.Value = propInfo.GetValue(filter);
#else
                        param.Value = propInfo.GetValue(filter, null);
#endif

                        var type = CommandManager.GetDbType(propInfo.PropertyType);

                        if (type.HasValue)
                            param.DbType = type.Value;

                        var value = item.Value;
                        if (value != null)
                        {
                            value.Invoke(param);
                            if (_nullValueMode == NullValueMode.FilterOnlyFull
                                || _nullValueMode == NullValueMode.Full)
                            {
                                param.Value = ReplaceNullWithDbNull(param.Value);
                            }
                        }
                        else if (_nullValueMode == NullValueMode.FilterOnlyMinimum
                                 || _nullValueMode == NullValueMode.FilterOnlyFull
                                 || _nullValueMode == NullValueMode.Full)
                        {
                            param.Value = ReplaceNullWithDbNull(param.Value);
                        }

                        parameters.Add(param);
                    }
                }
                else
                {
                    var implicitProperties = _propertyMetadataExtractor
                        .FindAllowedProperties(
                            filter.GetType(),
                            DescriptorActions.Insert | DescriptorActions.Update | DescriptorActions.Delete,
                            _parameterInclusions.Select(p => p.Key).ToArray(),
                            _parameterExclusions.ToArray());

                    foreach (var propInfo in implicitProperties)
                    {
                        if (!_parameterExclusions.Contains(propInfo.Value.ActiveAlias))
                        {
                            var param = databaseManager.CreateParameter();

                            param.SourceColumn = propInfo.Key.Name;
                            param.ParameterName = propInfo.Value.ActiveAlias;
                            param.Direction = ParameterDirection.Input;
#if !NETFX40
                            param.Value = propInfo.Key.GetValue(filter);
#else
                            param.Value = propInfo.Key.GetValue(filter, null);
#endif

                            var type = CommandManager.GetDbType(propInfo.Key.PropertyType);

                            if (type.HasValue)
                                param.DbType = type.Value;

                            Action<DbParameter> value;
                            if (_parameterInclusions.TryGetValue(propInfo.Value.ActiveAlias, out value))
                            {
                                if (value != null)
                                {
                                    value.Invoke(param);
                                    if (_nullValueMode == NullValueMode.FilterOnlyFull
                                        || _nullValueMode == NullValueMode.Full)
                                    {
                                        param.Value = ReplaceNullWithDbNull(param.Value);
                                    }
                                }
                                else if (_nullValueMode == NullValueMode.FilterOnlyMinimum
                                         || _nullValueMode == NullValueMode.FilterOnlyFull
                                         || _nullValueMode == NullValueMode.Full)
                                {
                                    param.Value = ReplaceNullWithDbNull(param.Value);
                                }
                            }
                            else
                            {
                                if (type == null)
                                    continue; //If we don't know what to do with the Type of the property
                                //and there isn't a explicit inclusion of the property, then ignore it.

                                if (_nullValueMode == NullValueMode.FilterOnlyMinimum
                                    || _nullValueMode == NullValueMode.FilterOnlyFull
                                    || _nullValueMode == NullValueMode.Full)
                                {
                                    param.Value = ReplaceNullWithDbNull(param.Value);
                                }
                            }

                            parameters.Add(param);
                        }
                    }
                }
            }

            return parameters;
        }

        /// <summary>
        ///     Replaces the null with database null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        protected object ReplaceNullWithDbNull(object value)
        {
            var returnValue = value;
            if (value == null)
                returnValue = DBNull.Value;

            return returnValue;
        }

        /// <summary>
        /// Builds the parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        public IEnumerable<DbParameter> BuildParameters(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return BuildParameters(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Builds the parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        public IEnumerable<DbParameter> BuildParameters(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters)
        {
            return BuildParameters(databaseManager, filter, null, explicitParameters);
        }
    }
}