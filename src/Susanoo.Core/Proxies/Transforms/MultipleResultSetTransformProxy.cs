using System.Collections.Generic;
using System.Linq;
using Susanoo.Command;
using Susanoo.Processing;
using System;
#if !NETFX40
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Susanoo.Proxies.Transforms
{
    /// <summary>
    /// A proxy for Multiple result command processors that allows transforms to be applied prior to execution.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class MultipleResultSetTransformProxy<TFilter>
            : MultipleResultSetProxy<TFilter>
    {
        private readonly IEnumerable<CommandTransform> _transforms;
        private readonly Action<IExecutableCommandInfo> _queryInspector;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResultSetTransformProxy{TFilter}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="transforms">The transforms.</param>
        public MultipleResultSetTransformProxy(IMultipleResultSetCommandProcessor<TFilter> source,
            IEnumerable<CommandTransform> transforms,
            Action<IExecutableCommandInfo> queryInspector = null)
            : base(source)
        {
            _transforms = transforms;
            _queryInspector = queryInspector ?? new Action<IExecutableCommandInfo>((info) => { });
        }

        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>IResultSetCollection.</returns>
        /// <exception cref="SusanooExecutionException">Any exception occured during execution.</exception>
        public override IResultSetCollection Execute(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            _queryInspector(transformed);

            return Source.Execute(databaseManager, transformed);
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
        public override async Task<IResultSetCollection> ExecuteAsync(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo,
            CancellationToken cancellationToken)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            _queryInspector(transformed);

            return await Source.ExecuteAsync(databaseManager, transformed, cancellationToken);
        }
#endif
    }
}
