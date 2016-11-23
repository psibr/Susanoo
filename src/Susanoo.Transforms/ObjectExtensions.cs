using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;

namespace Susanoo
{
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object to an ExpandoObject.
        /// </summary>
        /// <param name="anonymousObject">The anonymous object.</param>
        /// <returns>ExpandoObject.</returns>
        internal static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            

            foreach (var propertyDescriptor in anonymousObject.GetType().GetTypeInfo()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var obj = propertyDescriptor.GetValue(anonymousObject);
                expando.Add(propertyDescriptor.Name, obj);
            }

            return (ExpandoObject)expando;
        }
    }
}
