using System.Data;

namespace Susanoo
{
    /// <summary>
    /// The interface a Data later abstraction must support for use with Susanoo
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// Executes the data reader.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IDataReader.</returns>
        IDataReader ExecuteDataReader(string commandText, CommandType commandType, IDbTransaction transaction, params IDbDataParameter[] parameters);

        /// <summary>
        /// Executes the data reader.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IDataReader.</returns>
        IDataReader ExecuteDataReader(string commandText, CommandType commandType, params IDbDataParameter[] parameters);

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>T.</returns>
        T ExecuteScalar<T>(string commandText, CommandType commandType, IDbTransaction transaction, params IDataParameter[] parameters);

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>T.</returns>
        T ExecuteScalar<T>(string commandText, CommandType commandType, params IDataParameter[] parameters);

        /// <summary>
        /// Executes the stored procedure non query.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteStoredProcedureNonQuery(string commandText, CommandType commandType, IDbTransaction transaction, params IDbDataParameter[] parameters);

        /// <summary>
        /// Executes the stored procedure non query.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteStoredProcedureNonQuery(string commandText, CommandType commandType, params IDbDataParameter[] parameters);

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <returns>IDbDataParameter.</returns>
        IDbDataParameter CreateParameter();

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <returns>IDbTransaction.</returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>IDbDataParameter.</returns>
        IDbDataParameter CreateParameter(string parameterName, ParameterDirection parameterDirection, DbType parameterType, object value);

        /// <summary>
        /// Creates the input parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>IDbDataParameter.</returns>
        IDbDataParameter CreateInputParameter(string parameterName, DbType parameterType, object value);
    }
}