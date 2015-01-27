#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using Susanoo.Pipeline.Command;
using Susanoo.Pipeline.Command.ResultSets.Processing;

#endregion

namespace Susanoo
{
    /// <summary>
    /// This class is used as the single entry point when using with Susanoo.
    /// </summary>
    public static class CommandManager
    {
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

        private static ICommandExpressionBuilder _commandBuilder = new CommandBuilder();

        private static Func<string, IDatabaseManager> _databaseManagerFactoryMethod = connectionStringName =>
            new DatabaseManager(connectionStringName);

        private static readonly ConcurrentDictionary<BigInteger, ICommandProcessorWithResults> RegisteredCommandProcessors =
            new ConcurrentDictionary<BigInteger, ICommandProcessorWithResults>();

        private static readonly ConcurrentDictionary<string, ICommandProcessorWithResults> NamedCommandProcessors =
            new ConcurrentDictionary<string, ICommandProcessorWithResults>();

        /// <summary>
        /// Gets the commander.
        /// </summary>
        /// <value>The commander.</value>
        public static ICommandExpressionBuilder Commander
        {
            get { return _commandBuilder; }
        }

        /// <summary>
        /// Gets the dynamic namespace.
        /// </summary>
        /// <value>The dynamic namespace.</value>
        internal static ModuleBuilder DynamicNamespace
        {
            get { return ModuleBuilder; }
        }

        /// <summary>
        /// Gets the database manager.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>IDatabaseManager.</returns>
        /// <value>The database manager.</value>
        public static IDatabaseManager BuildDatabaseManager(string connectionString)
        {
            return _databaseManagerFactoryMethod(connectionString);
        }

        /// <summary>
        /// Registers the database manager.
        /// </summary>
        /// <param name="databaseManagerFactoryMethod">The database manager factory method.</param>
        public static void RegisterDatabaseManagerFactory(Func<string, IDatabaseManager> databaseManagerFactoryMethod)
        {
            if (databaseManagerFactoryMethod != null)
                _databaseManagerFactoryMethod = databaseManagerFactoryMethod;
        }

        /// <summary>
        /// Registers a command builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void RegisterCommandBuilder(ICommandExpressionBuilder builder)
        {
            _commandBuilder = builder;
        }

        /// <summary>
        /// Begins the command definition process using a Fluent API implementation, move to next step with DefineMappings on
        /// the result of this call.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public static ICommandExpression<TFilter> DefineCommand<TFilter>(string commandText, CommandType commandType)
        {
            return Commander
                .DefineCommand<TFilter>(commandText, commandType);
        }

        /// <summary>
        /// Begins the command definition process using a Fluent API implementation, move to next step with DefineMappings on
        /// the result of this call.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public static ICommandExpression<dynamic> DefineCommand(string commandText, CommandType commandType)
        {
            return Commander
                .DefineCommand(commandText, commandType);
        }

        /// <summary>
        ///     Gets the database type from the CLR type.
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
        /// Attempts to get a command processor by hash code.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="commandProcessor">The command processor.</param>
        /// <returns><c>true</c> if a command processor with the same configuration has been registered and not garbage collected,
        /// <c>false</c> otherwise.</returns>
        public static bool TryGetCommandProcessor(BigInteger hash, out ICommandProcessorWithResults commandProcessor)
        {
            var result = RegisteredCommandProcessors.TryGetValue(hash, out commandProcessor);

            return result;
        }

        /// <summary>
        /// Attempts to get a command processor by name.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <param name="commandProcessor">The command processor.</param>
        /// <returns><c>true</c> if a command processor with the same configuration has been registered and not garbage collected,
        /// <c>false</c> otherwise.</returns>
        public static bool TryGetCommandProcessor(string name, out ICommandProcessorWithResults commandProcessor)
        {
            var result = NamedCommandProcessors.TryGetValue(name, out commandProcessor);

            return result;
        }

        /// <summary>
        /// Registers the command processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="name">The name.</param>
        public static void RegisterCommandProcessor(ICommandProcessorWithResults processor, string name)
        {
            RegisteredCommandProcessors.TryAdd(processor.CacheHash, processor);

            if (!string.IsNullOrWhiteSpace(name))
                NamedCommandProcessors.TryAdd(name, processor);
        }

        /// <summary>
        /// Flushes caches on all command processors.
        /// </summary>
        public static void FlushCacheGlobally()
        {
            foreach (var registeredCommandProcessor in RegisteredCommandProcessors)
            {
                registeredCommandProcessor.Value.FlushCache();
            }
        }

        /// <summary>
        /// Clears any column index information that may have been cached.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <exception cref="System.ArgumentNullException">processor</exception>
        public static void ClearColumnIndexInfo(ICommandProcessorWithResults processor)
        {
            if(processor == null)
                throw new ArgumentNullException("processor");

            processor.ClearColumnIndexInfo();
        }

        /// <summary>
        /// Clears any column index information that may have been cached.
        /// </summary>
        public static void ClearColumnIndexInfo()
        {
            RegisteredCommandProcessors
                .Select(kvp => kvp.Value)
                .ToList()
                .ForEach(processor => processor.ClearColumnIndexInfo());
        }

        /// <summary>
        /// Flushes caches on a specific named command processor.
        /// </summary>
        /// <param name="name">The name of the command processor.</param>
        public static void FlushCache(string name)
        {
            ICommandProcessorWithResults reference;
            if (NamedCommandProcessors.TryGetValue(name, out reference))
                reference.FlushCache();
        }

        /// <summary>
        /// Saves the dynamic assembly to disk.
        /// </summary>
        /// <param name="assemblyFileName">Name of the assembly file.</param>
        public static void SaveDynamicAssemblyToDisk(string assemblyFileName)
        {
            ExpressionAssembly.Save(assemblyFileName);
        }

        /// <summary>
        /// Gets the expression assembly that contains runtime compiled methods used for mappings.
        /// </summary>
        /// <value>The expression assembly.</value>
        private static readonly AssemblyBuilder ExpressionAssembly = AppDomain.CurrentDomain
            .DefineDynamicAssembly(new AssemblyName("Susanoo.DynamicExpression"), AssemblyBuilderAccess.RunAndSave);

        /// <summary>
        /// The module builder for the dynamic assembly.
        /// </summary>
        private static readonly ModuleBuilder ModuleBuilder = ExpressionAssembly
            .DefineDynamicModule("Susanoo.DynamicExpression", "Susanoo.DynamicExpression.dll");
    }
}