using Autofac;

namespace Susanoo.DependencyInjection.Autofac
{
    public class AutofacSusanooBootstrapper
        : SusanooBootstrapper
    {
        public AutofacSusanooBootstrapper(IContainer container)
            : base(new AutofacAdapter(container))
        {

        }
    }
}
