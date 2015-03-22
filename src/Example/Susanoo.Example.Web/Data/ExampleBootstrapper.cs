using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using Susanoo.Pipeline.Command;

namespace Susanoo.Example.Web.Data
{
    public class ExampleBootstrapper : SusanooBootstrapper
    {
        public override void OnExecutionException(ICommandInfo info, Exception exception, DbParameter[] parameters)
        {
            //Custom error handling here.

            base.OnExecutionException(info, exception, parameters);
        }
    }
}