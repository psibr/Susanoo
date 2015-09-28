using System;
using System.Collections.Generic;
using Susanoo.Pipeline;

namespace Susanoo
{
    /// <summary>
    /// Exposure points for extending or overriding Susanoo's behavior.
    /// </summary>
    public interface ISusanooBootstrapper
    {
        /// <summary>
        /// Retrieves a set of attributes to use to determine when to ignore a property unless explicitly included.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Attribute&gt;.</returns>
        IEnumerable<Type> RetrieveIgnoredPropertyAttributes();

        /// <summary>
        /// Creates a command builder.
        /// </summary>
        /// <returns>ICommandBuilder.</returns>
        ICommandBuilder ResolveCommandBuilder(string name = null);

        /// <summary>
        /// Resolves a database manager factory.
        /// </summary>
        /// <returns>IDatabaseManagerFactory.</returns>
        IDatabaseManagerFactory ResolveDatabaseManagerFactory(string name = null);
    }
}