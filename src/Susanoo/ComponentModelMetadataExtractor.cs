using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Susanoo
{
    /// <summary>
    /// Default implementation of IPropertyMetadataExtractor that uses Component Model ColumnAttributes to resolve declarative aliases.
    /// </summary>
    public class ComponentModelMetadataExtractor : IPropertyMetadataExtractor
    {
        /// <summary>
        /// Finds the parameters from a filter by type and resolves if they are actionable and declarative aliases.
        /// </summary>
        /// <param name="filterType">Type of the filter.</param>
        /// <param name="actions">The actions which qualify the search for properties. Default: Read, Update, and Insert</param>
        /// <param name="whitelist">The white list. Default: null</param>
        /// <param name="blacklist">The black list. Default: null</param>
        /// <returns>Dictionary&lt;PropertyInfo, PropertyMap&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">filterType</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public Dictionary<PropertyInfo, PropertyMap> FindPropertiesFromFilter(
            Type filterType,
            Susanoo.DescriptorActions actions = Susanoo.DescriptorActions.Read 
                | Susanoo.DescriptorActions.Update 
                | Susanoo.DescriptorActions.Insert,
            string[] whitelist = null,
            string[] blacklist = null)
        {
            if (filterType == null)
                throw new ArgumentNullException("filterType");

            Contract.EndContractBlock();

            Dictionary<PropertyInfo, PropertyMap> Actionable = new Dictionary<PropertyInfo, PropertyMap>();

            foreach (PropertyInfo PI in filterType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                object[] Attributes = PI.GetCustomAttributes(true);

                if (PI.CanWrite && IsActionableProperty(PI, Attributes, actions))
                    Actionable.Add(PI, new PropertyMap(PI, ResolveReturnName(PI, Attributes)));
            }

            return Actionable;
        }

        /// <summary>
        /// Resolves the name of the return column as defined declaratively.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="customAttributes">The custom attributes.</param>
        /// <returns>System.String.</returns>
        public virtual string ResolveReturnName(PropertyInfo propertyInfo, object[] customAttributes)
        {
            ColumnAttribute Column = customAttributes
                .OfType<ColumnAttribute>()
                .FirstOrDefault();

            return Column != null && !string.IsNullOrWhiteSpace(Column.Name) ? Column.Name : propertyInfo.Name;
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
                throw new ArgumentNullException("propertyInfo");

            Contract.EndContractBlock();

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
                throw new ArgumentNullException("propertyInfo");

            Contract.EndContractBlock();

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
        /// <exception cref="System.ArgumentNullException">
        /// propertyInfo
        /// or
        /// customAttributes
        /// </exception>
        public virtual bool IsActionableProperty(
            PropertyInfo propertyInfo,
            object[] customAttributes,
            Susanoo.DescriptorActions actions = Susanoo.DescriptorActions.Read 
                | Susanoo.DescriptorActions.Update 
                | Susanoo.DescriptorActions.Insert,
            string[] whitelist = null,
            string[] blacklist = null)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (customAttributes == null)
                throw new ArgumentNullException("customAttributes");

            Contract.EndContractBlock();

            var attribute = customAttributes
                .OfType<AllowedActionsAttribute>()
                .FirstOrDefault();

            bool IsActionable = IsWhitelisted(propertyInfo, whitelist)
                || (!(IsBlacklisted(propertyInfo, blacklist))
                    && IsAllowedByAttribute(propertyInfo, attribute, actions));

            return IsActionable;
        }

        /// <summary>
        /// Determines whether the specified property is restricted declaratively.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="actions">The actions.</param>
        /// <returns><c>true</c> if [is allowed by attribute] then [the specified property information]; otherwise, <c>false</c>.</returns>
        public virtual bool IsAllowedByAttribute(
            PropertyInfo propertyInfo,
            AllowedActionsAttribute attribute,
            Susanoo.DescriptorActions actions = Susanoo.DescriptorActions.Read 
                | Susanoo.DescriptorActions.Update 
                | Susanoo.DescriptorActions.Insert)
        {
            return attribute == null || attribute.Actions == actions;
        }
    }
}