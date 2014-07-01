using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Susanoo
{
    /// <summary>
    /// This class is used as the single entry point when dealing with Susanoo.
    /// </summary>
    public static class CommandManager
    {
        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly object syncRoot = new object();

        private static AssemblyBuilder _expressionAssembly = AppDomain.CurrentDomain
                    .DefineDynamicAssembly(new AssemblyName("Susanoo.DynamicExpression"), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);

        private static ModuleBuilder _moduleBuilder = ExpressionAssembly
                    .DefineDynamicModule("Susanoo.DynamicExpression", "Susanoo.DynamicExpression.dll");

        private static ICommandExpressionBuilder _CommandBuilder = new CommandBuilder();

        /// <summary>
        /// Gets the expression assembly that contains runtime compiled methods used for mappings.
        /// </summary>
        /// <value>The expression assembly.</value>
        public static AssemblyBuilder ExpressionAssembly
        {
            get
            {
                return _expressionAssembly;
            }
        }

        /// <summary>
        /// Gets the dynamic namespace.
        /// </summary>
        /// <value>The dynamic namespace.</value>
        internal static ModuleBuilder DynamicNamespace
        {
            get
            {
                return _moduleBuilder;
            }
        }

        /// <summary>
        /// Gets the database manager.
        /// </summary>
        /// <value>The database manager.</value>
        public static IDatabaseManager DatabaseManager { get; private set; }

        /// <summary>
        /// Gets the commander.
        /// </summary>
        /// <value>The commander.</value>
        internal static ICommandExpressionBuilder Commander
        {
            get
            {
                return _CommandBuilder;
            }
        }

        /// <summary>
        /// Registers the database manager.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        public static void RegisterDatabaseManager(IDatabaseManager databaseManager)
        {
            CommandManager.DatabaseManager = databaseManager;
        }

        /// <summary>
        /// Registers a command builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void RegisterCommandBuilder(ICommandExpressionBuilder builder)
        {
            CommandManager._CommandBuilder = builder;
        }

        /// <summary>
        /// Begins the command definition process using a Fluent API implementation, move to next step with DefineMappings on the result of this call.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public static ICommandExpression<TFilter, TResult> DefineCommand<TFilter, TResult>(string commandText, CommandType commandType)
            where TResult : new()
        {
            return CommandManager.Commander
                .DefineCommand<TFilter, TResult>(commandText, commandType);
        }

        /// <summary>
        /// Begins the command definition process using a Fluent API implementation, move to next step with DefineMappings on the result of this call.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public static ICommandExpression<dynamic, TResult> DefineCommand<TResult>(string commandText, CommandType commandType)
            where TResult : new()
        {
            return CommandManager.Commander
                .DefineCommand<TResult>(commandText, commandType);
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <returns>IDbDataParameter.</returns>
        public static IDbDataParameter CreateParameter()
        {
            return CommandManager.DatabaseManager
                .CreateParameter();
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>IDbDataParameter.</returns>
        public static IDbDataParameter CreateParameter(string parameterName, ParameterDirection parameterDirection, DbType parameterType, object value)
        {
            return CommandManager.DatabaseManager
                .CreateParameter(parameterName, parameterDirection, parameterType, value);
        }

        /// <summary>
        /// Creates an input parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>IDbDataParameter.</returns>
        public static IDbDataParameter CreateInputParameter(string parameterName, DbType parameterType, object value)
        {
            return CommandManager.DatabaseManager
                .CreateInputParameter(parameterName, parameterType, value);
        }

        private static IDictionary<string, IDatabaseManager> databaseManagers = new Dictionary<string, IDatabaseManager>();

    }
}