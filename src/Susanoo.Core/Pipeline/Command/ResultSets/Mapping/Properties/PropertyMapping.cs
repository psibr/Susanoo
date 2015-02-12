#region

using System;
using System.Reflection;

#endregion

namespace Susanoo.Pipeline.Command.ResultSets.Mapping.Properties
{
    /// <summary>
    /// Represents basic information about a property including its reflection information and alias.
    /// </summary>
    public class PropertyMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapping" /> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="alias">The alias.</param>
        /// <exception cref="System.ArgumentNullException">property</exception>
        public PropertyMapping(PropertyInfo property, string alias = null)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            PropertyMetadata = property;
            ActiveAlias = (!string.IsNullOrWhiteSpace(alias)) ? alias : property.Name;
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>The property information.</value>
        public PropertyInfo PropertyMetadata { get; private set; }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <value>The alias.</value>
        public string ActiveAlias { get; private set; }
    }
}