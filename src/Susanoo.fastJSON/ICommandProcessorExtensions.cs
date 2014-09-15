using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo.fastJSON
{
    public static class ICommandProcessorExtensions
    {
        public static string ExecuteToJson<TFilter, TResult>(this Susanoo.ICommandProcessor<TFilter, TResult> processor,
            IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
            where TResult : new()
        {
            throw new NotImplementedException();
        }

        public static string ExecuteToJson<TFilter, TResult>(this Susanoo.ICommandProcessor<TFilter, TResult> processor,
            IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
            where TResult : new()
        {
            throw new NotImplementedException();
        }
    }
}