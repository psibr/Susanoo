using System;
using Susanoo.Command;
using Susanoo.Mapping;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Exposes information to CommandBuilder Processors for result mapping.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandResultInfo<in TFilter>
        : ICommandResultInfo
    {
        /// <summary>
        /// Gets the command information.
        /// </summary>
        /// <value>The CommandBuilder information.</value>
        ICommandBuilderInfo<TFilter> GetCommandInfo();
    }

    /// <summary>
    /// Exposes information to CommandBuilder Processors for result mapping.
    /// </summary>
    public interface ICommandResultInfo
    {

        /// <summary>
        /// Retrieves the result set mappings.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <returns>IMappingExport.</returns>
        IMappingExport RetrieveResultSetMappings(Type resultType);
    }
}