using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Susanoo
{
    /// <summary>
    /// Represents basic information about a property including its reflection information and alias.
    /// </summary>
    /// <remarks>Similar to IPropertyMappingConfiguration, but much lighter and represents a property before the mapping compilation tasks.</remarks>
    public class PropertyMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMap" /> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="alias">The alias.</param>
        /// <exception cref="System.ArgumentNullException">property</exception>
        public PropertyMap(PropertyInfo property, string alias)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            Contract.EndContractBlock();

            this.Property = property;
            this.Alias = (!string.IsNullOrWhiteSpace(alias)) ? alias : property.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMap" /> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <exception cref="System.ArgumentNullException">property</exception>
        public PropertyMap(PropertyInfo property)
            : this(property, null)
        {
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>The property information.</value>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <value>The alias.</value>
        public string Alias { get; private set; }
    }
}