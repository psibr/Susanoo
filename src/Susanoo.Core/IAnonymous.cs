namespace Susanoo
{
    /// <summary>
    /// Denotes that a type is a runtime generated type from Susanoo
    /// </summary>
    public interface IAnonymous
    {
        /// <summary>
        /// Allows quick retrieval and setting of fields on Susanoo's runtime generated types.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>System.Object.</returns>
        object this[string fieldName] { get; set; }
    }
}
