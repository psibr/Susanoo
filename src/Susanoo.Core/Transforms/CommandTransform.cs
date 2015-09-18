using System;
using Susanoo.Command;

namespace Susanoo.Transforms
{
    /// <summary>
    /// Describes and contains a transform operation for IExecutableCommandInfo.
    /// </summary>
    public class CommandTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTransform"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="transform">The transform.</param>
        public CommandTransform(string description, Func<IExecutableCommandInfo, IExecutableCommandInfo> transform)
        {
            Description = description;
            Transform = transform;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; }

        /// <summary>
        /// Gets or sets the modifier function.
        /// </summary>
        /// <value>The modifier function.</value>
        public Func<IExecutableCommandInfo, IExecutableCommandInfo> Transform { get; }
    }
}
