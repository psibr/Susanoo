﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Susanoo
{
    /// <summary>
    /// Sql Server specific extensions to IDatabaseManager
    /// </summary>
    public static class IDatabaseManagerExtensions
    {
        /// <summary>
        /// Creates a table valued parameter from an IEnumerable of T. Must be disposed.
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

            var sqlParam = param as SqlParameter;

            if (sqlParam == null)
                throw new NotSupportedException("databaseManager is not using SqlClient as provider.");

            sqlParam.ResetDbType();
            sqlParam.SqlDbType = SqlDbType.Structured;
            sqlParam.TypeName = typeName;
            sqlParam.Value = items.ToDataRecords();

            return sqlParam;
        }
    }
}