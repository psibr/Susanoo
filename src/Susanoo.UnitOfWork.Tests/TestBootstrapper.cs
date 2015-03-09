using System;
using System.Data.Common;
using Susanoo.Pipeline.Command;

namespace Susanoo.UnitOfWork.Tests
{
    public class TestBootstrapper : 
        SusanooBootstrapper
    {
        public override void OnExecutionException(ICommandInfo info, Exception exception, DbParameter[] parameters)
        {
            base.OnExecutionException(info, exception, parameters);
        }
    }
}
