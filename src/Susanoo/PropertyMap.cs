using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Susanoo
{
    public class PropertyMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMap" /> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="databaseName">Name of the return column.</param>
        /// <exception cref="System.ArgumentNullException">property</exception>
        public PropertyMap(PropertyInfo property, string databaseName)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            Contract.EndContractBlock();

            this.Property = property;
            this.DatabaseName = (!string.IsNullOrWhiteSpace(databaseName)) ? databaseName : property.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMap" /> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="databaseName">Name of the return column.</param>
        /// <exception cref="System.ArgumentNullException">property</exception>
        public PropertyMap(PropertyInfo property)
            : this(property, null)
        {
        }

        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Gets the name of the return column.
        /// </summary>
        /// <value>The name of the return column.</value>
        public string DatabaseName { get; private set; }
    }
}