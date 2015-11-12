using System.Data;
using Machine.Specifications;
using Susanoo.Command;
using InResponse = Machine.Specifications.Because;
using TheCommand = Machine.Specifications.It;
using AndTheCommand = Machine.Specifications.It;

namespace Susanoo.MSpec.Tests
{
    [Subject(typeof(CommandManager))]
    public class When_defining_a_command_with_a_dynamic_filter
    {
        static string CommandText = "SELECT 1 AS Success";
        static CommandType CommandType = CommandType.Text;
        static ICommandExpression<dynamic> Command;

        Establish context = () =>
        {
            CommandManager.Instance.Bootstrapper.ResolveCommandBuilder()
                .DefineCommand(CommandText, CommandType);
        };

        InResponse to_defining_a_dynamic_command = () => Command =
            CommandManager.Instance
                .DefineCommand(CommandText, CommandType);

        TheCommand should_have_a_type_param_of_object = () => 
            Command.ShouldBeAssignableTo(typeof(ICommandExpression<object>));

        AndTheCommand should_not_return_null = () =>
            Command.ShouldNotBeNull();

        //Behavior
        AndTheCommand should_respect_provided_command_text = () => 
            Command.CommandText.ShouldEqual(CommandText);
        AndTheCommand should_respect_provided_command_type = () => 
            Command.DbCommandType.ShouldEqual(CommandType);

        Cleanup after = () => { };
    }
}
