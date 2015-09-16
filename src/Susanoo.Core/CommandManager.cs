#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using Susanoo.Command;
using Susanoo.Pipeline;
using Susanoo.Processing;

#endregion

namespace Susanoo
{
    /// <summary>
    /// This class is used as the single entry point when using Susanoo.
    /// </summary>
    public partial class CommandManager
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

        private readonly ConcurrentDictionary<BigInteger, ICommandProcessorWithResults> _registeredCommandProcessors =
                    new ConcurrentDictionary<BigInteger, ICommandProcessorWithResults>();

        private readonly ConcurrentDictionary<string, ICommandProcessorWithResults> _namedCommandProcessors =
                    new ConcurrentDictionary<string, ICommandProcessorWithResults>();

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
        /// Handles exceptions in execution.
        /// </summary>
        /// <param name="commandInfo">The CommandBuilder expression information.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        public void HandleExecutionException(
            ICommandInfo commandInfo,
            Exception ex,
            DbParameter[] parameters)
        {
            Bootstrapper.OnExecutionException(commandInfo, ex, parameters);
        }

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
                .ResolveDependency<ICommandBuilder>()
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
                .ResolveDependency<ICommandBuilder>()
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
        /// Attempts to get a CommandBuilder processor by hash code.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="commandProcessor">The CommandBuilder processor.</param>
        /// <returns><c>true</c> if a CommandBuilder processor with the same configuration has been registered and not garbage collected,
        /// <c>false</c> otherwise.</returns>
        public bool TryGetCommandProcessor(BigInteger hash, out ICommandProcessorWithResults commandProcessor)
        {
            var result = _registeredCommandProcessors.TryGetValue(hash, out commandProcessor);

            return result;
        }

        /// <summary>
        /// Attempts to get a CommandBuilder processor by name.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <param name="commandProcessor">The CommandBuilder processor.</param>
        /// <returns><c>true</c> if a CommandBuilder processor with the same configuration has been registered and not garbage collected,
        /// <c>false</c> otherwise.</returns>
        public bool TryGetCommandProcessor(string name, out ICommandProcessorWithResults commandProcessor)
        {
            var result = _namedCommandProcessors.TryGetValue(name, out commandProcessor);

            return result;
        }

        /// <summary>
        /// Registers the CommandBuilder processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="name">The name.</param>
        /// <param name="hashCodeOverride">The hash code override.</param>
        public void RegisterCommandProcessor(ICommandProcessorWithResults processor, string name = null, BigInteger hashCodeOverride = default(BigInteger))
        {
            var hash = hashCodeOverride != default(BigInteger) ? hashCodeOverride : processor.CacheHash;

            _registeredCommandProcessors.TryAdd(hash, processor);

            if (!string.IsNullOrWhiteSpace(name))
                _namedCommandProcessors.TryAdd(name, processor);
        }

        /// <summary>
        /// Flushes caches on all CommandBuilder processors.
        /// </summary>
        public void FlushCacheGlobally()
        {
            foreach (var registeredCommandProcessor in _registeredCommandProcessors)
            {
                registeredCommandProcessor.Value.FlushCache();
            }
        }

        /// <summary>
        /// Clears any column index information that may have been cached.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <exception cref="System.ArgumentNullException">processor</exception>
        public void ClearColumnIndexInfo(ICommandProcessorWithResults processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            processor.ClearColumnIndexInfo();
        }

        /// <summary>
        /// Clears any column index information that may have been cached.
        /// </summary>
        public void ClearColumnIndexInfo()
        {
            foreach (var processor in _registeredCommandProcessors
                .Select(kvp => kvp.Value))
                processor.ClearColumnIndexInfo();
        }

        /// <summary>
        /// Flushes caches on a specific named CommandBuilder processor.
        /// </summary>
        /// <param name="name">The name of the CommandBuilder processor.</param>
        public void FlushCache(string name)
        {
            ICommandProcessorWithResults reference;
            if (_namedCommandProcessors.TryGetValue(name, out reference))
                reference.FlushCache();
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

            Bootstrapper = bootstrapper;
        }

        /// <summary>
        /// Gets the bootstrapper.
        /// </summary>
        /// <value>The bootstrapper.</value>
        public ISusanooBootstrapper Bootstrapper { get; private set; } =
            new SusanooBootstrapper();
    }
}