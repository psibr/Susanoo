using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;

namespace Susanoo
{
    public class CommandResultMappingExpression<TFilter, TResult>
        : ICommandResultMappingExpression<TFilter, TResult>
        where TResult : new()
    {
        private readonly ReaderWriterLockSlim threadSync = new ReaderWriterLockSlim();
        private readonly IDictionary<string, Action<IPropertyMappingConfiguration<IDataRecord>>> mappingActions = new Dictionary<string, Action<IPropertyMappingConfiguration<IDataRecord>>>();

        public CommandResultMappingExpression(ICommandExpression<TFilter, TResult> commandExpression)
        {
            this.CommandExpression = commandExpression;

            MapDeclarativeProperties();
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter, TResult> CommandExpression { get; private set; }

        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandResultMappingExpression<TFilter, TResult> ClearMappings()
        {
            this.threadSync.EnterWriteLock();
            this.mappingActions.Clear();
            this.threadSync.ExitWriteLock();

            return this;
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="options">The options.</param>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public ICommandResultMappingExpression<TFilter, TResult> ForProperty(Expression<Func<TResult, object>> propertyExpression, Action<IPropertyMappingConfiguration<IDataRecord>> options)
        {
            return ForProperty(propertyExpression.GetPropertyName(), options);
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandResultMappingExpression<TFilter, TResult> ForProperty(string propertyName, Action<IPropertyMappingConfiguration<IDataRecord>> options)
        {
            this.threadSync.EnterWriteLock();
            this.mappingActions.Add(propertyName, options);
            this.threadSync.ExitWriteLock();

            return this;
        }

        /// <summary>
        /// Prepares the command for caching and executing.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public ICommandProcessor<TFilter, TResult> PrepareCommand()
        {
            return new CommandProcessor<TFilter, TResult>(this);
        }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IDictionary<string, IPropertyMappingConfiguration<IDataRecord>> Export()
        {
            var exportDictionary = new Dictionary<string, IPropertyMappingConfiguration<IDataRecord>>();

            foreach (var item in this.mappingActions)
            {
                var config = new PropertyMappingConfiguration<IDataRecord>(typeof(TResult).GetProperty(item.Key));
                item.Value.Invoke(config);

                exportDictionary.Add(item.Key, config);
            }

            return exportDictionary;
        }

        /// <summary>
        /// Maps the declarative properties.
        /// </summary>
        protected virtual void MapDeclarativeProperties()
        {
            foreach (var item in CommandManager.Instance.Container.Resolve<IPropertyMetadataExtractor>()
                .FindPropertiesFromFilter(typeof(TResult), Susanoo.DescriptorActions.Read))
            {
                mappingActions.Add(item.Key.Name, o => o.AliasProperty(item.Value.DatabaseName));
            }
        }
    }
}