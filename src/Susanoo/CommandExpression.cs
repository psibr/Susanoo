#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Text;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Contains information needed to build a command and provides FluentPipeline methods for defining results and
    ///     modifiers.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class CommandExpression<TFilter>
        : ICommandExpression<TFilter>
    {
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

        /// <summary>
        ///     The explicit inclusion mode
        /// </summary>
        private bool _explicitInclusionMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandExpression{TFilter}" /> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     databaseManager
        ///     or
        ///     commandText
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     No command text provided.;commandText
        ///     or
        ///     TableDirect is not supported.;commandType
        /// </exception>
        public CommandExpression(string commandText, CommandType commandType)
        {
            if (commandText == null)
                throw new ArgumentNullException("commandText");
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentException("No command text provided.", "commandText");
            if (commandType == CommandType.TableDirect)
                throw new ArgumentException("TableDirect is not supported.", "commandType");
            CommandText = commandText;
            DbCommandType = commandType;
        }

        /// <summary>
        ///     Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public virtual string CommandText { get; private set; }

        /// <summary>
        ///     Gets the type of the database command.
        /// </summary>
        /// <value>The type of the database command.</value>
        public virtual CommandType DbCommandType { get; private set; }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash
        {
            get
            {
                var hashText = new StringBuilder(CommandText);

                hashText.Append(DbCommandType);
                hashText.Append(_explicitInclusionMode.ToString());
                hashText.Append(_constantParameters.Aggregate(string.Empty, (p, c) => p + c.Key));
                hashText.Append(_parameterInclusions.Aggregate(string.Empty, (p, c) => p + c.Key));
                hashText.Append(_parameterExclusions.Aggregate(string.Empty, (p, c) => p + c));

                string resultBeforeHash = hashText.ToString();
                BigInteger hashCode = FnvHash.GetHash(resultBeforeHash, 128);

                return hashCode;
            }
        }

        /// <summary>
        ///     Realizes the pipeline with no result mappings.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter&gt;.</returns>
        public ICommandProcessor<TFilter> Realize()
        {
            return new NoResultSetCommandProcessor<TFilter>(this);
        }

        /// <summary>
        ///     Adds parameters that will always use the same value.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterModifier">The parameter modifier.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        public virtual ICommandExpression<TFilter> AddConstantParameter(string parameterName,
            Action<DbParameter> parameterModifier)
        {
            _constantParameters.Add(parameterName, parameterModifier);

            return this;
        }

        /// <summary>
        ///     Builds the parameters (Not part of Fluent API).
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        public virtual DbParameter[] BuildParameters(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters)
        {
            var propertyParameters = BuildPropertyParameters(databaseManager, filter);

            var dbParameters = propertyParameters as DbParameter[] ?? propertyParameters.ToArray();
            int parameterCount = (dbParameters.Count() + _constantParameters.Count) +
                                 ((explicitParameters != null) ? explicitParameters.Count() : 0);
            var parameters = new DbParameter[parameterCount];

            int i = 0;
            foreach (var item in dbParameters)
            {
                parameters[i] = item;
                i++;
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
                    parameters[i] = item;
                    i++;
                }

            return parameters;
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
        ///     Includes the property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICommandExpression<TFilter> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression)
        {
            return IncludeProperty(propertyExpression.GetPropertyName(), null);
        }

        /// <summary>
        ///     Includes the property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICommandExpression<TFilter> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression,
            Action<DbParameter> parameterOptions)
        {
            return IncludeProperty(propertyExpression.GetPropertyName(), parameterOptions);
        }

        /// <summary>
        ///     Includes the property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
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
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult> DefineResults<TResult>() where TResult : new()
        {
            return new CommandResultExpression<TFilter, TResult>(this);
        }

        /// <summary>
        ///     Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2> DefineResults<TResult1, TResult2>()
            where TResult1 : new()
            where TResult2 : new()
        {
            return new CommandResultExpression<TFilter, TResult1, TResult2>(this);
        }

        /// <summary>
        ///     Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> DefineResults
            <TResult1, TResult2, TResult3>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
        {
            return new CommandResultExpression<TFilter, TResult1, TResult2, TResult3>(this);
        }

        /// <summary>
        ///     Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> DefineResults
            <TResult1, TResult2, TResult3, TResult4>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
        {
            return new CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>(this);
        }

        /// <summary>
        ///     Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <typeparam name="TResult5">The type of the result5.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> DefineResults
            <TResult1, TResult2, TResult3, TResult4, TResult5>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
        {
            return new CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>(this);
        }

        /// <summary>
        ///     Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <typeparam name="TResult5">The type of the result5.</typeparam>
        /// <typeparam name="TResult6">The type of the result6.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
            DefineResults<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
            where TResult6 : new()
        {
            return new CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this);
        }

        /// <summary>
        ///     Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <typeparam name="TResult5">The type of the result5.</typeparam>
        /// <typeparam name="TResult6">The type of the result6.</typeparam>
        /// <typeparam name="TResult7">The type of the result7.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
            DefineResults
            <TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
            where TResult6 : new()
            where TResult7 : new()
        {
            return
                new CommandResultExpression
                    <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this);
        }

        /// <summary>
        ///     Builds the property inclusion parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        public virtual IEnumerable<DbParameter> BuildPropertyParameters(IDatabaseManager databaseManager, TFilter filter)
        {
            var properties = new List<DbParameter>();

            if (filter != null)
            {
                if (_explicitInclusionMode)
                {
                    foreach (var item in _parameterInclusions)
                    {
                        var propInfo = filter.GetType()
                            .GetProperty(item.Key, BindingFlags.Instance | BindingFlags.Public);
                        var param = databaseManager.CreateParameter();

                        param.ParameterName = item.Key;
                        param.Direction = ParameterDirection.Input;

                        param.Value = propInfo.GetValue(filter);

                        var type = CommandManager.GetDbType(propInfo.PropertyType);

                        if (type.HasValue)
                            param.DbType = type.Value;

                        if (item.Value != null)
                            item.Value.Invoke(param);

                        properties.Add(param);
                    }
                }
                else
                {
                    foreach (var propInfo in filter.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        )
                    {
                        if (!_parameterExclusions.Contains(propInfo.Name))
                        {
                            var param = databaseManager.CreateParameter();

                            param.ParameterName = propInfo.Name;
                            param.Direction = ParameterDirection.Input;

                            param.Value = propInfo.GetValue(filter);

                            var type = CommandManager.GetDbType(propInfo.PropertyType);

                            if (type.HasValue)
                                param.DbType = type.Value;

                            Action<DbParameter> value;
                            if (_parameterInclusions.TryGetValue(propInfo.Name, out value))
                            {
                                if (value != null)
                                    value.Invoke(param);
                            }
                            else
                            {
                                if (type == null)
                                    continue; //If we don't know what to do with the Type of the property
                                //and there isn't a explicit inclusion of the property, then ignore it.
                            }

                            properties.Add(param);
                        }
                    }
                }
            }

            return properties;
        }

        /// <summary>
        ///     Builds the parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;DbParameter&gt;.</returns>
        public IEnumerable<DbParameter> BuildParameters(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return BuildParameters(databaseManager, default(TFilter), explicitParameters);
        }
    }
}