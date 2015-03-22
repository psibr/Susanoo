using System.Web.Optimization;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.TinyIoc;

namespace Susanoo.Example.Web
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            var libsScriptBundle = new ScriptBundle("~/js/libs") // 3rd Party Libs
                .IncludeDirectory("~/content/js/libs", "*.js", false);

            var coreStyleBundle = new StyleBundle("~/css")
                .IncludeDirectory("~/content/css/", "*.css", false) // Bootstrap & theme
                .IncludeDirectory("~/content/css/select2", "*.css");

            BundleTable.Bundles.Add(libsScriptBundle);
            BundleTable.Bundles.Add(coreStyleBundle);

            BundleTable.EnableOptimizations = true;
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration() { Password = "admin" }; }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions
                .Add(StaticContentConventionBuilder.AddDirectory("/content"));

            nancyConventions.ViewLocationConventions.Insert(0, (viewName, model, context) =>
                string.IsNullOrWhiteSpace(context.ModulePath)
                    ? null
                    : string.Concat("views/", context.ModulePath, "/", context.ModuleName, "/", viewName));

            base.ConfigureConventions(nancyConventions);
        }

    }
}