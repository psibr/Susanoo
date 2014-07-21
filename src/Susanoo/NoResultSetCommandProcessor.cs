namespace Susanoo
{
    public class NoResultSetCommandProcessor<TFilter> : ICommandProcessor<TFilter>, IFluentPipelineFragment
    {
        private readonly ICommandExpression<TFilter> _CommandExpression;

        public NoResultSetCommandProcessor(ICommandExpression<TFilter> command)
        {
            this._CommandExpression = command;
        }

        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _CommandExpression; }
        }

        public System.Numerics.BigInteger CacheHash
        {
            get { return _CommandExpression.CacheHash; }
        }

        public TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter, params System.Data.IDbDataParameter[] explicitParameters)
        {
            return databaseManager.ExecuteScalar<TReturn>(
                CommandExpression.CommandText,
                CommandExpression.DBCommandType,
                CommandExpression.BuildParameters(databaseManager, filter, explicitParameters));
        }

        public TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, params System.Data.IDbDataParameter[] explicitParameters)
        {
            return ExecuteScalar<TReturn>(databaseManager, default(TFilter), explicitParameters);
        }

        public int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter, params System.Data.IDbDataParameter[] explicitParameters)
        {
            return databaseManager.ExecuteStoredProcedureNonQuery(
                CommandExpression.CommandText,
                CommandExpression.DBCommandType,
                CommandExpression.BuildParameters(databaseManager, filter, explicitParameters));
        }

        public int ExecuteNonQuery(IDatabaseManager databaseManager, params System.Data.IDbDataParameter[] explicitParameters)
        {
            return ExecuteNonQuery(databaseManager, default(TFilter), explicitParameters);
        }
    }
}