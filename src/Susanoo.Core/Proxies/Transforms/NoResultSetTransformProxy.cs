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
    /// A proxy for no result set command processors that allows transforms to be applied prior to execution.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class NoResultSetTransformProxy<TFilter>
        : NoResultSetProxy<TFilter>
    {
        private readonly IEnumerable<CommandTransform> _transforms;
        private readonly Action<IExecutableCommandInfo> _queryInspector;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoResultSetTransformProxy{TFilter}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="transforms">The transforms.</param>
        public NoResultSetTransformProxy(INoResultCommandProcessor<TFilter> source, IEnumerable<CommandTransform> transforms,
            Action<IExecutableCommandInfo> queryInspector = null)
            : base(source)
        {
            _transforms = transforms;
            _queryInspector = queryInspector ?? new Action<IExecutableCommandInfo>((info) => { });
        }

        /// <summary>
        ///     Executes the non query.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>System.Int32.</returns>
        public override int ExecuteNonQuery(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            _queryInspector(transformed);

            return Source.ExecuteNonQuery(databaseManager, transformed);
        }

        /// <summary>
        ///     Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>TReturn.</returns>
        public override TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            _queryInspector(transformed);

            return Source.ExecuteScalar<TReturn>(databaseManager, transformed);
        }

#if !NETFX40

        /// <summary>
        ///     Executes the non query asynchronously.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Int32&gt;.</returns>
        public override async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            _queryInspector(transformed);

            return await Source.ExecuteNonQueryAsync(databaseManager, transformed, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Executes the scalar asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;TReturn&gt;.</returns>
        public override async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            _queryInspector(transformed);

            return await Source.ExecuteScalarAsync<TReturn>(databaseManager, transformed, cancellationToken)
                .ConfigureAwait(false);
        }
#endif

    }
}
