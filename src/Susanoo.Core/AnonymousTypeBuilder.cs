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
        private static readonly ModuleBuilder ModuleBuilder;
        private static readonly Dictionary<string, Type> BuiltTypes = new Dictionary<string, Type>();

        static AnonymousTypeBuilder()
        {
            ModuleBuilder = CommandManager.DynamicNamespace;
        }

        //private static string GetTypeKey(Dictionary<string, Type> properties)
        //{
        //    return properties.Aggregate(string.Empty,
        //        (current, field) => current + (field.Key + ";" + field.Value.Name + ";"));
        //}

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
                var className = Guid.NewGuid().ToString().Replace("-", string.Empty);

                Type value;
                if (BuiltTypes.TryGetValue(className, out value))
                    return value;

                var typeBuilder = ModuleBuilder.DefineType(className,
                    TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);


                foreach (var field in fields)
                {
                    var fb = typeBuilder.DefineField("_" + field.Key, field.Value, FieldAttributes.Private);

                    PropertyBuilder pb = typeBuilder
                        .DefineProperty(field.Key, PropertyAttributes.HasDefault, field.Value, null);

                    const MethodAttributes ma = MethodAttributes.Public |
                                                MethodAttributes.SpecialName |
                                                MethodAttributes.HideBySig;

                    MethodBuilder getBuilder =
                        typeBuilder.DefineMethod("get_" + field.Key,
                                                   ma,
                                                   field.Value,
                                                   Type.EmptyTypes);
                    MethodBuilder setBuilder =
                        typeBuilder.DefineMethod("set_" + field.Key,
                                                   ma,
                                                   null,
                                                   new[] { field.Value });

                    pb.SetGetMethod(getBuilder);
                    ILGenerator getIL = getBuilder.GetILGenerator();

                    getIL.Emit(OpCodes.Ldarg_0);
                    getIL.Emit(OpCodes.Ldfld, fb);
                    getIL.Emit(OpCodes.Ret);

                    pb.SetSetMethod(setBuilder);
                    ILGenerator setIL = setBuilder.GetILGenerator();

                    setIL.Emit(OpCodes.Ldarg_0);
                    setIL.Emit(OpCodes.Ldarg_1);
                    //setIL.Emit(OpCodes.Castclass, field.Value);
                    setIL.Emit(OpCodes.Stfld, fb);
                    setIL.Emit(OpCodes.Ret);
                }

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