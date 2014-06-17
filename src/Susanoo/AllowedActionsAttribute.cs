using System;
using System.ComponentModel;

namespace Susanoo
{
    [ImmutableObject(true)]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AllowedActionsAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowedActionsAttribute"/> class.
        /// </summary>
        /// <param name="actions">The actions in which this property is allowed to participate.</param>
        public AllowedActionsAttribute(Susanoo.DescriptorActions actions)
        {
            this.Actions = actions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowedActionsAttribute"/> class.
        /// </summary>
        public AllowedActionsAttribute()
            : this(Susanoo.DescriptorActions.Read | Susanoo.DescriptorActions.Insert | Susanoo.DescriptorActions.Update)
        {
        }

        #endregion Constructors

        /// <summary>
        /// Gets the allowed actions for a property.
        /// </summary>
        /// <value>
        /// The actions allowed.
        /// </value>
        public DescriptorActions Actions { get; private set; }
    }
}