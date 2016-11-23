//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Susanoo.Command;
//using Susanoo.Pipeline;
//using Susanoo.Processing;

//namespace Susanoo.Transforms
//{
//    /// <summary>
//    /// Class SusanooExtensions.
//    /// </summary>
//    public static class SusanooExtensions
//    {
//        /// <summary>
//        /// Builds a computed insert statement.
//        /// </summary>
//        /// <typeparam name="TFilter">The type of the filter.</typeparam>
//        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
//        public static INoResultCommandProcessor<TFilter> DefineInsert<TFilter>(string tableName, Func<ICommandExpression<TFilter>, ICommandExpression<TFilter>> commandFunc = null)
//        {
//            var guid = Guid.NewGuid().ToString();

//            var command = Susanoo.Instance.Bootstrapper
//                .ResolveDependency<ICommandBuilder>()
//                .DefineCommand<TFilter>(guid, CommandType.Text);

//            if (commandFunc != null)
//                command = commandFunc(command);

//            var commandInfo = (ICommandBuilderInfo<TFilter>)command;

//            var columnNames = Susanoo.Instance.Bootstrapper
//                .ResolveDependency<IPropertyMetadataExtractor>()
//                .FindAllowedProperties(
//                    typeof(TFilter),
//                    DescriptorActions.Insert,
//                    commandInfo.PropertyWhitelist.ToArray(),
//                    commandInfo.PropertyBlacklist.ToArray())
//                .Select(p => p.Value.ActiveAlias)
//                .Aggregate(string.Empty, (p, c) =>
//                    p.Length == 0 ? c : p + ", " + c);

//            var insertReadyFormat = $"INSERT INTO {tableName} ( {columnNames} ) VALUES {{0}}";

//            var xform = new CommandTransform("Insert Builder",
//                info => new ExecutableCommandInfo
//                {
//                    CommandText = info.CommandText.Replace(guid, string.Format(insertReadyFormat, null)), //
//                    DbCommandType = CommandType.Text,
//                    Parameters = info.Parameters
//                });

//            return command.Compile()
//                .ApplyTransforms(source => new[] {xform});


//        }
//    }
//}
