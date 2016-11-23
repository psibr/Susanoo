#region

using System;
using System.Collections.Generic;


using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Susanoo.Mapping.Properties;
#if !NETFX40 && !DOTNETCORE
using System.ComponentModel.DataAnnotations.Schema;
#endif

#endregion

namespace Susanoo
{
    /// <summary>
    /// Default implementation of IPropertyMetadataExtractor that uses Component Model ColumnAttributes to resolve
    /// declarative aliases.
    /// </summary>
    public class ComponentModelMetadataExtractor : IPropertyMetadataExtractor
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
        /// <exception cref="System.ArgumentNullException">filterType</exception>
        /// <exception cref="TypeLoadException">A custom attribute type could not be loaded. </exception>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public Dictionary<PropertyInfo, PropertyMapping> FindAllowedProperties(
            TypeInfo objectType, DescriptorActions actions = DescriptorActions.Read,
            string[] whitelist = null,
            string[] blacklist = null)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            var actionable = new Dictionary<PropertyInfo, PropertyMapping>();

            var ignoreIsWritable = !actions.HasFlag(DescriptorActions.Read);

            foreach (var pi in objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attributes = pi.GetCustomAttributes(true);

                if ((ignoreIsWritable || pi.CanWrite) && IsActionableProperty(pi, attributes, actions))
                    actionable.Add(pi, new PropertyMapping(pi, ResolveAlias(pi, attributes)));
            }

            return actionable;
        }

        /// <summary>
        /// Resolves the name of the return column as defined declaratively.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="customAttributes">The custom attributes.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">propertyInfo</exception>
        public virtual string ResolveAlias(PropertyInfo propertyInfo,
#if !DOTNETCORE
            object[] customAttributes
#else
            IEnumerable<Attribute> customAttributes
#endif
        )
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var column = customAttributes?.OfType<ColumnAttribute>()
                .FirstOrDefault();

            return !string.IsNullOrWhiteSpace(column?.Name) ? column.Name : propertyInfo.Name;
        }

        /// <summary>
        /// Determines whether the specified property is whitelisted.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="whitelist">The whitelist.</param>
        /// <returns><c>true</c> if the specified property information is whitelisted; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">propertyInfo</exception>
        public bool IsWhitelisted(PropertyInfo propertyInfo, string[] whitelist)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            return (whitelist != null && whitelist.Contains(propertyInfo.Name));
        }

        /// <summary>
        /// Determines whether the specified property is blacklisted.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="blacklist">The blacklist.</param>
        /// <returns><c>true</c> if the specified property information is blacklisted; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">propertyInfo</exception>
        public bool IsBlacklisted(PropertyInfo propertyInfo, string[] blacklist)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            return (blacklist != null && blacklist.Contains(propertyInfo.Name));
        }

        /// <summary>
        /// Determines whether the specified property is actionable.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="customAttributes">The custom attributes.</param>
        /// <param name="actions">The actions.</param>
        /// <param name="whitelist">The whitelist.</param>
        /// <param name="blacklist">The blacklist.</param>
        /// <returns><c>true</c> if [is actionable property] [the specified property information]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">propertyInfo
        /// or
        /// customAttributes</exception>
        protected virtual bool IsActionableProperty(
            PropertyInfo propertyInfo,
#if !DOTNETCORE
            object[] customAttributes,
#else
            IEnumerable<Attribute> customAttributes,
#endif

            DescriptorActions actions = DescriptorActions.Read,
            string[] whitelist = null,
            string[] blacklist = null)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));
            if (customAttributes == null)
                throw new ArgumentNullException(nameof(customAttributes));

            var isActionable = IsWhitelisted(propertyInfo, whitelist)
                                || (!(IsBlacklisted(propertyInfo, blacklist))
                                    && IsAllowedByAttributes(propertyInfo, customAttributes, actions));

            return isActionable;
        }

        /// <summary>
        /// Determines whether the specified property is restricted declaratively.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="actions">The actions.</param>
        /// <returns><c>true</c> if [is allowed by attribute] then [the specified property information]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsAllowedByAttributes(
            PropertyInfo propertyInfo,
#if !DOTNETCORE
            object[] attributes,
#else
            IEnumerable<Attribute> attributes,
#endif
            DescriptorActions actions)
        {
            var result = true;

            if (attributes.Count() > 0)
            {
                var allowedActions = attributes.OfType<AllowedActionsAttribute>().FirstOrDefault();
                result = (allowedActions == null || (allowedActions.Actions & actions) != 0);

                result = result && !attributes.Any(a => SusanooCommander.Instance.Bootstrapper.RetrieveIgnoredPropertyAttributes()
                    .Contains(a.GetType()));
            }

            return result;
        }
    }
}