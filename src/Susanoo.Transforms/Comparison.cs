using System;

namespace Susanoo.Transforms
{
    /// <summary>
    /// Comparison options
    /// </summary>
    public static class Comparison
    {
        /// <summary>
        /// Remove the property from the comparison.
        /// </summary>
        public static CompareMethod Ignore => CompareMethod.Ignore;

        /// <summary>
        /// Values must equal.
        /// </summary>
        public static CompareMethod Equal => CompareMethod.Equal;

        /// <summary>
        /// Column value must be less than parameter value.
        /// </summary>
        public static CompareMethod LessThan => CompareMethod.LessThan;

        /// <summary>
        /// Column value must be less than or equal parameter value.
        /// </summary>
        public static CompareMethod LessThanOrEqual => CompareMethod.LessThanOrEqual;

        /// <summary>
        /// Column value must be greater than parameter value.
        /// </summary>
        public static CompareMethod GreaterThan => CompareMethod.GreaterThan;

        /// <summary>
        /// Column value must be greater than or equal parameter value.
        /// </summary>
        public static CompareMethod GreaterThanOrEqual => CompareMethod.GreaterThanOrEqual;

        /// <summary>
        /// Values must NOT equal.
        /// </summary>
        public static CompareMethod NotEqual => CompareMethod.NotEqual;

        /// <summary>
        /// Column value must start with parameter value.
        /// </summary>
        public static CompareMethod StartsWith => CompareMethod.StartsWith;

        /// <summary>
        /// Column value must end with parameter value.
        /// </summary>
        public static CompareMethod EndsWith => CompareMethod.EndsWith;

        /// <summary>
        /// Column value must contain parameter value.
        /// </summary>
        public static CompareMethod Contains => CompareMethod.Contains;

        /// <summary>
        /// Overrides the comparison with a provided comparison string.
        /// </summary>
        /// <param name="overrideText">The override text.</param>
        public static ComparisonOverride Override(string overrideText)
        {
            return new ComparisonOverride(overrideText);
        }

        /// <summary>
        /// Gets the comparison format string.
        /// </summary>
        /// <param name="compare">The compare.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentOutOfRangeException">CompareMethod is not mapped to an action.</exception>
        public static string GetComparisonFormat(CompareMethod compare)
        {
            var comparisonString = "";

            switch (compare)
            {
                case CompareMethod.Equal:
                    comparisonString = "\r\n    AND {1} = @{0}";
                    break;
                case CompareMethod.NotEqual:
                    comparisonString = "\r\n    AND {1} <> @{0}";
                    break;
                case CompareMethod.LessThan:
                    comparisonString = "\r\n    AND {1} < @{0}";
                    break;
                case CompareMethod.LessThanOrEqual:
                    comparisonString = "\r\n    AND {1} <= @{0}";
                    break;
                case CompareMethod.GreaterThan:
                    comparisonString = "\r\n    AND {1} > @{0}";
                    break;
                case CompareMethod.GreaterThanOrEqual:
                    comparisonString = "\r\n    AND {1} >= @{0}";
                    break;
                case CompareMethod.StartsWith:
                    comparisonString = "\r\n    AND {1} LIKE @{0} + '%'";
                    break;
                case CompareMethod.EndsWith:
                    comparisonString = "\r\n    AND {1} LIKE '%' + @{0}";
                    break;
                case CompareMethod.Contains:
                    comparisonString = "\r\n    AND {1} LIKE '%' + @{0} + '%'";
                    break;
                case CompareMethod.Ignore:
                    break;
                case CompareMethod.Override:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compare), compare, null);
            }

            return comparisonString;
        }
    }
}