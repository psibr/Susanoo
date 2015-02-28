using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Susanoo.Pipeline.Command;

namespace Susanoo.SqlServer.Tests
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
