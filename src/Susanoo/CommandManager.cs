#region

using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     This class is used as the single entry point when dealing with Susanoo.
    /// </summary>
    public static class CommandManager
    {
        /// <summary>
        ///     Gets the expression assembly that contains runtime compiled methods used for mappings.
        /// </summary>
        /// <value>The expression assembly.</value>
        public static readonly AssemblyBuilder ExpressionAssembly = AppDomain.CurrentDomain
            .DefineDynamicAssembly(new AssemblyName("Susanoo.DynamicExpression"), AssemblyBuilderAccess.RunAndSave);

        private static readonly ModuleBuilder ModuleBuilder = ExpressionAssembly
            .DefineDynamicModule("Susanoo.DynamicExpression", "Susanoo.DynamicExpression.dll");

        private static ICommandExpressionBuilder _commandBuilder = new CommandBuilder();

        private static Func<string, IDatabaseManager> _databaseManagerFactoryMethod = connectionStringName =>
            new DatabaseManager(connectionStringName);


        /// <summary>
        ///     Gets the dynamic namespace.
        /// </summary>
        /// <value>The dynamic namespace.</value>
        internal static ModuleBuilder DynamicNamespace
        {
            get { return ModuleBuilder; }
        }

        /// <summary>
        ///     Gets the commander.
        /// </summary>
        /// <value>The commander.</value>
        internal static ICommandExpressionBuilder Commander
        {
            get { return _commandBuilder; }
        }

        /// <summary>
        ///     Gets the database manager.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>IDatabaseManager.</returns>
        /// <value>The database manager.</value>
        public static IDatabaseManager BuildDatabaseManager(string connectionString)
        {
            return _databaseManagerFactoryMethod(connectionString);
        }

        /// <summary>
        ///     Registers the database manager.
        /// </summary>
        /// <param name="databaseManagerFactoryMethod">The database manager factory method.</param>
        public static void RegisterDatabaseManagerFactory(Func<string, IDatabaseManager> databaseManagerFactoryMethod)
        {
            if (databaseManagerFactoryMethod != null)
                _databaseManagerFactoryMethod = databaseManagerFactoryMethod;
        }

        /// <summary>
        ///     Registers a command builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void RegisterCommandBuilder(ICommandExpressionBuilder builder)
        {
            _commandBuilder = builder;
        }

        /// <summary>
        ///     Begins the command definition process using a Fluent API implementation, move to next step with DefineMappings on
        ///     the result of this call.
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
        ///     Begins the command definition process using a Fluent API implementation, move to next step with DefineMappings on
        ///     the result of this call.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public static ICommandExpression<dynamic> DefineCommand(string commandText, CommandType commandType)
        {
            return Commander
                .DefineCommand(commandText, commandType);
        }
    }

    //Testing CI.

    //public sealed class MappingContainer
    //{
    //    private IDictionary<Type, IDictionary<string, Func<IDataRecord, object>>> compiledMappings =
    //        new Dictionary<Type, IDictionary<string, Func<IDataRecord, object>>>();

    //    public void Store(Type type, string id, Func<IDataRecord, object> mappingDelegate)
    //    {
    //    }
    //}
}