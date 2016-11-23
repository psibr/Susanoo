using System;
using System.Data;
using System.Data.Common;
using Machine.Specifications;
using Moq;
using Susanoo.Command;
using Susanoo.Processing;
using It = Machine.Specifications.It;

namespace Susanoo.MSpec.Tests.CommandBuilderInfo
{
    [Subject("CommandBuilderInfo")]
    public class when_building_parameters_in_implicit_property_mode
    {
        static string CommandText = "SELECT 1 AS Success";
        static CommandType CommandType = CommandType.Text;
        static ICommandExpression<Tuple<string, string, string>> CommandExpression;
        static INoResultCommandProcessor<Tuple<string, string, string>> CommandProcessor;
        static Mock<IDatabaseManager> MockDatabaseManager;
        static DbParameter[] Parameters;

        Establish context = () =>
        {
            CommandExpression = CommandManager.Instance
                .DefineCommand<Tuple<string, string, string>>(CommandText, CommandType)
                .IncludeProperty(tuple => tuple.Item1)
                .ExcludeProperty(tuple => tuple.Item3);

            CommandProcessor = CommandExpression.Realize();

            MockDatabaseManager = new Mock<IDatabaseManager>();
            MockDatabaseManager
                .Setup(manager =>
                    manager.CreateParameter())
                .Returns(() =>
                    new Mock<DbParameter>()
                        .SetupProperty(parameter => parameter.ParameterName)
                        .SetupProperty(parameter => parameter.Direction)
                        .SetupProperty(parameter => parameter.DbType)
                        .SetupProperty(parameter => parameter.Value)
                        .Object);
        };

        Because of = () => Parameters = CommandProcessor.CommandBuilderInfo
            .BuildParameters(
                MockDatabaseManager.Object,
                new Tuple<string, string, string>("Explicit", "Implicit", "Excluded"),
                null, null);

        It should_include_explicit_properties = () =>
            Parameters.ShouldContain(parameter => parameter.ParameterName == "Item1");

        It should_include_implicit_properties = () =>
            Parameters.ShouldContain(parameter => parameter.ParameterName == "Item2");

        It should_not_include_excluded_properties = () =>
            Parameters.ShouldNotContain(parameter => parameter.ParameterName == "Item3");
    }
}
