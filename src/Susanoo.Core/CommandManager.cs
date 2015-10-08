#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using Susanoo.Command;

#endregion

namespace Susanoo
{
    /// <summary>
    /// This class is used as the single entry point when using Susanoo.
    /// </summary>
    public class CommandManager
    {
        private CommandManager()
        {

        }

        private static readonly IDictionary<Type, DbType> BuiltinTypeConversions =
            new ConcurrentDictionary<Type, DbType>(new Dictionary<Type, DbType>
            {
                {typeof (byte), DbType.Byte},
                {typeof (sbyte), DbType.SByte},
                {typeof (short), DbType.Int16},
                {typeof (ushort), DbType.UInt16},
                {typeof (int), DbType.Int32},
                {typeof (uint), DbType.UInt32},
                {typeof (long), DbType.Int64},
                {typeof (ulong), DbType.UInt64},
                {typeof (float), DbType.Single},
                {typeof (double), DbType.Double},
                {typeof (decimal), DbType.Decimal},
                {typeof (bool), DbType.Boolean},
                {typeof (string), DbType.String},
                {typeof (char), DbType.StringFixedLength},
                {typeof (Guid), DbType.Guid},
                {typeof (DateTime), DbType.DateTime},
                {typeof (DateTimeOffset), DbType.DateTimeOffset},
                {typeof (byte[]), DbType.Binary},
                {typeof (byte?), DbType.Byte},
                {typeof (sbyte?), DbType.SByte},
                {typeof (short?), DbType.Int16},
                {typeof (ushort?), DbType.UInt16},
                {typeof (int?), DbType.Int32},
                {typeof (uint?), DbType.UInt32},
                {typeof (long?), DbType.Int64},
                {typeof (ulong?), DbType.UInt64},
                {typeof (float?), DbType.Single},
                {typeof (double?), DbType.Double},
                {typeof (decimal?), DbType.Decimal},
                {typeof (bool?), DbType.Boolean},
                {typeof (char?), DbType.StringFixedLength},
                {typeof (Guid?), DbType.Guid},
                {typeof (DateTime?), DbType.DateTime},
                {typeof (DateTimeOffset?), DbType.DateTimeOffset}
            });

        /// <summary>
        /// Gets the expression assembly that contains runtime compiled methods used for mappings.
        /// </summary>
        /// <value>The expression assembly.</value>
        private static readonly AssemblyBuilder ExpressionAssembly = AppDomain.CurrentDomain
            .DefineDynamicAssembly(new AssemblyName("Susanoo.DynamicExpression"), AssemblyBuilderAccess.RunAndSave);

        /// <summary>
        /// Gets the dynamic namespace.
        /// </summary>
        /// <value>The dynamic namespace.</value>
        internal static ModuleBuilder DynamicNamespace { get; } =
            ExpressionAssembly
            .DefineDynamicModule("Susanoo.DynamicExpression", "Susanoo.DynamicExpression.dll");

        /// <summary>
        /// Begins the CommandBuilder definition process using a Fluent API implementation.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="commandText">The CommandBuilder text.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter> DefineCommand<TFilter>(string commandText, CommandType commandType)
        {
            return Bootstrapper
                .ResolveCommandBuilder()
                .DefineCommand<TFilter>(commandText, commandType);
        }

        /// <summary>
        /// Begins the CommandBuilder definition process using a Fluent API implementation.
        /// </summary>
        /// <param name="commandText">The CommandBuilder text.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<dynamic> DefineCommand(string commandText, CommandType commandType)
        {
            return Bootstrapper
                .ResolveCommandBuilder()
                .DefineCommand(commandText, commandType);
        }

        /// <summary>
        /// Gets the database type from the CLR type for use in parameters.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <returns>DbType.</returns>
        public static DbType? GetDbType(Type type)
        {
            DbType dataType;
            DbType? typeToUse;
            if (!BuiltinTypeConversions.TryGetValue(type, out dataType))
                typeToUse = null;
            else
                typeToUse = dataType;

            return typeToUse;
        }

        /// <summary>
        /// Saves the dynamic assembly to disk.
        /// </summary>
        public static void SaveDynamicAssemblyToDisk()
        {
            ExpressionAssembly.Save("Susanoo.DynamicExpression.Loader");
        }

        private static CommandManager _instance;

        private static readonly object SyncRoot = new object();

        private ISusanooBootstrapper _bootstrapper;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static CommandManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new CommandManager();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Registers a bootstrapper which provides extension points in susanoo.
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper.</param>
        /// <exception cref="System.ArgumentNullException">bootstrapper</exception>
        public void Bootstrap(ISusanooBootstrapper bootstrapper)
        {
            if (bootstrapper == null)
                throw new ArgumentNullException(nameof(bootstrapper));

            _bootstrapper = bootstrapper;

            _bootstrapper.Initialize();
        }

        /// <summary>
        /// Resolves a DatabaseManagerFactory from the bootstrapper
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Susanoo.IDatabaseManagerFactory.</returns>
        public static IDatabaseManagerFactory ResolveDatabaseManagerFactory(string name = null)
        {
            return Instance.Bootstrapper
                .ResolveDatabaseManagerFactory(name);
        }

        /// <summary>
        /// Gets the bootstrapper.
        /// </summary>
        /// <value>The bootstrapper.</value>
        public ISusanooBootstrapper Bootstrapper
        {
            get
            {
                if (_bootstrapper == null)
                {
                    _bootstrapper = new SusanooBootstrapper();

                    _bootstrapper.Initialize();
                }

                return _bootstrapper;
            }
        }
    }
}