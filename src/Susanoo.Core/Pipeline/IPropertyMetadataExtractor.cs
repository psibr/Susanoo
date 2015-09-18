#region

using System;
using System.Collections.Generic;
using System.Reflection;
using Susanoo.Mapping.Properties;

#endregion

namespace Susanoo.Pipeline
{
    /// <summary>
    /// Describes the required methods for determining if a property can be mapped using Susanoo.
    /// </summary>
    public interface IPropertyMetadataExtractor
    {
        /// <summary>
        /// Finds the properties on an object and resolves if they are actionable for mapping and discerns appropriate
        /// declarative aliases.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="actions">The actions.</param>
        /// <param name="whitelist">The whitelist.</param>
        /// <param name="blacklist">The blacklist.</param>
        /// <returns>Dictionary&lt;PropertyInfo, PropertyMap&gt;.</returns>
        Dictionary<PropertyInfo, PropertyMapping> FindAllowedProperties(
            Type objectType, DescriptorActions actions = DescriptorActions.Read,
            string[] whitelist = null,
            string[] blacklist = null);

        /// <summary>
        /// Determines whether the specified property is blacklisted.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="blacklist">The blacklist.</param>
        /// <returns><c>true</c> if the specified property information is blacklisted; otherwise, <c>false</c>.</returns>
        bool IsBlacklisted(PropertyInfo propertyInfo, string[] blacklist);

        /// <summary>
        /// Determines whether the specified property is whitelisted.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="whitelist">The whitelist.</param>
        /// <returns><c>true</c> if the specified property information is whitelisted; otherwise, <c>false</c>.</returns>
        bool IsWhitelisted(PropertyInfo propertyInfo, string[] whitelist);

        /// <summary>
        /// Resolves the name of the return column as defined declaratively.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="customAttributes">The custom attributes.</param>
        /// <returns>System.String.</returns>
        string ResolveAlias(PropertyInfo propertyInfo, object[] customAttributes);
    }
}