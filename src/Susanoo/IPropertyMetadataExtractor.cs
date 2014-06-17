using System;
using System.Collections.Generic;
using System.Reflection;

namespace Susanoo
{
    public interface IPropertyMetadataExtractor
    {
        /// <summary>
        /// Finds the parameters from a filter by type and resolves if they are actionable and declarative aliases.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">The filter.</param>
        /// <param name="actions">The actions which qualify the search for properties. Default: Read, Update, and Insert</param>
        /// <param name="whitelist">The white list. Default: null</param>
        /// <param name="blacklist">The black list. Default: null</param>
        Dictionary<PropertyInfo, PropertyMap> FindPropertiesFromFilter(
            Type filterType,
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
        /// <returns><c>true</c> if [is allowed by attribute] then [the specified property information]; otherwise, <c>false</c>.</returns>
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
        string ResolveReturnName(PropertyInfo propertyInfo, object[] customAttributes);
    }
}