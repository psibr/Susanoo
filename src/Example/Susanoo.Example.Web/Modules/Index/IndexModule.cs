using Nancy;

namespace Susanoo.Example.Web.Modules.Index
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/"] = model => View["Index"];
        }
    }
}