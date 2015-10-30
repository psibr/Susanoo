using StructureMap;

namespace Susanoo.DependencyInjection.StructureMap
{
    public class StructureMapSusanooBootstrapper
        : SusanooBootstrapper
    {
        public StructureMapSusanooBootstrapper(IContainer container)
            : base(new StructureMapAdapter(container))
        {
            
        }
    }
}
