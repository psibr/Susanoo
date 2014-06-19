using System;
using System.Collections.Generic;
using System.Reflection;

namespace Susanoo
{
    /// <summary>
    /// Describes the required methods for determining if a property can be mapped using Susanoo.
    /// </summary>
    public interface IPropertyMetadataExtractor
    {
        /// <summary>
        /// Finds the properties on an object and resolves if they are actionable for mapping and discerns appropriate declarative aliases.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="actions">The actions.</param>
        /// <param name="whitelist">The whitelist.</param>
        /// <param name="blacklist">The blacklist.</param>
        /// <returns>Dictionary&lt;PropertyInfo, PropertyMap&gt;.</returns>
        Dictionary<PropertyInfo, PropertyMap> FindAllowedProperties(
            Type objectType,
            Susanoo.DescriptorActions actions = Susanoo.DescriptorActions.Read
                | Susanoo.DescriptorActions.Update
                | Susanoo.DescriptorActions.Insert,
            string[] whitelist = null,
            string[] blacklist = null);

        /// <summary>
        /// Determines whether the specified property is actionable.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="customAttributes">The custom attributes.</param>
        /// <param name="actions">The actions.</param>
        /// <param name="whitelist">The whitelist.</param>
        /// <param name="blacklist">The blacklist.</param>
        /// <returns><c>true</c> if the property is actionable; otherwise, <c>false</c>.</returns>
        bool IsActionableProperty(
            PropertyInfo propertyInfo,
            object[] customAttributes,
            Susanoo.DescriptorActions actions = Susanoo.DescriptorActions.Read
                | Susanoo.DescriptorActions.Update
                | Susanoo.DescriptorActions.Insert,
            string[] whitelist = null,
            string[] blacklist = null);

        /// <summary>
        /// Determines whether the specified property is restricted declaratively.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="actions">The actions.</param>
        /// <returns><c>true</c> if the property is not declaratively restricted; otherwise, <c>false</c>.</returns>
        bool IsAllowedByAttribute(
            PropertyInfo propertyInfo,
            AllowedActionsAttribute attribute,
            Susanoo.DescriptorActions actions = Susanoo.DescriptorActions.Read
                | Susanoo.DescriptorActions.Update
                | Susanoo.DescriptorActions.Insert);

        /// <summary>
        /// Determines whether the specified property is blacklisted.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="blacklist">The blacklist.</param>
        bool IsBlacklisted(PropertyInfo propertyInfo, string[] blacklist);

        /// <summary>
        /// Determines whether the specified property is whitelisted.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="whitelist">The whitelist.</param>
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