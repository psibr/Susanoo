using Machine.Specifications;

namespace Susanoo.MSpec.Tests
{
    [Subject(typeof(CommandManager))]
    public class when_resolving_a_database_manager_factory
    {
        static IDatabaseManagerFactory DatabaseManagerFactory;

        Because of = () =>
            DatabaseManagerFactory = CommandManager.ResolveDatabaseManagerFactory();

        It should_not_be_null = () =>
            DatabaseManagerFactory.ShouldNotBeNull();

        It should_be_able_to_create_a_database_manager = () =>
            DatabaseManagerFactory.CreateFromConnectionString("", "System.Data.SqlClient").ShouldNotBeNull();
    }
}