using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Susanoo
{
    public class CommandExpression<TFilter, TResult>
        : ICommandExpression<TFilter, TResult>
        where TResult : new()
    {
        private readonly ReaderWriterLockSlim threadSync = new ReaderWriterLockSlim();
        private readonly IDictionary<string, Action<IDbDataParameter>> parameterInclusions = new Dictionary<string, Action<IDbDataParameter>>();
        private readonly List<IDbDataParameter> explicitParameters = new List<IDbDataParameter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExpression{TFilter, TResult}"/> class.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        public CommandExpression(IDatabaseManager databaseManager, string commandText, CommandType commandType)
        {
            this.DatabaseManager = databaseManager;
            this.CommandText = commandText;
            this.DbCommandType = commandType;
        }

        /// <summary>
        /// Includes the property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression)
        {
            return IncludeProperty(propertyExpression, null);
        }

        /// <summary>
        /// Includes the property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression, Action<IDbDataParameter> parameterOptions)
        {
            return IncludeProperty(propertyExpression.GetPropertyName(), parameterOptions);
        }

        /// <summary>
        /// Includes the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> IncludeProperty(string propertyName)
        {
            return IncludeProperty(propertyName, null);
        }

        /// <summary>
        /// Includes the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> IncludeProperty(string propertyName, Action<IDbDataParameter> parameterOptions)
        {
            this.threadSync.EnterWriteLock();
            this.parameterInclusions.Add(propertyName, parameterOptions);
            this.threadSync.ExitWriteLock();

            return this;
        }

        /// <summary>
        /// Adds the parameters explicitly.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> AddParameters(params IDbDataParameter[] parameters)
        {
            this.threadSync.EnterWriteLock();
            this.explicitParameters.AddRange(parameters);
            this.threadSync.ExitWriteLock();

            return this;
        }

        /// <summary>
        /// Excludes the property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>Susanoo.ICommandExpression&lt;TFilter,TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> ExcludeProperty(Expression<Func<TFilter, object>> propertyExpression)
        {
            return ExcludeProperty(propertyExpression.GetPropertyName());
        }

        /// <summary>
        /// Excludes the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> ExcludeProperty(string propertyName)
        {
            if (this.parameterInclusions.ContainsKey(propertyName))
            {
                this.threadSync.EnterWriteLock();
                this.parameterInclusions.Remove(propertyName);
                this.threadSync.ExitWriteLock();
            }

            return this;
        }

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandResultMappingExpression<TFilter, TResult> DefineResultMappings()
        {
            return new CommandResultMappingExpression<TFilter, TResult>(this);
        }

        /// <summary>
        /// Builds the parameters.
        /// </summary>
        /// <returns>IEnumerable&lt;IDbDataParameter&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<IDbDataParameter> BuildParameters(TFilter filter, params IDbDataParameter[] explicitParameters)
        {
            List<IDbDataParameter> parameters = BuildPropertyParameters(filter).ToList();

            parameters.AddRange(this.explicitParameters);
            parameters.AddRange(explicitParameters);

            return parameters;
        }

        /// <summary>
        /// Builds the property inclusion parameters.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>IEnumerable&lt;IDbDataParameter&gt;.</returns>
        public IEnumerable<IDbDataParameter> BuildPropertyParameters(TFilter filter)
        {
            foreach (var item in this.parameterInclusions)
            {
                var propInfo = typeof(TFilter).GetProperty(item.Key, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                var param = this.DatabaseManager.CreateParameter();

                param.ParameterName = item.Key;
                param.Direction = ParameterDirection.Input;

                if(item.Value != null)
                    item.Value.Invoke(param);

                param.Value = propInfo.GetValue(filter);

                yield return param;
            }
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText { get; private set; }

        /// <summary>
        /// Gets the type of the database command.
        /// </summary>
        /// <value>The type of the database command.</value>
        public CommandType DbCommandType { get; private set; }

        /// <summary>
        /// Gets the database manager.
        /// </summary>
        /// <value>The database manager.</value>
        public IDatabaseManager DatabaseManager { get; private set; }
    }
}
