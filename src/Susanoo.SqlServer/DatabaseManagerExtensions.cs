using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Susanoo.SqlServer;

namespace Susanoo
{
    /// <summary>
    /// Microsoft Sql Server specific extensions to IDatabaseManager
    /// </summary>
    public static class DatabaseManagerExtensions
    {
        /// <summary>
        /// Creates a table valued parameter from an IEnumerable of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="direction">The direction of the parameter.</param>
        /// <param name="items">The items.</param>
        /// <returns>DbParameter.</returns>
        /// <exception cref="NotSupportedException">databaseManager is not using SqlClient as provider.</exception>
        public static SqlParameter CreateTableValuedParameter<T>(this IDatabaseManager databaseManager, string name,
            string typeName, IEnumerable<T> items, ParameterDirection direction = ParameterDirection.Input)
        {
            var param = databaseManager.CreateParameter(name, direction, DbType.Object, null);
            param.Value = items;

            return param.MakeTableValuedParameter(name, typeName, direction);
        }
    }
}
