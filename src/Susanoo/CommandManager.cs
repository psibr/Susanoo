using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Susanoo
{
    public sealed class CommandManager
    {
        /// <summary>
        /// The expression assembly
        /// </summary>
        public AssemblyBuilder expressionAssembly = AppDomain.CurrentDomain
            .DefineDynamicAssembly(new AssemblyName("Susanoo.DynamicExpression"), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);

        /// <summary>
        /// The synchronization root.
        /// </summary>
        private static readonly object syncRoot = new object();

        /// <summary>
        /// The instance
        /// </summary>
        private static CommandManager _Instance;

        private ModuleBuilder _moduleBuilder;

        /// <summary>
        /// Prevents a default instance of the <see cref="CommandManager" /> class from being created.
        /// </summary>
        private CommandManager()
        {
            this.Container = TinyIoC.TinyIoCContainer.Current;

            this.Container.Register<ICommandExpressionBuilder>(new CommandBuilder());
            this.Container.Register<IPropertyMetadataExtractor>(new ComponentModelMetadataExtractor());

            this._moduleBuilder = this.expressionAssembly
                    .DefineDynamicModule("Susanoo.DynamicExpression", "Susanoo.DynamicExpression.dll");
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static CommandManager Instance
        {
            get
            {
                //Perform double-checked singleton instantiation for thread synchronization.
                if (_Instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new CommandManager();
                    }
                }

                return _Instance;
            }
        }

        /// <summary>
        /// Gets the dynamic namespace.
        /// </summary>
        /// <value>The dynamic namespace.</value>
        public ModuleBuilder DynamicNamespace
        {
            get
            {
                return this._moduleBuilder;
            }
        }

        /// <summary>
        /// Gets the IoC container.
        /// </summary>
        /// <value>The container.</value>
        public TinyIoC.TinyIoCContainer Container { get; private set; }

        /// <summary>
        /// Gets the database manager.
        /// </summary>
        /// <value>The database manager.</value>
        private static IDatabaseManager DatabaseManager
        {
            get
            {
                return CommandManager.Instance.Container.Resolve<IDatabaseManager>();
            }
        }

        /// <summary>
        /// Gets the commander.
        /// </summary>
        /// <value>The commander.</value>
        private static ICommandExpressionBuilder Commander
        {
            get
            {
                return CommandManager.Instance.Container.Resolve<ICommandExpressionBuilder>();
            }
        }

        /// <summary>
        /// Registers the database manager.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        public static void RegisterDatabaseManager(IDatabaseManager databaseManager)
        {
            CommandManager.Instance.Container.Register<IDatabaseManager>(databaseManager);
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
            return CommandManager.Commander.DefineCommand<TFilter, TResult>(commandText, commandType);
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <returns>IDbDataParameter.</returns>
        public static IDbDataParameter CreateParameter()
        {
            return CommandManager.DatabaseManager.CreateParameter();
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
            return CommandManager.DatabaseManager.CreateParameter(parameterName, parameterDirection, parameterType, value);
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
            return CommandManager.DatabaseManager.CreateInputParameter(parameterName, parameterType, value);
        }
    }
}