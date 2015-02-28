using System;
using System.Numerics;

namespace Susanoo.Pipeline.Command
{
    /// <summary>
    /// Describes and places a priority ranking on a modification of a command.
    /// </summary>
    public class CommandModifier :
        IFluentPipelineFragment
    {
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the modifier function.
        /// </summary>
        /// <value>The modifier function.</value>
        public Func<IExecutableCommandInfo, IExecutableCommandInfo> ModifierFunc { get; set; }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash { get; set; }
    }
}