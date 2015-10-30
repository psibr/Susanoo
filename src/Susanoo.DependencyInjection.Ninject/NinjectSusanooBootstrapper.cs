using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace Susanoo.DependencyInjection.Ninject
{
    public class NinjectSusanooBootstrapper
        : SusanooBootstrapper
    {
        public NinjectSusanooBootstrapper(IKernel kernel)
            : base(new NinjectAdapter(kernel))
        {
            
        }
    }
}
