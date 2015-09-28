using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Core;
using Susanoo.Deserialization;
using Susanoo.Exceptions;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Tests
{
    public class InterceptedSusanooBootstrapper : SusanooBootstrapper
    {
        public InterceptedSusanooBootstrapper()
        {
            Container.Register<ISingleResultSetCommandProcessorFactory>((container)
                => new InterceptedSingleResultSetFactory(container.Resolve<IDeserializerResolver>()));
        }
    }

    public class InterceptedSingleResultSetFactory
        : SingleResultSetCommandProcessorFactory
    {
        public InterceptedSingleResultSetFactory(IDeserializerResolver deserializerResolver)
            : base(deserializerResolver)
        {
        }

        /// <summary>
        /// Builds the command processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <param name="name">The name.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public override ISingleResultSetCommandProcessor<TFilter, TResult> BuildCommandProcessor<TFilter, TResult>(ICommandResultInfo<TFilter> mappings, string name = null)
            => base.BuildCommandProcessor<TFilter, TResult>(mappings, name)
                .InterceptExceptions(ex => Debug.Print($"{ex.Info.CommandText}"));
    }
}

