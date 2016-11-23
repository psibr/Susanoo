namespace Susanoo.Transforms
{
    /// <summary>
    /// Replacement comparison for override
    /// </summary>
    public class ComparisonOverride
    {
        /// <summary>
        /// Gets the override text.
        /// </summary>
        /// <value>The override text.</value>
        public string OverrideText { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonOverride"/> class.
        /// </summary>
        /// <param name="overrideText">The override text.</param>
        public ComparisonOverride(string overrideText)
        {
            OverrideText = "\r\n    AND " + overrideText;
        }

        /// <summary>
        /// Override Text.
        /// </summary>
        public override string ToString()
        {
            return OverrideText;
        }
    }
}