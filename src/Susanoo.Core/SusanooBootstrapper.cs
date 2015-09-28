using System;
using System.Collections.Generic;
using Susanoo.Command;
using Susanoo.DependencyInjection;
using Susanoo.DependencyInjection.TinyIoC;
#if !NETFX40
using System.ComponentModel.DataAnnotations.Schema;
#endif
using Susanoo.Deserialization;
using Susanoo.Pipeline;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo
{
    /// <summary>
    /// Provides all options for overriding Susanoo's behavior.
    /// </summary>
    public class SusanooBootstrapper 
        : ISusanooBootstrapper
    {
        /// <summary>
        /// The TinyIoC container used by default for building components.
        /// </summary>
        protected readonly IContainer Container;

        /// <summary>
        /// Instantiates the bootstrapper and wires all dependencies to the resolve method. Uses TinyIoC container by default.
        /// </summary>
        public SusanooBootstrapper()
            : this(new TinyIoCContainerAdapter(TinyIoCContainer.Current))
        {

        }

        /// <summary>
        /// Instantiates the bootstrapper and wires all dependencies to the resolve method.
        /// </summary>
        public SusanooBootstrapper(IContainer container)
        {
            Container = container;

            RegisterTypeChain();
        }

        /// <summary>
        /// Registers the type chain for all types in Susanoo.
        /// </summary>
        protected void RegisterTypeChain()
        {
            Container.Register<IPropertyMetadataExtractor>(new ComponentModelMetadataExtractor());

            Container.Register<IDeserializerResolver>(new DeserializerResolver(
                new KeyValuePairDeserializerFactory()));

            Container.Register<IDatabaseManagerFactory>(new DatabaseManagerFactory());

            //Processors

            //No need for a factory method here since NoResult is also NoDependency
            Container.Register<INoResultSetCommandProcessorFactory>(new NoResultSetCommandProcessorFactory());

            Container.Register<ISingleResultSetCommandProcessorFactory>((container)
                => new SingleResultSetCommandProcessorFactory(container.Resolve<IDeserializerResolver>()));

            Container.Register<IMultipleResultSetCommandProcessorFactory>((container)
                => new MultipleResultSetCommandProcessorFactory(container.Resolve<IDeserializerResolver>()));

            //Result Mapping
            Container.Register<ICommandMultipleResultExpressionFactory>((container)
                => new CommandMultipleResultExpressionFactory(
                    container.Resolve<IPropertyMetadataExtractor>(),
                    container.Resolve<IMultipleResultSetCommandProcessorFactory>()));

            Container.Register<ICommandSingleResultExpressionFactory>((container)
                => new CommandSingleResultExpressionFactory(
                    container.Resolve<IPropertyMetadataExtractor>(),
                    container.Resolve<ISingleResultSetCommandProcessorFactory>()));

            //Command Expression
            Container.Register<ICommandExpressionFactory>((container)
                => new CommandExpressionFactory(
                    container.Resolve<IPropertyMetadataExtractor>(),
                    container.Resolve<INoResultSetCommandProcessorFactory>(),
                    container.Resolve<ICommandMultipleResultExpressionFactory>(),
                    container.Resolve<ICommandSingleResultExpressionFactory>()));

            Container.Register<ICommandBuilder>((container)
                => new CommandBuilder(container.Resolve<CommandExpressionFactory>()));
        }

        /// <summary>
        /// Retrieves a set of attributes to use to determine when to ignore a property unless explicitly included.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Attribute&gt;.</returns>
        public virtual IEnumerable<Type> RetrieveIgnoredPropertyAttributes()
        {
            return new[] { typeof(NotMappedAttribute) };
        }

        /// <summary>
        /// Resolves a command builder.
        /// </summary>
        /// <returns>ICommandBuilder.</returns>
        public ICommandBuilder ResolveCommandBuilder(string name = null)
        {
            return Container.Resolve<ICommandBuilder>(name);
        }

        /// <summary>
        /// Resolves a database manager factory.
        /// </summary>
        /// <returns>IDatabaseManagerFactory.</returns>
        public IDatabaseManagerFactory ResolveDatabaseManagerFactory(string name = null)
        {
            return Container.Resolve<IDatabaseManagerFactory>(name);
        }
    }
}