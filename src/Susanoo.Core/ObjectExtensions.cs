using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
            {
                var obj = propertyDescriptor.GetValue(anonymousObject);
                expando.Add(propertyDescriptor.Name, obj);
            }

            return (ExpandoObject)expando;
        }
    }
}
