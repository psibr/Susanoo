using Castle.Windsor;

namespace Susanoo.DependencyInjection.CastleWindsor
{
    public class CastleWindsorSusanooBootstrapper
        : SusanooBootstrapper
    {
        public CastleWindsorSusanooBootstrapper(IWindsorContainer container)
            : base(new CastleWindsorAdapter(container))
        {
            
        }
    }
}
