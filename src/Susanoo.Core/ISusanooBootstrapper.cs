using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;
using Susanoo.Command;
using Susanoo.Deserialization;
using Susanoo.Pipeline;

namespace Susanoo
{
    /// <summary>
    /// Exposure points for extending or overriding Susanoo's behavior.
    /// </summary>
    public interface ISusanooBootstrapper
    {
        /// <summary>
        /// Resolves a type to a concrete implementation.
        /// </summary>
        /// <typeparam name="TDependency">The type of the  dependency.</typeparam>
        /// <returns>Dependency.</returns>
        TDependency ResolveDependency<TDependency>()
            where TDependency : class;

        /// <summary>
        /// Retrieves a set of attributes to use to determine when to ignore a property unless explicitly included.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Attribute&gt;.</returns>
        IEnumerable<Type> RetrieveIgnoredPropertyAttributes();
    }
}