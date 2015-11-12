using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Machine.Specifications;
using Moq;
using Susanoo.Command;
using Susanoo.Processing;
using It = Machine.Specifications.It;

namespace Susanoo.MSpec.Tests
{
    [Subject("Command Expression")]
    public class when_realizing_a_no_result_set_command
    {
        static string CommandText = "SELECT 1 AS Success";
        static CommandType CommandType = CommandType.Text;
        static ICommandExpression<Tuple<string, string, string>> CommandExpression;
        static INoResultCommandProcessor<Tuple<string, string, string>> CommandProcessor;

        Establish context = () =>
        {
            CommandExpression = CommandManager.Instance
                .DefineCommand<Tuple<string, string, string>>(CommandText, CommandType)
                .IncludeProperty(tuple => tuple.Item1)
                .ExcludeProperty(tuple => tuple.Item3);
        };

        Because of = () => CommandProcessor = CommandExpression.Realize();

        It should_not_return_null = () =>
            CommandProcessor.ShouldNotBeNull();

        It should_respect_provided_command_text = () =>
            CommandProcessor.CommandBuilderInfo.CommandText.ShouldEqual(CommandText);

        It should_respect_provided_command_type = () =>
            CommandProcessor.CommandBuilderInfo.DbCommandType.ShouldEqual(CommandType);

        It should_whitelist_explicitly_included_properties = () =>
            CommandProcessor.CommandBuilderInfo.PropertyWhitelist.ShouldContain("Item1");

        It should_not_whitelist_implicitly_included_properties = () =>
            CommandProcessor.CommandBuilderInfo.PropertyWhitelist.ShouldNotContain("Item2");

        It should_blacklist_explicitly_excluded_properties = () =>
            CommandProcessor.CommandBuilderInfo.PropertyBlacklist.ShouldContain("Item3");

        It should_allow_storing_column_info_by_default = () =>
            CommandProcessor.CommandBuilderInfo.AllowStoringColumnInfo.ShouldBeTrue();

    }
}
