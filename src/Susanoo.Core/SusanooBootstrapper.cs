using System;
using System.Collections.Generic;

#if !NETFX40
using System.ComponentModel.DataAnnotations.Schema;
#endif
using Susanoo.Deserialization;
using Susanoo.Exceptions;
using Susanoo.Pipeline;
using Susanoo.Processing;
using Susanoo.TinyIoC;

namespace Susanoo
{
    /// <summary>
    /// Provides all options for overriding Susanoo's behavior.
    /// </summary>
    public class SusanooBootstrapper : ISusanooBootstrapper
    {
        /// <summary>
        /// The TinyIoC container used by default for building components.
        /// </summary>
        protected readonly TinyIoCContainer DIContainer = TinyIoCContainer.Current;

        /// <summary>
        /// Instantiates the bootstrapper and wires all dependencies to the resolve method.
        /// </summary>
        public SusanooBootstrapper()
        {
            #region Factory Registrations

            DIContainer.Register<IDatabaseManagerFactory>(new DatabaseManagerFactory());

            DIContainer.Register<INoResultSetCommandProcessorFactory>(new NoResultSetCommandProcessorFactory());
            DIContainer.Register<ISingleResultSetCommandProcessorFactory>(new SingleResultSetCommandProcessorFactory());
            DIContainer.Register<IMultipleResultSetCommandProcessorFactory>(
                new MultipleResultSetCommandProcessorFactory());

            #endregion Factory Registrations

            #region Service Registrations

            DIContainer.Register<IDeserializerResolver>(new DeserializerResolver(
                new KeyValuePairDeserializerFactory()));

            DIContainer.Register<ICommandBuilder>(new CommandBuilder());
            DIContainer.Register<IPropertyMetadataExtractor>(new ComponentModelMetadataExtractor());

            #endregion Service Registrations
        }

        /// <summary>
        /// Resolves a type to a concrete implementation.
        /// </summary>
        /// <typeparam name="TDependency">The type of the  dependency.</typeparam>
        /// <exception cref="SusanooDependencyResolutionException"></exception>
        /// <returns>Dependency.</returns>
        public virtual TDependency ResolveDependency<TDependency>()
            where TDependency : class
        {
            try
            {
                return DIContainer.Resolve<TDependency>();
            }
            catch (TinyIoCResolutionException ex)
            {
                throw new SusanooDependencyResolutionException("Unable to resolve a dependency.", ex);
            }
        }

        /// <summary>
        /// Retrieves a set of attributes to use to determine when to ignore a property unless explicitly included.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Attribute&gt;.</returns>
        public virtual IEnumerable<Type> RetrieveIgnoredPropertyAttributes()
        {
            return new[] { typeof(NotMappedAttribute) };
        }
    }
}