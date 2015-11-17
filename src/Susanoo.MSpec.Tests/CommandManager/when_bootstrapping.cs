using Machine.Specifications;

namespace Susanoo.MSpec.Tests
{
    [Subject(typeof(CommandManager))]
    public class when_bootstrapping
    {
        static readonly BasicBootstrapper Bootstrapper = new BasicBootstrapper();
        static bool InitializeWasCalled;

        Because of = () =>
            CommandManager.Instance
                .Bootstrap(Bootstrapper);

        It should_respect_provided_command_text = () =>
            CommandManager.Instance.Bootstrapper.ShouldBeTheSameAs(Bootstrapper);

        It should_have_been_initialized = () => 
            InitializeWasCalled.ShouldBeTrue();
            

        class BasicBootstrapper
            : SusanooBootstrapper
        {
            /// <summary>
            /// Registers the type chain for all types in Susanoo.
            /// </summary>
            public override void Initialize()
            {
                base.Initialize();

                InitializeWasCalled = true;
            }
        }
    }
}