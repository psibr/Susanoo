#region

using System;
using System.Reflection;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Represents basic information about a property including its reflection information and alias.
    /// </summary>
    public class PropertyMap
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyMap" /> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="alias">The alias.</param>
        /// <exception cref="System.ArgumentNullException">property</exception>
        public PropertyMap(PropertyInfo property, string alias)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            PropertyMetadata = property;
            ActiveAlias = (!string.IsNullOrWhiteSpace(alias)) ? alias : property.Name;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyMap" /> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <exception cref="System.ArgumentNullException">property</exception>
        public PropertyMap(PropertyInfo property)
            : this(property, null)
        {
        }

        /// <summary>
        ///     Gets the property information.
        /// </summary>
        /// <value>The property information.</value>
        public PropertyInfo PropertyMetadata { get; private set; }

        /// <summary>
        ///     Gets the alias.
        /// </summary>
        /// <value>The alias.</value>
        public string ActiveAlias { get; private set; }
    }
}