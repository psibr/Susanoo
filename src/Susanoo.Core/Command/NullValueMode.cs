namespace Susanoo.Command
{
    /// <summary>
    /// Opt-in levels for sending null values in parameters.
    /// </summary>
    public enum NullValueMode
    {
        /// <summary>
        /// Default option, standard ADO.NET behavior, values of null exclude the parameter from the parameter set.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Replaces null with DbNull on filter properties when no modifier action is provided.
        /// </summary>
        FilterOnlyMinimum,

        /// <summary>
        /// Replaces null with DbNull on all filter properties.
        /// </summary>
        FilterOnlyFull,

        /// <summary>
        /// Replaces null with DbNull on explicit parameters only.
        /// </summary>
        ExplicitParametersOnly,

        /// <summary>
        /// Replaces null with DbNull on all parameters EXCEPT constants.
        /// </summary>
        Full
    }
}