using System.Collections.Generic;
using System.Data;
using Machine.Specifications;
using Susanoo.Command;

namespace Susanoo.MSpec.Tests
{
    [Subject(typeof(CommandManager))]
    public class when_defining_a_command_expression
    {
        static string CommandText = "SELECT 1 AS Success";
        static CommandType CommandType = CommandType.Text;
        static ICommandExpression<KeyValuePair<string, string>> Command;

        Because of = () => Command =
            CommandManager.Instance
                .DefineCommand<KeyValuePair<string, string>>(CommandText, CommandType);

        It should_not_return_null = () =>
            Command.ShouldNotBeNull();

        It should_respect_provided_command_text = () =>
            Command.CommandText.ShouldEqual(CommandText);

        It should_respect_provided_command_type = () =>
            Command.DbCommandType.ShouldEqual(CommandType);
    }
}