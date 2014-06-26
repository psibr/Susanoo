using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Susanoo
{
    public class CommandExpression<TFilter, TResult>
        : ICommandExpression<TFilter, TResult>
        where TResult : new()
    {
        private readonly IDictionary<string, Action<IDbDataParameter>> parameterInclusions = new Dictionary<string, Action<IDbDataParameter>>();
        private readonly IList<string> parameterExclusions = new List<string>();
        private readonly List<IDbDataParameter> constantParameters = new List<IDbDataParameter>();

        private bool explicitInclusionMode = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        public CommandExpression(IDatabaseManager databaseManager, string commandText, CommandType commandType)
        {
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
        public virtual ICommandExpression<TFilter, TResult> AddConstantParameters(params IDbDataParameter[] parameters)
        {
            this.constantParameters.AddRange(parameters);

            return this;
        }

        /// <summary>
        /// Defines the result mappings (Moves to next Step in Fluent API).
        /// </summary>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual ICommandResultMappingExpression<TFilter, TResult> DefineResultMappings()
        {
            return new CommandResultMappingExpression<TFilter, TResult>(this);
        }

        /// <summary>
        /// Builds the parameters (Not part of Fluent API).
        /// </summary>
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
        public ICommandExpression<TFilter, TResult> UseExplicitPropertyInclusionMode()
        {
            this.explicitInclusionMode = true;

            return this;
        }

        /// <summary>
        /// Excludes a property of the filter.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> ExcludeProperty(Expression<Func<TFilter, object>> propertyExpression)
        {
            return this.ExcludeProperty(propertyExpression.GetPropertyName());
        }

        /// <summary>
        /// Excludes a property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> ExcludeProperty(string propertyName)
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
        public ICommandExpression<TFilter, TResult> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression)
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
        public ICommandExpression<TFilter, TResult> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression, Action<IDbDataParameter> parameterOptions)
        {
            return this.IncludeProperty(propertyExpression.GetPropertyName(), parameterOptions);
        }

        /// <summary>
        /// Includes the property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICommandExpression<TFilter, TResult> IncludeProperty(string propertyName)
        {
            return this.IncludeProperty(propertyName, null);
        }

        /// <summary>
        /// Includes the property of the filter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> IncludeProperty(string propertyName, Action<IDbDataParameter> parameterOptions)
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
    }
}