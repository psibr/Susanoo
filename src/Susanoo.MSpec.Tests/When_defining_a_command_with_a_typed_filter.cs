using System.Collections.Generic;
using System.Data;
using Machine.Specifications;
using Susanoo.Command;
using After = Machine.Specifications.Because;

namespace Susanoo.MSpec.Tests
{
    [Subject(typeof(CommandManager))]
    public class When_defining_a_command_with_a_typed_filter
    {
        static string CommandText = "SELECT 1 AS Success";
        static CommandType CommandType = CommandType.Text;
        protected static ICommandExpression<KeyValuePair<string, string>> Command;

        Establish context = () => { };

        After defining_a_command_with_a_typed_filter = () => Command =
            CommandManager.Instance
                .DefineCommand<KeyValuePair<string, string>>(CommandText, CommandType);

        It should_have_a_matching_type_param = () => 
            Command.ShouldBeAssignableTo(typeof(ICommandExpression<KeyValuePair<string, string>>));

        Cleanup after = () => { };
    }
}