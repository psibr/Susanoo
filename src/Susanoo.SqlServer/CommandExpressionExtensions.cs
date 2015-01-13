using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Susanoo.SqlServer;

namespace Susanoo
{
    public static class CommandExpressionExtensions
    {
        /// <summary>
        /// Includes the property as structured datatype (Table Valued Parameter).
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="commandExpression">The command expression.</param>
        /// <param name="property">The property.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public static ICommandExpression<TFilter> IncludePropertyAsStructured<TFilter>(
            this ICommandExpression<TFilter> commandExpression, Expression<Func<TFilter, object>> property, string typeName)
        {
            return IncludePropertyAsStructured(commandExpression, property.Name, typeName);
        }

        /// <summary>
        /// Includes the property as structured datatype (Table Valued Parameter).
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="commandExpression">The command expression.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public static ICommandExpression<TFilter> IncludePropertyAsStructured<TFilter>(
            this ICommandExpression<TFilter> commandExpression, string propertyName, string typeName)
        {
            commandExpression.IncludeProperty(propertyName,
                parameter => parameter.MakeTableValuedParameter(propertyName, typeName));

            return commandExpression;
        }

        /// <summary>
        /// Includes the property as structured datatype (Table Valued Parameter).
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="commandExpression">The command expression.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public static ICommandExpression<TFilter> IncludePropertyAsStructured<TFilter>(
            this ICommandExpression<TFilter> commandExpression, string propertyName, string typeName, Action<DbParameter> parameterOptions)
        {

            if (parameterOptions != null)
            {
                commandExpression.IncludeProperty(propertyName,
                    CombineActions(parameter => parameter.MakeTableValuedParameter(propertyName, typeName), parameterOptions));
            }
            else
            {
                commandExpression.IncludeProperty(propertyName,
                parameter => parameter.MakeTableValuedParameter(propertyName, typeName));
            }

            return commandExpression;
        }


        /// <summary>
        /// Includes the property as structured datatype (Table Valued Parameter).
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="commandExpression">The command expression.</param>
        /// <param name="property">The property.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public static ICommandExpression<TFilter> IncludePropertyAsStructured<TFilter>(
            this ICommandExpression<TFilter> commandExpression, Expression<Func<TFilter, object>> property, string typeName, Action<DbParameter> parameterOptions)
        {
            return IncludePropertyAsStructured(commandExpression, property.Name, typeName, parameterOptions);
        }


        internal static Action<DbParameter> CombineActions(Action<DbParameter> arg0, Action<DbParameter> arg1)
        {
            return arg0 + arg1;
        }

        /// <summary>
        /// Creates a table valued parameter from an IEnumerable of T.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="direction">The direction of the parameter.</param>
        /// <returns>DbParameter.</returns>
        /// <exception cref="System.NotSupportedException">databaseManager is not using SqlClient as provider.</exception>
        /// <exception cref="NotSupportedException">databaseManager is not using SqlClient as provider.</exception>
        internal static SqlParameter MakeTableValuedParameter(this DbParameter parameter, string name,
            string typeName, ParameterDirection direction = ParameterDirection.Input)
        {
            var param = parameter;

            var sqlParam = param as SqlParameter;

            if (sqlParam == null)
                throw new NotSupportedException("databaseManager is not using SqlClient as provider.");

            sqlParam.ResetDbType();
            sqlParam.SqlDbType = SqlDbType.Structured;
            sqlParam.TypeName = typeName;
            sqlParam.Value = ((IEnumerable)sqlParam.Value).ToDataRecords();

            return sqlParam;
        }
    }
}
