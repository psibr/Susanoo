using System.Data;
using Machine.Specifications;
using Susanoo.Command;

namespace Susanoo.MSpec.Tests
{
    [Subject(typeof(CommandManager))]
    public class When_defining_a_command_with_a_dynamic_filter
    {
        static ICommandExpression<dynamic> command;

        Establish context = () => { };

        Because of = () => command = CommandManager.Instance.DefineCommand("SELECT 1 AS Success", CommandType.Text);

        It should_have_a_type_param_of_object = () => command.ShouldBeAssignableTo(typeof(ICommandExpression<object>));

        Cleanup after = () => { };
    }
}
