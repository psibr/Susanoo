using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Susanoo
{
    /// <summary>
    /// Helps build types to represent dynamic.
    /// </summary>
    public static class AnonymousTypeBuilder
    {
        private static readonly AssemblyName AssemblyName = new AssemblyName() { Name = "Susanoo.AnonymousTypes" };
        private static readonly ModuleBuilder ModuleBuilder;
        private static readonly Dictionary<string, Type> BuiltTypes = new Dictionary<string, Type>();

        static AnonymousTypeBuilder()
        {
            ModuleBuilder = Thread.GetDomain().DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name);
        }

        private static string GetTypeKey(Dictionary<string, Type> properties)
        {
            return properties.Aggregate(string.Empty, (current, field) => current + (field.Key + ";" + field.Value.Name + ";"));
        }

        /// <summary>
        /// Builds a new type to represent the data.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>Type.</returns>
        /// <exception cref="System.ArgumentNullException">fields</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">fields;fields must have at least 1 field definition</exception>
        public static Type BuildType(Dictionary<string, Type> fields)
        {
            if (null == fields)
                throw new ArgumentNullException("fields");
            if (0 == fields.Count)
                throw new ArgumentOutOfRangeException("fields", "fields must have at least 1 field definition");

            try
            {
                Monitor.Enter(BuiltTypes);
                var className = GetTypeKey(fields);

                Type value;
                if (BuiltTypes.TryGetValue(className, out value))
                    return value;

                var typeBuilder = ModuleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                foreach (var field in fields)
                    typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public);

                BuiltTypes[className] = typeBuilder.CreateType();

                return BuiltTypes[className];
            }
            finally
            {
                Monitor.Exit(BuiltTypes);
            }
        }
    }
}
