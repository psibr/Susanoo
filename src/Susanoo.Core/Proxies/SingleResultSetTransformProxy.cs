using System;
using System.Collections.Generic;
using System.Linq;
using Susanoo.Command;
using Susanoo.Processing;
#if !NETFX40
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Susanoo.Proxies
{
    /// <summary>
    /// A proxy for single result command processors that allows transforms to be applied prior to execution.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class SingleResultSetTransformProxy<TFilter, TResult>
            : SingleResultSetProxy<TFilter, TResult>
    {
        private readonly IEnumerable<CommandTransform> _transforms;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResultSetTransformProxy{TFilter,TResult}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="transforms">The transforms.</param>
        public SingleResultSetTransformProxy(ISingleResultSetCommandProcessor<TFilter, TResult> source,
            IEnumerable<CommandTransform> transforms)
            : base(source)
        {
            _transforms = transforms;
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public override IEnumerable<TResult> Execute(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            return Source.Execute(databaseManager, transformed);
        }

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public override ISingleResultSetCommandProcessor<TFilter, TResult> InterceptOrProxyWith(Func<ISingleResultSetCommandProcessor<TFilter, TResult>, ISingleResultSetCommandProcessor<TFilter, TResult>> interceptOrProxy)
        {
            return interceptOrProxy(this);
        }

#if !NETFX40
        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public override async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo,
            CancellationToken cancellationToken)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            return await Source.ExecuteAsync(databaseManager, transformed, cancellationToken)
                .ConfigureAwait(false);
        }
#endif
    }
}
