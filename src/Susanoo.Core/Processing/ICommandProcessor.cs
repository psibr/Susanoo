#region

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Susanoo.Command;

#endregion

namespace Susanoo.Processing
{
    /// <summary>
    /// Represents a fully built and ready to be executed CommandBuilder expression with no mapping expressions and a filter
    /// parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandProcessor<TFilter> : ICommandProcessorInterop<TFilter>
#if !NETFX40
, ICommandProcessorAsync<TFilter>
#endif
    {
        /// <summary>
        /// Executes the CommandBuilder and returns a return code.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteNonQuery(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo);

        /// <summary>
        /// Executes the CommandBuilder and retrieves a single value.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>TReturn.</returns>
        TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo);

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>ICommandProcessor&lt;TFilter&gt;.</returns>
        ICommandProcessor<TFilter> InterceptOrProxyWith(
            Func<ICommandProcessor<TFilter>, ICommandProcessor<TFilter>> interceptOrProxy);

        /// <summary>
        /// Executes the CommandBuilder and retrieves a single value.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder and retrieves a single value.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the command and retrieves a single value.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder and returns a return code.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder and returns a return code.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder and returns a return code.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteNonQuery(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }

#if !NETFX40
    /// <summary>
    /// Async actions for no result CommandBuilder processors.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandProcessorAsync<in TFilter>
    {
        /// <summary>
        /// Executes the scalar asynchronous.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, CancellationToken cancellationToken,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the non query asynchronous.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, CancellationToken cancellationToken,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder and returns a return code asynchronously.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Int32.</returns>
        Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the scalar action asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>TReturn.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the scalar action asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, CancellationToken cancellationToken, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the scalar action asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the scalar action asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the scalar action asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Async actions for Single result CommandBuilder processors.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface ICommandProcessorAsync<in TFilter, TResult>
    {
        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, CancellationToken cancellationToken, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, CancellationToken cancellationToken, params DbParameter[] explicitParameters);
    }

#endif

    /// <summary>
    /// Represents a fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled
    /// and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<TFilter, TResult>
        : ICommandProcessorWithResults<TFilter>, ICommandProcessorInterop<TFilter>
#if !NETFX40
, ICommandProcessorAsync<TFilter, TResult>
#endif
    {
        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        IEnumerable<TResult> Execute(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo);

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        ICommandProcessor<TFilter, TResult> InterceptOrProxyWith(
            Func<ICommandProcessor<TFilter, TResult>, ICommandProcessor<TFilter, TResult>> interceptOrProxy); 

        /// <summary>
        /// Updates the column index information.
        /// </summary>
        /// <param name="info">The column checker.</param>
        void UpdateColumnIndexInfo(ColumnChecker info);

        /// <summary>
        /// Retrieves a copy of the column index info.
        /// </summary>
        ColumnChecker RetrieveColumnIndexInfo();

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        /// this is confusing and therefor is not allowed.;mode</exception>
        ICommandProcessor<TFilter, TResult> EnableResultCaching(CacheMode mode = CacheMode.Permanent,
            double? interval = null);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled
    /// and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<in TFilter, TResult1, TResult2>
        : ICommandProcessorWithResults<TFilter>, ICommandProcessorInterop<TFilter>
    {
        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2&gt;.</returns>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        /// this is confusing and therefor is not allowed.;mode</exception>
        ICommandProcessor<TFilter, TResult1, TResult2> EnableResultCaching(CacheMode mode = CacheMode.Permanent,
            double? interval = null);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;&gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
                params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled
    /// and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<in TFilter, TResult1, TResult2, TResult3>
        : ICommandProcessorWithResults<TFilter>, ICommandProcessorInterop<TFilter>
    {
        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        /// this is confusing and therefor is not allowed.;mode</exception>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3> EnableResultCaching(
            CacheMode mode = CacheMode.Permanent, double? interval = null);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;&gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
                params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled
    /// and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<in TFilter, TResult1, TResult2, TResult3, TResult4>
        : ICommandProcessorWithResults<TFilter>, ICommandProcessorInterop<TFilter>
    {
        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        /// this is confusing and therefor is not allowed.;mode</exception>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> EnableResultCaching(
            CacheMode mode = CacheMode.Permanent, double? interval = null);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;, IEnumerable&lt;TResult4&gt;&gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
                params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled
    /// and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<in TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
        : ICommandProcessorWithResults<TFilter>, ICommandProcessorInterop<TFilter>
    {
        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        /// this is confusing and therefor is not allowed.;mode</exception>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> EnableResultCaching(
            CacheMode mode = CacheMode.Permanent, double? interval = null);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;
        /// &gt;.</returns>
        Tuple
            <IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>,
                IEnumerable<TResult5>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;
        /// &gt;.</returns>
        Tuple
            <IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>,
                IEnumerable<TResult5>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;, IEnumerable&lt;TResult4&gt;, IEnumerable&lt;TResult5&gt;&gt;.</returns>
        Tuple
            <IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>,
                IEnumerable<TResult5>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
                params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled
    /// and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<in TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
        : ICommandProcessorWithResults<TFilter>, ICommandProcessorInterop<TFilter>
    {
        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        /// this is confusing and therefor is not allowed.;mode</exception>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> EnableResultCaching(
            CacheMode mode = CacheMode.Permanent, double? interval = null);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;
        /// &gt;.</returns>
        Tuple
            <IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>,
                IEnumerable<TResult5>, IEnumerable<TResult6>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;
        /// &gt;.</returns>
        Tuple
            <IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>,
                IEnumerable<TResult5>, IEnumerable<TResult6>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;, IEnumerable&lt;TResult4&gt;, IEnumerable&lt;TResult5&gt;, IEnumerable&lt;TResult6&gt;&gt;.</returns>
        Tuple
            <IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>,
                IEnumerable<TResult5>, IEnumerable<TResult6>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
                params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled
    /// and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
    /// <typeparam name="TResult7">The type of the 7th result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<in TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
        : ICommandProcessorWithResults<TFilter>, ICommandProcessorInterop<TFilter>
    {
        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        /// this is confusing and therefor is not allowed.;mode</exception>
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
            EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;,
        /// IEnumerable&lt;TResult7&gt;
        /// &gt;.</returns>
        Tuple
            <IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>,
                IEnumerable<TResult5>, IEnumerable<TResult6>, IEnumerable<TResult7>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;,
        /// IEnumerable&lt;TResult7&gt;
        /// &gt;.</returns>
        Tuple
            <IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>,
                IEnumerable<TResult5>, IEnumerable<TResult6>, IEnumerable<TResult7>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;, IEnumerable&lt;TResult4&gt;, IEnumerable&lt;TResult5&gt;, IEnumerable&lt;TResult6&gt;, IEnumerable&lt;TResult7&gt;&gt;.</returns>
        Tuple
            <IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>,
                IEnumerable<TResult5>, IEnumerable<TResult6>, IEnumerable<TResult7>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
                params DbParameter[] explicitParameters);
    }

}