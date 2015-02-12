#region

using System;

#endregion

namespace Susanoo
{
    /// <summary>
    /// Indicates which actions are allowed for a property.
    /// </summary>
    [Flags]
    public enum DescriptorActions
    {
        /// <summary>
        /// Indicates no actions allowed
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Indicates read action allowed
        /// </summary>
        Read = 0x01,

        /// <summary>
        /// Indicates update action allowed
        /// </summary>
        Update = 0x02,

        /// <summary>
        /// Indicates insert action allowed
        /// </summary>
        Insert = 0x04,

        /// <summary>
        /// Indicates delete action allowed
        /// </summary>
        Delete = 0x08
    }
}