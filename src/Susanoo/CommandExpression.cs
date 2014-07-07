using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Susanoo
{

    public class CommandExpression<TFilter>
        : ICommandExpression<TFilter>
    {
        /// <summary>
        /// The parameter inclusions
        /// </summary>
        private readonly IDictionary<string, Action<IDbDataParameter>> parameterInclusions = new Dictionary<string, Action<IDbDataParameter>>();
        /// <summary>
        /// The parameter exclusions
        /// </summary>
        private readonly IList<string> parameterExclusions = new List<string>();
        /// <summary>
        /// The constant parameters
        /// </summary>
        private readonly List<IDbDataParameter> constantParameters = new List<IDbDataParameter>();

        /// <summary>
        /// The explicit inclusion mode
        /// </summary>
        private bool explicitInclusionMode = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <exception cref="System.ArgumentNullException">
        /// databaseManager
        /// or
        /// commandText
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// No command text provided.;commandText
        /// or
        /// TableDirect is not supported.;commandType
        /// </exception>
        public CommandExpression(IDatabaseManager databaseManager, string commandText, CommandType commandType)
        {
            if (databaseManager == null)
                throw new ArgumentNullException("databaseManager");
            if (commandText == null)
                throw new ArgumentNullException("commandText");
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentException("No command text provided.", "commandText");
            if (commandType == CommandType.TableDirect)
                throw new ArgumentException("TableDirect is not supported.", "commandType");

            this.DatabaseManager = databaseManager;
            this.CommandText = commandText;
            this.DBCommandType = commandType;
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public virtual string CommandText { get; private set; }

        /// <summary>
        /// Gets the type of the database command.
        /// </summary>
        /// <value>The type of the database command.</value>
        public virtual CommandType DBCommandType { get; private set; }

        /// <summary>
        /// Gets the database manager.
        /// </summary>
        /// <value>The database manager.</value>
        public virtual IDatabaseManager DatabaseManager { get; private set; }
        /// <summary>
        /// Adds parameters that will always use the same value.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        public virtual ICommandExpression<TFilter> AddConstantParameters(params IDbDataParameter[] parameters)
        {
            this.constantParameters.AddRange(parameters);

            return this;
        }

        /// <summary>
        /// Builds the parameters (Not part of Fluent API).
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;IDbDataParameter&gt;.</returns>
        public virtual IDbDataParameter[] BuildParameters(TFilter filter, params IDbDataParameter[] explicitParameters)
        {
            IDbDataParameter[] parameters = null;

            int parameterCount = 0;

            IEnumerable<IDbDataParameter> propertyParameters = BuildPropertyParameters(filter);

            parameterCount = (propertyParameters.Count() + this.constantParameters.Count) + ((explicitParameters != null) ? explicitParameters.Count() : 0);
            parameters = new IDbDataParameter[parameterCount];

            int i = 0;
            foreach (var item in propertyParameters)
            {
                parameters[i] = item;
                i++;
            }

            foreach (var item in this.constantParameters)
            {
                parameters[i] = item;
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
        /// Builds the property inclusion parameters.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>IEnumerable&lt;IDbDataParameter&gt;.</returns>
        public virtual IEnumerable<IDbDataParameter> BuildPropertyParameters(TFilter filter)
        {
            var properties = new List<IDbDataParameter>();

            if (filter != null)
            {
                if (this.explicitInclusionMode)
                {
                    foreach (var item in parameterInclusions)
                    {
                        var propInfo = filter.GetType().GetProperty(item.Key, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                        var param = this.DatabaseManager.CreateParameter();

                        param.ParameterName = item.Key;
                        param.Direction = ParameterDirection.Input;

                        if (item.Value != null)
                            item.Value.Invoke(param);

                        param.Value = propInfo.GetValue(filter);

                        properties.Add(param);
                    }
                }
                else
                {
                    foreach (PropertyInfo propInfo in filter.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                    {
                        if (!this.parameterExclusions.Contains(propInfo.Name))
                        {
                            var param = this.DatabaseManager.CreateParameter();

                            param.ParameterName = propInfo.Name;
                            param.Direction = ParameterDirection.Input;

                            if (this.parameterInclusions.ContainsKey(propInfo.Name)
                                    && this.parameterInclusions[propInfo.Name] != null)
                                this.parameterInclusions[propInfo.Name].Invoke(param);

                            param.Value = propInfo.GetValue(filter);

                            properties.Add(param);
                        }
                    }
                }
            }

            return properties;
        }


        /// <summary>
        /// Builds the parameters.
        /// </summary>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;IDbDataParameter&gt;.</returns>
        public IEnumerable<IDbDataParameter> BuildParameters(params IDbDataParameter[] explicitParameters)
        {
            return this.BuildParameters(default(TFilter), explicitParameters);
        }


        /// <summary>
        /// Uses the explicit property inclusion mode for a potential filter.
        /// </summary>
        /// <returns>ICommandExpression&lt;TResult&gt;.</returns>
        public ICommandExpression<TFilter> UseExplicitPropertyInclusionMode()
        {
            this.explicitInclusionMode = true;

            return this;
        }

        /// <summary>
        /// Excludes a property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter> ExcludeProperty(Expression<Func<TFilter, object>> propertyExpression)
        {
            return this.ExcludeProperty(propertyExpression.GetPropertyName());
        }

        /// <summary>
        /// Excludes a property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter> ExcludeProperty(string propertyName)
        {
            if (!this.parameterExclusions.Contains(propertyName))
            {
                this.parameterExclusions.Add(propertyName);
            }

            return this;
        }

        /// <summary>
        /// Includes the property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICommandExpression<TFilter> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression)
        {
            return this.IncludeProperty(propertyExpression.GetPropertyName(), null);
        }

        /// <summary>
        /// Includes the property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICommandExpression<TFilter> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression, Action<IDbDataParameter> parameterOptions)
        {
            return this.IncludeProperty(propertyExpression.GetPropertyName(), parameterOptions);
        }

        /// <summary>
        /// Includes the property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICommandExpression<TFilter> IncludeProperty(string propertyName)
        {
            return this.IncludeProperty(propertyName, null);
        }

        /// <summary>
        /// Includes the property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter> IncludeProperty(string propertyName, Action<IDbDataParameter> parameterOptions)
        {
            if (this.parameterInclusions.Keys.Contains(propertyName))
            {
                this.parameterInclusions[propertyName] = parameterOptions;
            }
            else
            {
                this.parameterInclusions.Add(propertyName, parameterOptions);
            }

            return this;
        }

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult> DefineResultMappings<TResult>() where TResult : new()
        {
            return new CommandResultExpression<TFilter, TResult>(this);
        }

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2> DefineResultMappings<TResult1, TResult2>()
            where TResult1 : new()
            where TResult2 : new()
        {
            return new CommandResultExpression<TFilter, TResult1, TResult2>(this);
        }

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> DefineResultMappings<TResult1, TResult2, TResult3>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
        {
            return new CommandResultExpression<TFilter, TResult1, TResult2, TResult3>(this);
        }

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> DefineResultMappings<TResult1, TResult2, TResult3, TResult4>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
        {
            return new CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>(this);
        }

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <typeparam name="TResult5">The type of the result5.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> DefineResultMappings<TResult1, TResult2, TResult3, TResult4, TResult5>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
        {
            return new CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>(this);
        }

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <typeparam name="TResult5">The type of the result5.</typeparam>
        /// <typeparam name="TResult6">The type of the result6.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> DefineResultMappings<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>()
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
        /// Defines the result mappings.
        /// </summary>
        /// <typeparam name="TResult1">The type of the result1.</typeparam>
        /// <typeparam name="TResult2">The type of the result2.</typeparam>
        /// <typeparam name="TResult3">The type of the result3.</typeparam>
        /// <typeparam name="TResult4">The type of the result4.</typeparam>
        /// <typeparam name="TResult5">The type of the result5.</typeparam>
        /// <typeparam name="TResult6">The type of the result6.</typeparam>
        /// <typeparam name="TResult7">The type of the result7.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> DefineResultMappings<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>()
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
            where TResult6 : new()
            where TResult7 : new()
        {
            return new CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this);
        }
    }
}