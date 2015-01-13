#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
#if !NETFX40
using System.ComponentModel.DataAnnotations.Schema;
#endif

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Default implementation of IPropertyMetadataExtractor that uses Component Model ColumnAttributes to resolve
    ///     declarative aliases.
    /// </summary>
    public class ComponentModelMetadataExtractor : IPropertyMetadataExtractor
    {
        /// <summary>
        ///     Finds the properties on an object and resolves if they are actionable for mapping and discerns appropriate
        ///     declarative aliases.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="actions">The actions.</param>
        /// <param name="whitelist">The whitelist.</param>
        /// <param name="blacklist">The blacklist.</param>
        /// <returns>Dictionary&lt;PropertyInfo, PropertyMap&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">filterType</exception>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public Dictionary<PropertyInfo, PropertyMap> FindAllowedProperties(
            Type objectType,
            DescriptorActions actions = DescriptorActions.Read
                                        | DescriptorActions.Update
                                        | DescriptorActions.Insert,
            string[] whitelist = null,
            string[] blacklist = null)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType");

            var actionable = new Dictionary<PropertyInfo, PropertyMap>();

            foreach (PropertyInfo pi in objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                object[] attributes = pi.GetCustomAttributes(true);

                if (pi.CanWrite && IsActionableProperty(pi, attributes, actions))
                    actionable.Add(pi, new PropertyMap(pi, ResolveAlias(pi, attributes)));
            }

            return actionable;
        }

        /// <summary>
        ///     Resolves the name of the return column as defined declaratively.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="customAttributes">The custom attributes.</param>
        /// <returns>System.String.</returns>
        public virtual string ResolveAlias(PropertyInfo propertyInfo, object[] customAttributes)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            ColumnAttribute column = customAttributes != null
                ? customAttributes
                    .OfType<ColumnAttribute>()
                    .FirstOrDefault()
                : null;

            return column != null && !string.IsNullOrWhiteSpace(column.Name) ? column.Name : propertyInfo.Name;
        }

        /// <summary>
        ///     Determines whether the specified property is whitelisted.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="whitelist">The whitelist.</param>
        /// <returns><c>true</c> if the specified property information is whitelisted; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">propertyInfo</exception>
        public bool IsWhitelisted(PropertyInfo propertyInfo, string[] whitelist)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            return (whitelist != null && whitelist.Contains(propertyInfo.Name));
        }

        /// <summary>
        ///     Determines whether the specified property is blacklisted.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="blacklist">The blacklist.</param>
        /// <returns><c>true</c> if the specified property information is blacklisted; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">propertyInfo</exception>
        public bool IsBlacklisted(PropertyInfo propertyInfo, string[] blacklist)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            return (blacklist != null && blacklist.Contains(propertyInfo.Name));
        }

        /// <summary>
        ///     Determines whether the specified property is actionable.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="customAttributes">The custom attributes.</param>
        /// <param name="actions">The actions.</param>
        /// <param name="whitelist">The whitelist.</param>
        /// <param name="blacklist">The blacklist.</param>
        /// <returns><c>true</c> if [is actionable property] [the specified property information]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">
        ///     propertyInfo
        ///     or
        ///     customAttributes
        /// </exception>
        public virtual bool IsActionableProperty(
            PropertyInfo propertyInfo,
            object[] customAttributes,
            DescriptorActions actions = DescriptorActions.Read
                                        | DescriptorActions.Update
                                        | DescriptorActions.Insert,
            string[] whitelist = null,
            string[] blacklist = null)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (customAttributes == null)
                throw new ArgumentNullException("customAttributes");

            var attribute = customAttributes
                .OfType<AllowedActionsAttribute>()
                .FirstOrDefault();

            bool isActionable = IsWhitelisted(propertyInfo, whitelist)
                                || (!(IsBlacklisted(propertyInfo, blacklist))
                                    && IsAllowedByAttribute(propertyInfo, attribute, actions));

            return isActionable;
        }

        /// <summary>
        ///     Determines whether the specified property is restricted declaratively.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="actions">The actions.</param>
        /// <returns><c>true</c> if [is allowed by attribute] then [the specified property information]; otherwise, <c>false</c>.</returns>
        public virtual bool IsAllowedByAttribute(
            PropertyInfo propertyInfo,
            AllowedActionsAttribute attribute,
            DescriptorActions actions = DescriptorActions.Read
                                        | DescriptorActions.Update
                                        | DescriptorActions.Insert)
        {
            return attribute == null || (attribute.Actions & actions) != 0;
        }
    }
}