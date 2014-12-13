#region

using System;
using System.ComponentModel;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Allows declarative specification of when reading and writing to properties is allowed.
    /// </summary>
    [ImmutableObject(true)]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AllowedActionsAttribute : Attribute
    {
        /// <summary>
        ///     Gets the allowed actions for a property.
        /// </summary>
        /// <value>
        ///     The actions allowed.
        /// </value>
        public DescriptorActions Actions { get; private set; }

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AllowedActionsAttribute" /> class.
        /// </summary>
        /// <param name="actions">The actions in which this property is allowed to participate.</param>
        public AllowedActionsAttribute(DescriptorActions actions)
        {
            Actions = actions;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AllowedActionsAttribute" /> class.
        /// </summary>
        public AllowedActionsAttribute()
            : this(DescriptorActions.Read | DescriptorActions.Insert | DescriptorActions.Update)
        {
        }

        #endregion Constructors
    }
}