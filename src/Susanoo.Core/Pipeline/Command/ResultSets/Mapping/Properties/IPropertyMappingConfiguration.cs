namespace Susanoo.Pipeline.Command.ResultSets.Mapping.Properties
{
    /// <summary>
    /// Allows configuration of the Susanoo mapper at the property level during command definition.
    /// </summary>
    public interface IPropertyMappingConfiguration : IFluentPipelineFragment
    {
        /// <summary>
        /// Uses the specified alias when mapping from the data call.
        /// </summary>
        /// <param name="columnNameAlias">The alias.</param>
        /// <returns>Susanoo.IResultMappingExpression&lt;TFilter,TResult&gt;.</returns>
        IPropertyMappingConfiguration UseAlias(string columnNameAlias);
    }
}