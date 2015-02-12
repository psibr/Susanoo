using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Susanoo.SqlServer
{
    /// <summary>
    /// Extensions for SQL Parameters
    /// </summary>
    public static class DbParameterExtensions
    {
        /// <summary>
        /// Creates a table valued parameter from an IEnumerable of T.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="direction">The direction of the parameter.</param>
        /// <returns>DbParameter.</returns>
        /// <exception cref="System.NotSupportedException">databaseManager is not using SqlClient as provider.</exception>
        public static SqlParameter MakeTableValuedParameter(this DbParameter parameter, string name,
            string typeName, ParameterDirection direction = ParameterDirection.Input)
        {
            var param = parameter;

            var sqlParam = param as SqlParameter;

            if (sqlParam == null)
                throw new NotSupportedException("databaseManager is not using SqlClient as provider.");

            sqlParam.ParameterName = name;
            sqlParam.Direction = direction;
            sqlParam.ResetDbType();
            sqlParam.SqlDbType = SqlDbType.Structured;
            sqlParam.TypeName = typeName;
            sqlParam.Value = ((IEnumerable)sqlParam.Value).ToDataRecords();

            return sqlParam;
        }
    }
}
