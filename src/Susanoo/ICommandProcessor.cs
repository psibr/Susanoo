using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Susanoo
{
    /// <summary>
    /// Shared members for all command processors.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandProcessorInterop<TFilter> : IFluentPipelineFragment
    {
        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        ICommandExpression<TFilter> CommandExpression { get; }
    }

    /// <summary>
    /// Represents a fully built and ready to be executed command expression with no mapping expressions and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandProcessor<TFilter> : ICommandProcessorInterop<TFilter>
    {
        
        /// <summary>
        /// Executes the command and retrieves a single value.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the command and retrieves a single value.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the command and returns a return code.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the command and returns a return code.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteNonQuery(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<TFilter, TResult>
        : ICommandProcessorInterop<TFilter>, IFluentPipelineFragment
        where TResult : new()
    {
        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        IEnumerable<TResult> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<TFilter, TResult1, TResult2>
        : ICommandProcessorInterop<TFilter>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
    {
        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;
        /// &gt;.
        /// </returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<TFilter, TResult1, TResult2, TResult3>
        : ICommandProcessorInterop<TFilter>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
    {
        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;
        /// &gt;.
        /// </returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>
        : ICommandProcessorInterop<TFilter>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
    {
        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;
        /// &gt;.
        /// </returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
        : ICommandProcessorInterop<TFilter>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
    {
        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;
        /// &gt;.
        /// </returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>, IEnumerable<TResult5>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>, IEnumerable<TResult5>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public interface ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
        : ICommandProcessorInterop<TFilter>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
    {
        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;
        /// &gt;.
        /// </returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>, IEnumerable<TResult5>, IEnumerable<TResult6>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>, IEnumerable<TResult5>, IEnumerable<TResult6>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }

    /// <summary>
    /// Represents a fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
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
    public interface ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
        : ICommandProcessorInterop<TFilter>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
        where TResult7 : new()
    {
        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;,
        /// IEnumerable&lt;TResult7&gt;
        /// &gt;.
        /// </returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>, IEnumerable<TResult5>, IEnumerable<TResult6>, IEnumerable<TResult7>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Assembles a data command for an ADO.NET provider,
        /// executes the command and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        /// Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;,
        /// IEnumerable&lt;TResult7&gt;
        /// &gt;.</returns>
        Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>, IEnumerable<TResult5>, IEnumerable<TResult6>, IEnumerable<TResult7>>
            Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }
}