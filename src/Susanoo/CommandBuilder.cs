namespace Susanoo
{
    public class CommandBuilder : ICommandExpressionBuilder
    {
        /// <summary>
        /// Begins the command definition process using a Fluent API implementation, move to next step with DefineMappings on the result of this call.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandExpression<TFilter, TResult> DefineCommand<TFilter, TResult>(string commandText, System.Data.CommandType commandType)
            where TResult : new()
        {
            return new CommandExpression<TFilter, TResult>(CommandManager.Instance.Container.Resolve<IDatabaseManager>(), commandText, commandType);
        }

        /// <summary>
        /// Begins the command definition process using a Fluent API implementation, move to next step with DefineResultMappings on the result of this call.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICommandExpression<TResult> DefineCommand<TResult>(string commandText, System.Data.CommandType commandType) where TResult : new()
        {
            throw new System.NotImplementedException();
        }
    }
}