#region

using NUnit.Framework;

#endregion


namespace Susanoo.UnitOfWork.Tests
{
    [SetUpFixture]
    public class Setup
    {

        [SetUp]
        public void Configure()
        {
            CommandManager.RegisterBootstrapper(new TestBootstrapper());
        }

    }
}