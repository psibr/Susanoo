using System;
using System.Data;
using Machine.Specifications;
using Susanoo.Command;
using Susanoo.Processing;
using It = Machine.Specifications.It;

namespace Susanoo.MSpec.Tests.CommandExpression
{
    [Subject("Command Expression")]
    public class when_realizing_a_command_expression_with_DoNotStoreColumnIndexes
    {
        static string CommandText = "SELECT 1 AS Success";
        static CommandType CommandType = CommandType.Text;
        static ICommandExpression<Tuple<string, string, string>> CommandExpression;
        static INoResultCommandProcessor<Tuple<string, string, string>> CommandProcessor;

        Establish context = () =>
        {
            CommandExpression = CommandManager.Instance
                .DefineCommand<Tuple<string, string, string>>(CommandText, CommandType)
                .DoNotStoreColumnIndexes();
        };

        Because of = () => CommandProcessor = CommandExpression.Realize();


        It should_not_allow_storing_column_info = () =>
            CommandProcessor.CommandBuilderInfo.AllowStoringColumnInfo.ShouldBeFalse();

    }
}
